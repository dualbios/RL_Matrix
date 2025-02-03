using Gym.Collections;
using Gym.Rendering.WinForm;
using NumSharp;
using OneOf;
using RLMatrix;
using Image = SixLabors.ImageSharp.Image;

namespace Road;

public class RoadEnvironment : IEnvironment<float[]> {
    private RoadGymEnvironment myEnv;

    private float[] myState;


    public RoadEnvironment() {
        Initialise();
    }

    public int stepCounter { get; set; }
    public int maxSteps { get; set; }
    public bool isDone { get; set; }
    public OneOf<int, (int, int)> stateSize { get; set; }
    public int actionSize { get; set; }

    public float[] GetCurrentState() {
        return myState ??= new float[8];
    }

    public void Initialise() {
        myEnv = new RoadGymEnvironment(WinFormEnvViewer.Factory);
        myEnv.Reset();
        stepCounter = 0;
        maxSteps = 1000;
        isDone = false;
        actionSize = myEnv.ActionSpace.Shape.Size;
        stateSize = myEnv.ObservationSpace.Shape.Size;
    }

    public void Reset() {
        myEnv.Reset();
        isDone = false;
        stepCounter = 0;
    }

    public float Step(int actionId) {
        (NDArray? observation, float reward, bool done, Dict? information) = myEnv.Step(actionId);

        Image img = myEnv.Render();

        myState = ToFloatArray(observation);
        isDone = done || stepCounter > maxSteps;

        return reward;
    }

    private static float[] ToFloatArray(NDArray npArray) {
        try {
            float[] doubleArray = npArray.ToArray<float>();
            return Array.ConvertAll(doubleArray, item => item);
        }
        catch (Exception) {
            double[] doubleArray = npArray.ToArray<double>();
            return Array.ConvertAll(doubleArray, item => (float)item);
        }
    }
}
