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
        [SerializeField] private List<Transform> particles;

        private Camera _camera;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        private AsteroidMovement _movement;
        private PlaySfxSound _sfxSound;

        private void Awake() {
            _camera = Camera.main;

            _movement = new AsteroidMovement(_camera, Random.Range(speedRange.From, speedRange.To));

            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _sfxSound = GetComponent<PlaySfxSound>();

            // @todo replace this
            SetDirection(Vector3.left + Vector3.down * .5f);
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

            if (isTotalDestroy || particles.Count == 0) {
                TotalDestroy();
            } else {
                SpawnParticles();
            }

            // @todo Pool
            Destroy(gameObject);
        }

        private void TotalDestroy() {
            Debug.Log("TotalDestroy");
        }

        private void SpawnParticles() {
            Debug.Log("SpawnParticles");
        }
    }
}
