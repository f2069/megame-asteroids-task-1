using UnityEngine;

namespace MegameAsteroids.Models.Reward {
    public class RewardModel {
        public int CurrentScore { get; private set; }

        public void AddReward(in int points) {
            CurrentScore += points;

            Debug.Log(CurrentScore);
        }
    }
}
