using MegameAsteroids.Core.Interfaces;
using UnityEngine;

namespace MegameAsteroids.Components {
    public class RewardComponent : MonoBehaviour, IReward {
        [SerializeField] private int rewardPoints;

        public int RewardPoints()
            => rewardPoints;
    }
}