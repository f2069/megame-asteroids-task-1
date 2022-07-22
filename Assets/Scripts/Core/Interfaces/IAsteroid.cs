using System;

namespace MegameAsteroids.Core.Interfaces {
    public interface IAsteroid : ISetDirection, ISetSpeed, IDestroyable<IAsteroid>, IRewarding {
        public delegate void OnSpawnParticle(IAsteroid target);

        public IDisposable SubscribeOnSpawnParticles(OnSpawnParticle call);
    }
}
