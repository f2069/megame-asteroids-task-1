using MegameAsteroids.Components;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Movement;
using UnityEngine;
using UnityEngine.Pool;

namespace MegameAsteroids.View.Weapons {
    [RequireComponent(
        typeof(Rigidbody2D),
        typeof(Collider2D)
    )]
    public class BulletView : MonoBehaviour, IBullet {
        [SerializeField] private float speed = 25f;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float damageValue = 1f;

        private Camera _camera;
        private BulletMovement _movement;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        private IObjectPool<IBullet> _pool;

        private float _maxDistance;
        private PlaySfxSound _audioSource;

        private void Awake() {
            _camera = Camera.main;

            _movement = new BulletMovement(_camera, speed);
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _audioSource = GetComponent<PlaySfxSound>();

            _maxDistance = _camera.ViewportToWorldPoint(Vector3.right).x * 2;
        }

        private void FixedUpdate() {
            var deltaTime = Time.deltaTime;

            var newPosition = _movement.GetNextPosition(_rigidBody.position, deltaTime);
            _rigidBody.position = newPosition;

            if (_movement.TotalDistance >= _maxDistance) {
                _pool.Release(this);
            }
        }

        public void SetDirection(Vector2 shipShotDirection)
            => _movement.Direction = shipShotDirection.normalized;

        public void SetPosition(Vector2 newPosition)
            => transform.position = newPosition;

        public void SetPool(IObjectPool<IBullet> pool)
            => _pool = pool;

        public void ReleaseFromPool() {
            _collider.enabled = true;
            gameObject.SetActive(true);

            _audioSource.PlayOnShot();
        }

        public void RetainInPool() {
            _collider.enabled = false;
            gameObject.SetActive(false);
            _movement.ResetState();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.gameObject.IsInLayer(targetLayers)) {
                return;
            }

            _pool.Release(this);

            var damageComponent = other.GetComponent<IDamagable>();
            damageComponent?.TakeDamage(damageValue, gameObject.transform);
        }
    }
}