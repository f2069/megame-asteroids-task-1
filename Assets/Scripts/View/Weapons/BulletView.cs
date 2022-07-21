using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Movement;
using UnityEngine;

namespace MegameAsteroids.View.Weapons {
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletView : MonoBehaviour {
        [SerializeField] private float speed = 25f;
        [SerializeField] private LayerMask targetLayers;

        private Camera _camera;
        private BulletMovement _movement;
        private Rigidbody2D _rigidBody;
        private float _maxDistance;
        private Collider2D _collider;

        private void Awake() {
            _camera = Camera.main;

            _movement = new BulletMovement(_camera, speed);
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();

            _maxDistance = _camera.ViewportToWorldPoint(Vector3.right).x * 2;
        }

        private void FixedUpdate() {
            var deltaTime = Time.deltaTime;

            var newPosition = _movement.GetNextPosition(_rigidBody.position, deltaTime);
            _rigidBody.position = newPosition;

            if (_movement.TotalDistance >= _maxDistance) {
                Destroy(gameObject);
            }
        }

        public void SetDirection(Vector3 shipShotDirection)
            => _movement.Direction = shipShotDirection.normalized;

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.gameObject.IsInLayer(targetLayers)) {
                return;
            }

            _collider.enabled = false;

            var damageComponent = other.GetComponent<IDamagable>();

            damageComponent?.TakeDamage();

            // @todo Pool
            Destroy(gameObject);
        }
    }
}
