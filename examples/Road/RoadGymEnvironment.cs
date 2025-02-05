using System.Globalization;
using Gym.Environments;
using Gym.Observations;
using Gym.Spaces;
using NumSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Font = System.Drawing.Font;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Image = SixLabors.ImageSharp.Image;
using PointF = SixLabors.ImageSharp.PointF;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SystemFonts = SixLabors.Fonts.SystemFonts;

namespace Road;

internal class RoadGymEnvironment : Gym.Envs.Env {
    private const int ViewportW = 300;
    private const int ViewportH = 200;
    private IEnvViewer? _viewer;

    private int iterations = 0;

    private readonly Color c1 = Color.FromRgba(255, 0, 0, 255);
    private readonly Color c2 = Color.FromRgba(0, 255, 0, 255);
    private readonly Color cb = Color.FromRgba(255, 255, 255, 255);

    private readonly IEnvironmentViewerFactoryDelegate _viewerFactory;
    GameState _gameState;
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
        image.Mutate(ctx => ctx.DrawText(_gameState.LeaderSpeed.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 20)));
        image.Mutate(ctx => ctx.DrawText(_gameState.FollowerSpeed.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 40)));
        image.Mutate(ctx => ctx.DrawText(_gameState.Distance.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 60)));
        image.Mutate(ctx => ctx.DrawText(_gameState.LastAction.ToString(CultureInfo.InvariantCulture), font, cb, new PointF(0, 80)));
    }

    private void RenderAgents(Image<Rgba32> image) {
        image.Mutate(a => a.Fill(c1, new EllipsePolygon(new PointF(_gameState.LeaderPosition, 150f), 3f)));
        image.Mutate(a => a.Fill(c2, new EllipsePolygon(new PointF(_gameState.FollowerPosition, 150f), 3f)));
    }

    public override NDArray Reset() {
        iterations = 0;
        _distance = new List<float>(100);
        _gameState = new GameState(np.random.uniform(10f, 20f, np.float32),
                                   np.random.uniform(5f, 40f, np.float32),
                                   np.random.uniform(10f, 20f, np.float32),
                                   np.random.uniform(0f, 35f, np.float32));
        return _gameState.ToNDArray();
    }

    public override void Seed(int seed) {
        RandomState.seed(seed);
    }

    public float[] GetState() {
        return _gameState.ToNDArray().ToArray<float>();
    }

    public override Step Step(object action) {
        //float acceleration = (int)action == 1 ? DefAcceleration : -DefAcceleration;
        float acceleration = (int)action switch {
            2 => DefAcceleration,
            1 => 0,
            _ => -DefAcceleration
        };

        _gameState.Action(0.1f, acceleration);

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

        bool done = _gameState.Distance > 60 
                    || _gameState.Distance < 0 
                    || (_distance.Count > 0 && all) 
                    ||  iterations > 300;

        float reward = 0;
        if (_gameState.Distance < 0) {
            reward = -10;
        }

        const float k = 0.25f;
        reward += (float)Math.Exp(-k * Math.Abs(_gameState.Distance - 30f));

        _distance.Add(_gameState.Distance);
        iterations++;

        return new Step(_gameState.ToNDArray(), reward, done, null);
    }
}

public class GameState {
    public float LeaderPosition { get; set; }

    public float FollowerPosition { get; set; }
    public float LeaderSpeed { get; set; }
    public float FollowerSpeed { get; set; }
    public float Distance { get; set; }
    public float LastAction { get; set; }

    public GameState(float leaderSpeed, float leaderPosition, float followerSpeed, float followerPosition) {
        LeaderSpeed = leaderSpeed;
        FollowerSpeed = followerSpeed;
        LeaderPosition = leaderPosition;
        FollowerPosition = followerPosition;
        Distance = LeaderPosition - FollowerPosition;
    }

    public GameState(float[] state) {
        LeaderSpeed = state[0];
        FollowerSpeed = state[1];
        Distance = state[2];
    }

    public NDArray ToNDArray() {
        return np.array(LeaderSpeed, FollowerSpeed, Distance);
    }

    public void Action(float timePeriod, float acceleration) {
        LastAction = acceleration;
        LeaderPosition += LeaderSpeed * timePeriod;
        FollowerPosition += FollowerSpeed * timePeriod;
        FollowerSpeed += acceleration * timePeriod;
        if (FollowerSpeed < 0) {
            FollowerSpeed = 0;
        }

        Distance = LeaderPosition - FollowerPosition;
    }
}
