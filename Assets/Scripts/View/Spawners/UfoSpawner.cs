using System;
using MegameAsteroids.Core.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Spawners {
    public class UfoSpawner : BaseSpawner {
        [SerializeField] [Range(0f, 1f)] private float spawnHeightMargin = .2f;

        private float _spawnHeightBounds;

        protected override void Awake() {
            base.Awake();

            _spawnHeightBounds = ScreenHalfHeight - ScreenHalfHeight * spawnHeightMargin;
        }

        protected override void SpawnObjects(int amount) {
            for (var i = 0; i < amount; i++) {
                var goTransform = Instantiate(startPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                var ufo = goTransform.GetComponent<IUfo>();

                OnSceneAmount += 1;

                SubscribeOnObject(ufo);

                ufo.SetDirection(goTransform.position.x < 0 ? Vector3.right : Vector3.left);
            }
        }

        protected override Vector2 GetRandomSpawnPosition() {
            var leftTopSide = Random.value < .5;

            return new Vector2(
                leftTopSide ? -ScreenHalfWidth : ScreenHalfWidth,
                Random.Range(-_spawnHeightBounds, _spawnHeightBounds)
            );
        }

        private void UfoDestroyed(IUfo target, Transform _) {
            // @todo unsubscribe ?

            OnSceneAmount = Math.Max(0, OnSceneAmount - 1);

            if (!SceneIsEmpty()) {
                return;
            }

            StartNewWave();
        }

        private void SubscribeOnObject(in IUfo target) {
            Trash.Retain(target.SubscribeOnDestroy(UfoDestroyed));

            rewardManager.SubscribeOnDestroyTarget(target);
        }
    }
}
