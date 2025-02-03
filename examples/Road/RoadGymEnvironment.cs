using Gym.Environments;
using Gym.Observations;
using NumSharp;
using Image = SixLabors.ImageSharp.Image;

internal class RoadGymEnvironment : Gym.Envs.Env {
    public RoadGymEnvironment(IEnvironmentViewerFactoryDelegate factory) {
        throw new NotImplementedException();
    }

    public override void CloseEnvironment() {
        throw new NotImplementedException();
    }

    public override Image Render(string mode = "human") {
        throw new NotImplementedException();
    }

    public override NDArray Reset() {
        throw new NotImplementedException();
    }

    public override void Seed(int seed) {
        throw new NotImplementedException();
    }

    public override Step Step(object action) {
        throw new NotImplementedException();
    }
}
