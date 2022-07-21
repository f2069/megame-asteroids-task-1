using System;

namespace MegameAsteroids.Core.Interfaces {
    public interface IDamagable {
        public delegate void OnDead();

        public void TakeDamage();

        public IDisposable SubscribeOnDead(OnDead call);
    }
}
