using System.Globalization;
using Gym.Environments;
using Gym.Observations;
using Gym.Spaces;
using NumSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using PointF = SixLabors.ImageSharp.PointF;
using SystemFonts = SixLabors.Fonts.SystemFonts;

namespace Road.WinForms;

internal class RoadGymEnvironment : Gym.Envs.Env {
    private const int ViewportW = 300;
    private const int ViewportH = 200;
    private IEnvViewer? _viewer;

    private int iterations = 0;

    private readonly Color c1 = Color.FromRgba(255, 0, 0, 255);
    private readonly Color c2 = Color.FromRgba(0, 255, 0, 255);
    private readonly Color cb = Color.FromRgba(255, 255, 255, 255);

    private readonly IEnvironmentViewerFactoryDelegate _viewerFactory;
    public GameState GameState { get; private set; }
    
    private readonly SixLabors.Fonts.Font font;
    private const float DefAcceleration = 2.0f;

    private List<float> _distance = new();

    public RoadGymEnvironment(IEnvironmentViewerFactoryDelegate factory) {
        _viewerFactory = factory;
        RandomState = np.random;

        // Leader speed; Follower speed; Distance
        ObservationSpace = new Box(np.array(0f, 0f, -5f), np.array(120f, 120f, 999f));
        // Follower acceleration/breaks
        //ActionSpace = new Box(np.array(0f, 0f), np.array(1f, 1f), random_state: RandomState);
        ActionSpace = new Discrete(3, np.float32, -1, -1, RandomState);

        font = SystemFonts.CreateFont("Arial", 10, SixLabors.Fonts.FontStyle.Bold);
    }

    private NumPyRandom RandomState { get; }

    public override void CloseEnvironment() {
        _viewer?.CloseEnvironment();
        _viewer = null;
    }

    public async Task Initialise() {
        _viewer = await _viewerFactory(ViewportW, ViewportH, "Road");
    }

    public override Image Render(string mode = "human") {
        if (_viewer == null) {
            throw new NullReferenceException("Viewer not initialised");
        }

        var image = new Image<Rgba32>(ViewportW, ViewportH);
        image.Mutate(a => a.BackgroundColor(new Rgba32(0, 0, 0)));

        RenderAgents(image);
        RenderStatus(image);

        _viewer.Render(image);
        return image;
    }

    private void RenderStatus(Image<Rgba32> image) {
        image.Mutate(ctx => ctx.DrawText(GameState.LeaderSpeed.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 20)));
        image.Mutate(ctx => ctx.DrawText(GameState.FollowerSpeed.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 40)));
        image.Mutate(ctx => ctx.DrawText(GameState.Distance.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 60)));
        image.Mutate(ctx => ctx.DrawText(GameState.LastAction.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 80)));
    }

    private void RenderAgents(Image<Rgba32> image) {
        image.Mutate(a => a.Fill(c1, new EllipsePolygon(new PointF(GameState.LeaderPosition, 150f), 3f)));
        image.Mutate(a => a.Fill(c2, new EllipsePolygon(new PointF(GameState.FollowerPosition, 150f), 3f)));
    }

    public override NDArray Reset() {
        iterations = 0;
        _distance = new List<float>(100);
        GameState = new GameState(np.random.uniform(5f, 10f, np.float32),
                                   np.random.uniform(35f, 40f, np.float32),
                                   np.random.uniform(0f, 10f, np.float32),
                                   np.random.uniform(0f, 5f, np.float32));
        return GameState.ToNDArray();
    }

    public override void Seed(int seed) {
        RandomState.seed(seed);
    }

    public float[] GetState() {
        return GameState.ToNDArray().ToArray<float>();
    }

    public override Step Step(object action) {
        //float acceleration = (int)action == 1 ? DefAcceleration : -DefAcceleration;
        float acceleration = (int)action switch {
            2 => DefAcceleration,
            1 => 0,
            _ => -DefAcceleration
        };

        GameState.Action(0.1f, acceleration);

        //bool done = _gameState.Distance > 60 || _gameState.Distance < 0 || iterations++ > 100;
        bool last50 = false;
        // int index = 0;
        // for (int i = _distance.Count; i >0 && index<50; i--, index++) {
        //     if( Math.Abs(_distance[i] - 30f)>1f) {
        //         last50 = false;
        //         break;
        //     }
        // }

        bool all = _distance.Skip(Math.Max(0, _distance.Count - 50)).Take(50).All(x=>Math.Abs(x - 30f) < 0.1f);

        bool done = GameState.Distance > 60 
                    || GameState.Distance < 0 
                    || (_distance.Count > 0 && all) 
                    ||  iterations > 300;

        float reward = 0;
        if (GameState.Distance < 0) {
            reward = -10;
        }

        const float k = 0.25f;
        reward += (float)Math.Exp(-k * Math.Abs(GameState.Distance - 30f));

        _distance.Add(GameState.Distance);
        iterations++;

        return new Step(GameState.ToNDArray(), reward, done, null);
    }
}