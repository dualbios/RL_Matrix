using Gym.Collections;
using Gym.Rendering.WinForm;
using NumSharp;
using OneOf;
using RLMatrix;

namespace Road.WinForms;

public class RoadEnvironment : IEnvironment<float[]> {
    private readonly Action<GameState> _drawAction;
    private RoadGymEnvironment myEnv;

    public int stepCounter { get; set; }
    public int maxSteps { get; set; }
    public bool isDone { get; set; }
    public OneOf<int, (int, int)> stateSize { get; set; }
    public int actionSize { get; set; }

    public float[] GetCurrentState() {
        return myEnv.GetState();
    }

    public RoadEnvironment(Action<GameState> drawAction) {
        _drawAction = drawAction;
    }

    public void Initialise() {
        myEnv = new RoadGymEnvironment(WinFormEnvViewer.Factory);
        myEnv.Initialise().GetAwaiter().GetResult();
        
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

        //Image img = myEnv.Render();
        _drawAction(myEnv.GameState);

        //myState = ToFloatArray(observation);
        isDone = done || stepCounter > maxSteps;

        return reward;
    }

    // private static float[] ToFloatArray(NDArray npArray) {
    //     try {
    //         float[] doubleArray = npArray.ToArray<float>();
    //         return Array.ConvertAll(doubleArray, item => item);
    //     }
    //     catch (Exception) {
    //         double[] doubleArray = npArray.ToArray<double>();
    //         return Array.ConvertAll(doubleArray, item => (float)item);
    //     }
    // }
}
