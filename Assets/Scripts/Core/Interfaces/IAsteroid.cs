using System;
using UnityEngine;

namespace MegameAsteroids.Core.Interfaces {
    public interface IAsteroid : ISetDirection, ISetSpeed {
        public delegate void OnDestroyed(IAsteroid target, Transform attacker);

        public delegate void OnSpawnParticle(IAsteroid target);

        public IDisposable SubscribeOnDestroy(OnDestroyed call);

        public IDisposable SubscribeOnSpawnParticles(OnSpawnParticle call);
    }
}
