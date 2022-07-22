using System;

namespace MegameAsteroids.Core.Interfaces {
    public interface IRewardManager {
        public int CurrentScore();

        public void ReleaseReward(int rewardPoints);

        public void SubscribeOnDestroyTarget<T>(T target) where T : IDestroyable<T>, IRewarding;

        public delegate void OnChange();

        public IDisposable SubscribeOnChange(OnChange call);
    }
}
