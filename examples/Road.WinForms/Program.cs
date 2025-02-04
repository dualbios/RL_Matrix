using RLMatrix;

namespace Road.WinForms;

static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        
        FormDrawingService chart = null;
        DQNAgentOptions opts = new(128, 10000, 0.99f, 1f, 0.05f, 50f, 0.005f, 1e-4f, chart);
        IDQNNetProvider<float[]> netProvider = null!;
        //netProvider = new DQNNetProvider<float[]>(32);
        IEnvironment<float[]> env = new RoadEnvironment(null!);

        D2QNAgent<float[]> dqnAgent = new(opts, env, netProvider);
        
        
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
