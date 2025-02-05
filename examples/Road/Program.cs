using RLMatrix;
using RLMatrix.WinformsChart;
using Road;

WinformsChart chart = new();
RoadEnvironment env = new();
env.Initialise();

//DQN
int saveIndex = 0;
DQNAgentOptions opts = new(128, 10000, 0.99f, 1f, 0.05f, 50f, 0.005f, 1e-4f, chart);
IDQNNetProvider<float[]> netProvider = null!;
//netProvider = new DQNNetProvider<float[]>(32);
D2QNAgent<float[]> dqnAgent = new(opts, env, netProvider);
for (int i = 0; i < 10000; i++) {
    dqnAgent.TrainEpisode();
    if (i % 100 == 0) {
        dqnAgent.SaveAgent($"racecar_{saveIndex++}", false);
    }
}

//PPO
// PPOAgentOptions optsppo = new(32, // Number of steps agent interacts with environment before learning from its experience
//                               10000, // Size of the replay buffer
//                               0.99f, // Discount factor for rewards
//                               0.95f, // Lambda factor for Generalized Advantage Estimation
//                               3e-5f, // Learning rate
//                               0.2f, // Clipping factor for PPO's objective function
//                               0.2f, // Clipping range for value loss
//                               0.5f, // Coefficient for value loss
//                               4, // Number of PPO epochs
//                               0.5f, // Maximum allowed gradient norm
//                               chart);
//
// PPOAgent<float[]> myAgent = new(optsppo, env);
//
// for (int i = 0; i < 1000; i++) myAgent.TrainEpisode();

Console.ReadLine();
