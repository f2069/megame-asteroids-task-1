namespace MegameAsteroids.Models.Reward {
    public class RewardModel {
        public int CurrentScore { get; private set; }

        public void AddReward(int points) {
            CurrentScore += points;
        }
    }
}
