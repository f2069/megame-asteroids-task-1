using System.Collections.Generic;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Movement;
using UnityEngine;

namespace MegameAsteroids.View.Environment {
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(PlaySfxSound))]
    public class AsteroidView : MonoBehaviour {
        [SerializeField] private LayerMask totalDestroyLayers;
        [SerializeField] private LayerMask projectileLayers;
        [SerializeField] private FloatRange speedRange;

        [Space] [SerializeField] private List<Transform> particles;
        [SerializeField] [Range(0f, 360f)] private float particlesAngle = 45f;

        private Camera _camera;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        private AsteroidMovement _movement;
        private PlaySfxSound _sfxSound;
        private Vector2 _movementDirection;

        private void Awake() {
            _camera = Camera.main;

            _movement = new AsteroidMovement(_camera, Random.Range(speedRange.From, speedRange.To));

            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _sfxSound = GetComponent<PlaySfxSound>();

            // @todo replace this
            var currentDirection = Vector3.left * Random.Range(0f, 10f) + Vector3.down * Random.Range(0f, 10f);
            SetDirection(currentDirection);
        }

        private void FixedUpdate() {
            var deltaTime = Time.deltaTime;

            var newPosition = _movement.GetNextPosition(_rigidBody.position, deltaTime);
            _rigidBody.position = newPosition;
        }

        public void SetDirection(Vector3 ufoDirection)
            => _movement.Direction = ufoDirection.normalized;

        private void OnTriggerEnter2D(Collider2D other) {
            var isTotalDestroy = other.gameObject.IsInLayer(totalDestroyLayers);
            var isPartiallyDestroyed = other.gameObject.IsInLayer(projectileLayers);

            if (!isTotalDestroy && !isPartiallyDestroyed) {
                return;
            }

            _sfxSound.PlayOnShot();

            _collider.enabled = false;

            var damageComponent = other.GetComponent<IDamagable>();
            damageComponent?.TakeDamage();

            if (isPartiallyDestroyed && particles.Count > 0) {
                SpawnParticles();
            } else {
                TotalDestroy();
            }

            SetDirection(Vector3.zero);

            // @todo Pool
            Destroy(gameObject);
        }

        private void TotalDestroy() {
        }

        private void SpawnParticles() {
            _movementDirection = _movement.Direction;

            var currentPosition = _rigidBody.transform.position;
            var eps = 1;

            foreach (var particle in particles) {
                Instantiate(particle, currentPosition, Quaternion.identity)
                    .GetComponent<AsteroidView>()
                    .SetDirection(Quaternion.Euler(0, 0, particlesAngle * eps) * _movementDirection);

                eps = -eps;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (!Application.isPlaying) {
                return;
            }

            var movementDirection = _movement.Direction == Vector2.zero ? _movementDirection : _movement.Direction;
            var position = transform.position;
            var red = new Color(1f, 0f, 0f, 1f);

            Debug.DrawRay(position, movementDirection * 3f, red);

            Debug.DrawRay(position, Quaternion.Euler(0, 0, particlesAngle) * movementDirection * 2f);
            Debug.DrawRay(position, Quaternion.Euler(0, 0, particlesAngle * -1) * movementDirection * 2f);
        }
#endif
    }
}
