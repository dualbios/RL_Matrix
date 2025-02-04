using NumSharp;

namespace Road.WinForms;

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
