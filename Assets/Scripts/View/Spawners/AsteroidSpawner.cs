using System;
using System.Collections.Generic;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Dictionares;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Core.Utils;
using MegameAsteroids.View.Environment;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Spawners {
    public class AsteroidSpawner : BaseSpawner {
        [SerializeField] private AsteroidLevel startSpawnLevel;
        [SerializeField] private FloatRange startSpeedRange;
        [SerializeField] private List<AsteroidData> prefabs;

        private ObjectPool<IAsteroid> _startPool;

        protected override void Awake() {
            base.Awake();

            for (var i = 0; i < prefabs.Count; i++) {
                var data = prefabs[i];

                data.Pool = new ObjectPool<IAsteroid>(
                    () => CreateAsteroid(data.Level),
                    obj => ActionOnReleaseAsteroid(obj, data.Level),
                    ActionOnDestroyAsteroid
                );

                prefabs[i] = data;
            }

            _startPool = prefabs.Find(item => item.Level == startSpawnLevel).Pool;
        }

        private IAsteroid CreateAsteroid(AsteroidLevel level) {
            var data = prefabs.Find(item => item.Level == level);

            var asteroidGo = SpawnUtils.I.Spawn(data.Prefab.transform, Vector3.zero).GetComponent<IAsteroid>();

            asteroidGo.SetPool(data.Pool);

            if (level == startSpawnLevel) {
                ConfigurateAsteroid(
                    asteroidGo,
                    GetRandomSpawnPosition(),
                    Random.insideUnitCircle,
                    Random.Range(startSpeedRange.From, startSpeedRange.To)
                );
            }

            SubscribeOnObject(asteroidGo);

            return asteroidGo;
        }

        private void ActionOnReleaseAsteroid(IAsteroid obj, AsteroidLevel asteroidLevel) {
            if (asteroidLevel == startSpawnLevel) {
                ConfigurateAsteroid(
                    obj,
                    GetRandomSpawnPosition(),
                    Random.insideUnitCircle,
                    Random.Range(startSpeedRange.From, startSpeedRange.To)
                );
            }

            obj.ReleaseFromPool();
        }

        private void ActionOnDestroyAsteroid(IAsteroid obj)
            => obj.RetainInPool();

        protected override void SpawnObjects(int amount) {
            for (var i = 0; i < amount; i++) {
                _startPool.Get();

                OnSceneAmount += 1;
            }
        }

        private void ConfigurateAsteroid(IAsteroid asteroid, Vector2 spawnPosition, Vector2 direction, float speed) {
            asteroid.SetPosition(spawnPosition);
            asteroid.SetDirection(direction);
            asteroid.SetSpeed(speed);
        }

        private void OnSpawnParticle(
            Vector2 spawnPosition,
            Vector2 newDirection,
            float speed,
            AsteroidLevel level
        ) {
            var pool = prefabs.Find(item => item.Level == level).Pool;

            var asteroid = pool.Get();

            ConfigurateAsteroid(asteroid, spawnPosition, newDirection, speed);

            OnSceneAmount += 1;
        }

        private void OnParticleDestroyed(IAsteroid target, Transform _) {
            OnSceneAmount = Math.Max(0, OnSceneAmount - 1);

            if (!SceneIsEmpty()) {
                return;
            }

            CurrentStartAmount += 1;

            StartNewWave();
        }

        private void SubscribeOnObject(in IAsteroid target) {
            Trash.Retain(target.SubscribeOnDestroy(OnParticleDestroyed));
            Trash.Retain(target.SubscribeOnSpawnParticles(OnSpawnParticle));

            rewardManager.SubscribeOnDestroyTarget(target);
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            foreach (var asteroidData in prefabs) {
                asteroidData.Pool.Clear();
            }
        }
    }

    [Serializable]
    public struct AsteroidData {
        [SerializeField] private AsteroidLevel level;

        [SerializeField] private AsteroidView prefab;

        public AsteroidLevel Level => level;
        public AsteroidView Prefab => prefab;

        public ObjectPool<IAsteroid> Pool { get; set; }
    }
}
