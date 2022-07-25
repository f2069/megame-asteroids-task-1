using System;
using System.Collections.Generic;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Dictionares;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Movement;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Environment {
    [RequireComponent(
        typeof(Rigidbody2D),
        typeof(Collider2D),
        typeof(PlaySfxSound)
    )]
    [RequireComponent(
        typeof(HealthComponent),
        typeof(RewardComponent)
    )]
    public class AsteroidView : MonoBehaviour, IAsteroid {
        [SerializeField] private AsteroidLevel asteroidLevel;
        [SerializeField] private LayerMask totalDestroyLayers;
        [SerializeField] private LayerMask projectileLayers;

        [Space] [SerializeField] private List<AsteroidView> particles;
        [SerializeField] [Range(0f, 360f)] private float particlesAngle = 45f;
        [SerializeField] private FloatRange particlesSpeed;

        public IReward RewardComponent
            => _rewardComponent;

        public AsteroidLevel AsteroidLevel
            => asteroidLevel;

        private Camera _camera;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        private AsteroidMovement _movement;
        private PlaySfxSound _audioSource;
        private Vector2 _movementDirection;
        private IDamagable _heathComponent;
        private IReward _rewardComponent;

        private event IDestroyable<IAsteroid>.OnDestroyed OnDestroyEvent;
        private event IAsteroid.OnSpawnParticle OnSpawnParticleEvent;

        private readonly CompositeDisposable _trash = new CompositeDisposable();
        private IObjectPool<IAsteroid> _pool;

        private void Awake() {
            _camera = Camera.main;

            _movement = new AsteroidMovement(_camera, 0f);

            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _audioSource = GetComponent<PlaySfxSound>();
            _heathComponent = GetComponent<IDamagable>();
            _rewardComponent = GetComponent<IReward>();
        }

        private void Start() {
            _trash.Retain(_heathComponent.SubscribeOnDead(OnDead));
        }

        private void FixedUpdate() {
            _rigidBody.position = _movement.GetNextPosition(_rigidBody.position, Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var isTotalDestroy = other.gameObject.IsInLayer(totalDestroyLayers);
            if (!isTotalDestroy) {
                return;
            }

            _collider.enabled = false;

            _heathComponent.Kill(other.transform);
        }

        public void SetDirection(Vector2 ufoDirection)
            => _movement.Direction = ufoDirection.normalized;

        public void SetPosition(Vector2 newPosition)
            => transform.position = newPosition;

        public void SetSpeed(float speed)
            => _movement.MaxSpeed = speed;

        public IDisposable SubscribeOnDestroy(IDestroyable<IAsteroid>.OnDestroyed call) {
            OnDestroyEvent += call;

            return new ActionDisposable(() => { OnDestroyEvent -= call; });
        }

        public IDisposable SubscribeOnSpawnParticles(IAsteroid.OnSpawnParticle call) {
            OnSpawnParticleEvent += call;

            return new ActionDisposable(() => { OnSpawnParticleEvent -= call; });
        }

        private void OnDead(Transform attacker) {
            var isTotalDestroy = attacker.gameObject.IsInLayer(totalDestroyLayers);
            var isPartiallyDestroyed = attacker.gameObject.IsInLayer(projectileLayers);

            if (!isTotalDestroy && !isPartiallyDestroyed) {
                return;
            }

            _audioSource.PlayOnShot();

            _collider.enabled = false;

            if (isPartiallyDestroyed && particles.Count > 0) {
                SpawnParticles();
            }

            OnDestroyEvent?.Invoke(this, attacker);

            _pool.Release(this);
        }

        private void SpawnParticles() {
            _movementDirection = _movement.Direction;

            var speed = Random.Range(particlesSpeed.From, particlesSpeed.To);
            var spawnPosition = _rigidBody.transform.position;
            var eps = 1;

            foreach (var particle in particles) {
                var newDirection = Quaternion.Euler(0, 0, particlesAngle * eps) * _movementDirection;
                OnSpawnParticleEvent?.Invoke(spawnPosition, newDirection, speed, particle.AsteroidLevel);

                eps = -eps;
            }
        }

        public void SetPool(IObjectPool<IAsteroid> pool)
            => _pool = pool;

        public void ReleaseFromPool() {
            _collider.enabled = true;
            gameObject.SetActive(true);
        }

        public void RetainInPool() {
            _collider.enabled = false;
            gameObject.SetActive(false);
            _movement.ResetState();
            _heathComponent.ResetState();
        }
    }
}
