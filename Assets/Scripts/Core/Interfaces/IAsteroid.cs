using System;

namespace MegameAsteroids.Core.Interfaces {
    public interface IAsteroid : ISetDirection, ISetSpeed {
        public delegate void OnDestroyed(IAsteroid target);

        public delegate void OnSpawnParticle(IAsteroid target);

        public IDisposable SubscribeOnDestroy(OnDestroyed call);

        public IDisposable SubscribeOnSpawnParticles(OnSpawnParticle call);
    }
}
