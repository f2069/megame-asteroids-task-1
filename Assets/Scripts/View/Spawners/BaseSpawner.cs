using System;
using System.Collections;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.View.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Spawners {
    public abstract class BaseSpawner : MonoBehaviour {
        [SerializeField] protected RewardManager rewardManager;
        [SerializeField] protected int startAmount = 2;
        [SerializeField] protected bool spawnOnAwake;
        [SerializeField] protected FloatRange wavesDelay;
        [SerializeField] protected Transform startPrefab;

        protected readonly CompositeDisposable Trash = new CompositeDisposable();

        protected int CurrentStartAmount;
        protected int OnSceneAmount;

        protected float ScreenHalfWidth;
        protected float ScreenHalfHeight;

        private Camera _camera;
        private Coroutine _waveCoroutine;

        protected abstract void SpawnObjects(int amount);

        protected virtual void Awake() {
            _camera = Camera.main;
            CurrentStartAmount = startAmount;

            ScreenHalfWidth = _camera.ViewportToWorldPoint(Vector3.right).x - .1f;
            ScreenHalfHeight = _camera.ViewportToWorldPoint(Vector3.up).y - .1f;
        }

        protected virtual void Start() {
            if (!wavesDelay.Valid()) {
                throw new ArgumentException("Invalid values.", nameof(wavesDelay));
            }

            if (spawnOnAwake) {
                SpawnObjects(startAmount);
            } else {
                StartNewWave();
            }
        }

        protected void StartNewWave() {
            TryStopCoroutine();
            _waveCoroutine = StartCoroutine(RunWaveCoroutine());
        }

        private IEnumerator RunWaveCoroutine() {
            yield return new WaitForSeconds(GetWaveDelay());
            SpawnObjects(CurrentStartAmount);
        }

        protected virtual Vector2 GetRandomSpawnPosition() {
            var onHorizontal = Random.value >= .5;
            var leftTopSide = Random.value < .5;

            if (onHorizontal) {
                return new Vector2(
                    Random.Range(-ScreenHalfWidth, ScreenHalfWidth),
                    leftTopSide ? ScreenHalfHeight : -ScreenHalfHeight
                );
            }

            return new Vector2(
                leftTopSide ? -ScreenHalfWidth : ScreenHalfWidth,
                Random.Range(-ScreenHalfHeight, ScreenHalfHeight)
            );
        }

        private void TryStopCoroutine() {
            if (_waveCoroutine == null) {
                return;
            }

            StopCoroutine(_waveCoroutine);
            _waveCoroutine = null;
        }

        private float GetWaveDelay() {
            return Math.Abs(wavesDelay.From - wavesDelay.To) < .001f
                ? wavesDelay.From
                : Random.Range(wavesDelay.From, wavesDelay.To);
        }

        protected virtual void OnDestroy() {
            TryStopCoroutine();

            Trash.Dispose();
        }

        protected bool SceneIsEmpty()
            => OnSceneAmount == 0;
    }
}
