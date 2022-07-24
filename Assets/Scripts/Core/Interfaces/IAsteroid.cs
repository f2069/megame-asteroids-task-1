using System;
using MegameAsteroids.Core.Dictionares;
using UnityEngine;

namespace MegameAsteroids.Core.Interfaces {
    public interface IAsteroid : ISetDirection, ISetSpeed, IDestroyable<IAsteroid>, ISetPool<IAsteroid>, IRewarding {
        public delegate void OnSpawnParticle(
            Vector2 spawnPosition,
            Vector2 newDirection,
            float speed,
            AsteroidLevel level
        );

        public IDisposable SubscribeOnSpawnParticles(OnSpawnParticle call);

        public AsteroidLevel AsteroidLevel { get; }
    }
}
