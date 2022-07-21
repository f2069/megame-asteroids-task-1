using System;
using System.Collections;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Disposables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Spawners {
    public abstract class BaseSpawner : MonoBehaviour {
        [SerializeField] protected int startAmount = 2;
        [SerializeField] protected FloatRange wavesDelay;
        [SerializeField] protected Transform startPrefab;

        protected readonly CompositeDisposable Trash = new CompositeDisposable();

        protected Coroutine WaveCoroutine;

        protected int CurrentStartAmount;
        protected int OnSceneAmount;

        protected float ScreenHalfWidth;
        protected float ScreenHalfHeight;

        private Camera _camera;

        protected abstract void SpawnObjects(int amount);

        protected virtual void Awake() {
            _camera = Camera.main;
            CurrentStartAmount = startAmount;

            ScreenHalfWidth = _camera.ViewportToWorldPoint(Vector3.right).x - .1f;
            ScreenHalfHeight = _camera.ViewportToWorldPoint(Vector3.up).y - .1f;
        }

        protected virtual void Start() {
            SpawnObjects(startAmount);

            if (!wavesDelay.Valid()) {
                throw new ArgumentException("Invalid values.", nameof(wavesDelay));
            }
        }

        protected void TryStopCoroutine() {
            if (WaveCoroutine == null) {
                return;
            }

            StopCoroutine(WaveCoroutine);
            WaveCoroutine = null;
        }

        protected IEnumerator StartNewWave() {
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

        protected virtual float GetWaveDelay() {
            return Math.Abs(wavesDelay.From - wavesDelay.To) < .001f
                ? wavesDelay.From
                : Random.Range(wavesDelay.From, wavesDelay.To);
        }

        protected virtual void OnDestroy() {
            TryStopCoroutine();

            Trash.Dispose();
        }

        protected virtual bool SceneIsEmpty()
            => OnSceneAmount == 0;
    }
}
