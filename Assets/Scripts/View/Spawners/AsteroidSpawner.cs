using System;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Spawners {
    public class AsteroidSpawner : BaseSpawner {
        [SerializeField] private FloatRange startSpeedRange;

        protected override void SpawnObjects(int amount) {
            for (var i = 0; i < amount; i++) {
                var go = Instantiate(startPrefab, GetRandomSpawnPosition(), Quaternion.identity)
                    .GetComponent<IAsteroid>();

                OnSceneAmount += 1;

                SubscribeOnObject(go);

                go.SetDirection(Random.insideUnitCircle);
                go.SetSpeed(Random.Range(startSpeedRange.From, startSpeedRange.To));
            }
        }

        private void SpawnParticle(IAsteroid target) {
            OnSceneAmount += 1;

            SubscribeOnObject(target);
        }

        private void ParticleDestroyed(IAsteroid target, Transform attacker) {
            // @todo unsubscribe ?

            OnSceneAmount = Math.Max(0, OnSceneAmount - 1);

            if (!SceneIsEmpty()) {
                return;
            }

            CurrentStartAmount += 1;

            TryStopCoroutine();
            WaveCoroutine = StartCoroutine(StartNewWave());
        }

        private void SubscribeOnObject(in IAsteroid target) {
            Trash.Retain(target.SubscribeOnDestroy(ParticleDestroyed));
            Trash.Retain(target.SubscribeOnSpawnParticles(SpawnParticle));
        }
    }
}
