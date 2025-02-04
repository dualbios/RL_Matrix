using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using RLMatrix;

namespace Road.WinForms;

public partial class Form1 : Form {
    Pen pen = new Pen(Color.Red, 3);

    private GameState _gameState = null;

    public Form1() {
        InitializeComponent();

        var pm = new PlotModel {
            // Title = "Trigonometric functions",
            // Subtitle = "Example using the FunctionSeries",
            // PlotType = PlotType.Cartesian,
            Background = OxyColors.LightBlue
        };
        pm.Series.Add(new LineSeries());
        // pm.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.1, "cos(x)"));
        // pm.Series.Add(new FunctionSeries(t => 5 * Math.Cos(t), t => 5 * Math.Sin(t), 0, 2 * Math.PI, 0.1, "cos(t),sin(t)"));
        plot1.Model = pm;

        Paint += OnPaint;
    }

    private void OnPaint(object sender, PaintEventArgs e) {
        if (_gameState == null) {
            return;
        }

        e.Graphics.DrawLine(pen, _gameState.LeaderPosition, 50, _gameState.LeaderPosition, 100);
        e.Graphics.DrawLine(pen, _gameState.FollowerPosition, 50, _gameState.FollowerPosition, 100);
    }

    protected override void OnClosed(EventArgs e) {
        base.OnClosed(e);
        pen.Dispose();
    }

    private void button1_Click(object sender, EventArgs e) {
        Task.Run(Train);
    }

    private void Train() {
        FormDrawingService chart = new(this.plot1);
        DQNAgentOptions opts = new(128, 10000, 0.99f, 1f, 0.05f, 50f, 0.005f, 1e-4f, chart);
        IDQNNetProvider<float[]> netProvider = null!;
        //netProvider = new DQNNetProvider<float[]>(32);
        IEnvironment<float[]> env = new RoadEnvironment(DrawGameState);

        D2QNAgent<float[]> dqnAgent = new(opts, env, netProvider);
        int saveIndex = 0;

        for (int i = 0; i < 1000; i++) {
            dqnAgent.TrainEpisode();
            if (i % 100 == 0) {
                dqnAgent.SaveAgent($"racecar_{saveIndex++}", false);
            }
        }
    }

    private void DrawGameState(GameState obj) {
        _gameState = obj;
        Invalidate();
    }
}
