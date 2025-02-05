using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RLMatrix;
using TorchSharp;
using TorchSharp.Utils;

namespace Road.Predict.Wpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        private float _followerSpeed;
        private float _leaderSpeed;

        private float _accelerateLeader;
        private float _distance;
        
        private const float DefAcceleration = 2.0f;

        public MainWindow() {
            InitializeComponent();

            DataContext = this;

        }

        private void Run() {
            
            DQN1D dqn1d = new DQN1D("dqn1d", 3, 1024, 3);
            dqn1d.load(@"C:\Repos\RL_Matrix\examples\Road\bin\Debug\net6.0-windows\racecar_6 - Copy\policy.pt");
            dqn1d.load(@"C:\Repos\RL_Matrix\examples\Road\bin\Debug\net6.0-windows\racecar_9\policy.pt");
            
            GameState gameState = new GameState(20, 30, 10, 10);
            
            // RoadEnvironment env = new();
            // env.Initialise();
            //
            // DQNAgentOptions opts = new(128, 10000, 0.99f, 1f, 0.05f, 50f, 0.005f, 1e-4f, null);
            // IDQNNetProvider<float[]> netProvider = null!;
            // D2QNAgent<float[]> dqnAgent = new(opts, env, netProvider);
            //
            // dqnAgent.LoadAgent("C:\\Repos\\RL_Matrix\\examples\\Road\\bin\\Debug\\net6.0-windows\\racecar_6");
            
            while (true) {
                _accelerateLeader /= 2.0f;
                gameState.LeaderSpeed += _accelerateLeader;

                torch.Tensor tensor = torch.tensor(new float[] {
                    gameState.LeaderSpeed,
                    gameState.FollowerSpeed,
                    gameState.Distance
                });
                
                //torch.Tensor actionTensor = dqn1d.forward(tensor);
                int action = (int) dqn1d.forward(tensor).argmax(1L).item<long>();
                //torch.Tensor a = torch.nn.functional.tanh(actionTensor);
                //float[] data = actionTensor.data<float>().ToArray();

                float acceleration = (int)action switch {
                    2 => DefAcceleration,
                    1 => 0,
                    _ => -DefAcceleration
                };
                
                gameState.Action(0.001f, 1);
                LeaderSpeed = gameState.LeaderSpeed;
                FollowerSpeed = gameState.FollowerSpeed;
                Distance = gameState.Distance;
            }
        }

        public float FollowerSpeed {
            get => _followerSpeed;
            set => SetField(ref _followerSpeed, value);
        }

        public float LeaderSpeed {
            get => _leaderSpeed;
            set => SetField(ref _leaderSpeed, value);
        }

        public float Distance {
            get => _distance;
            set => SetField(ref _distance, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void Accelerate_OnClick(object sender, RoutedEventArgs e) {
            _accelerateLeader += 0.5f;
        }

        private void Break_OnClick(object sender, RoutedEventArgs e) {
            _accelerateLeader -= 0.5f;
        }

        private void Start_OnClick(object sender, RoutedEventArgs e) {
            Task.Run(Run);
        }
    }
}
