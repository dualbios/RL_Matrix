using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;
using RLMatrix;

namespace Road.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged {
    private PlotModel _rewardPlotView;
    private D2QNAgent<float[]> dqnAgent;

    public MainWindow() {
        InitializeComponent();

        DataContext = this;
        RewardPlotView = new PlotModel ();
        RewardPlotView.Series.Add(new LineSeries());
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Code that needs to run on the UI thread
            CanvasDrawingService chart = new(RewardPlotView);
            DQNAgentOptions opts = new(128, 10000, 0.99f, 1f, 0.05f, 50f, 0.005f, 1e-4f, chart);
            IDQNNetProvider<float[]> netProvider = null!;
            //netProvider = new DQNNetProvider<float[]>(32);
            IEnvironment<float[]> env = new RoadEnvironment(DrawGameState);
        
            dqnAgent = new(opts, env, netProvider);
        });
        
        // CanvasDrawingService chart = new(RewardPlotView);
        // DQNAgentOptions opts = new(128, 10000, 0.99f, 1f, 0.05f, 50f, 0.005f, 1e-4f, chart);
        // IDQNNetProvider<float[]> netProvider = null!;
        // //netProvider = new DQNNetProvider<float[]>(32);
        // IEnvironment<float[]> env = new RoadEnvironment(DrawGameState);
        //
        // dqnAgent = new(opts, env, netProvider);

    }

    private void Process() {
        
        int saveIndex = 0;
        
        for (int i = 0; i < 1000; i++) {
            dqnAgent.TrainEpisode();
            if (i % 100 == 0) {
                dqnAgent.SaveAgent($"racecar_{saveIndex++}", false);
            }
        }
    }

    private void DrawGameState(GameState obj) {
        Dispatcher.Invoke(() => {
            DrawingCanvas.Children.Clear();
            Rectangle rect = new Rectangle
            {
                Width = 100,
                Height = 50,
                Fill = Brushes.Blue
            };

            Canvas.SetLeft(rect, 50);
            Canvas.SetTop(rect, 50);

            DrawingCanvas.Children.Add(rect);
            
            // LeaderPosition.Text = obj.LeaderPosition.ToString(CultureInfo.InvariantCulture);
            // FollowerPosition.Text = obj.FollowerPosition.ToString(CultureInfo.InvariantCulture);
            // LeaderSpeed.Text = obj.LeaderSpeed.ToString(CultureInfo.InvariantCulture);
            // FollowerSpeed.Text = obj.FollowerSpeed.ToString(CultureInfo.InvariantCulture);
            // Distance.Text = obj.Distance.ToString(CultureInfo.InvariantCulture);
            // LastAction.Text = obj.LastAction.ToString(CultureInfo.InvariantCulture);
        });
    }

    public PlotModel RewardPlotView {
        get => _rewardPlotView;
        set => SetField(ref _rewardPlotView, value);
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

    private void Start_OnClick(object sender, RoutedEventArgs e) {
        Task.Run(Process);
    }
}
