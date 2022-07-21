﻿using System;
using System.Collections.Generic;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Movement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Environment {
    [RequireComponent(
        typeof(Rigidbody2D),
        typeof(Collider2D),
        typeof(PlaySfxSound))
    ]
    public class AsteroidView : MonoBehaviour, IAsteroid {
        [SerializeField] private LayerMask totalDestroyLayers;
        [SerializeField] private LayerMask projectileLayers;

        [Space] [SerializeField] private List<Transform> particles;
        [SerializeField] [Range(0f, 360f)] private float particlesAngle = 45f;
        [SerializeField] private FloatRange particlesSpeed;

        private Camera _camera;
        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        private AsteroidMovement _movement;
        private PlaySfxSound _sfxSound;
        private Vector2 _movementDirection;

        private event IAsteroid.OnDestroyed OnDestroyEvent;
        private event IAsteroid.OnSpawnParticle OnSpawnParticleEvent;

        private void Awake() {
            _camera = Camera.main;

            _movement = new AsteroidMovement(_camera, 0f);

            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _sfxSound = GetComponent<PlaySfxSound>();
        }

        private void FixedUpdate() {
            _rigidBody.position = _movement.GetNextPosition(_rigidBody.position, Time.deltaTime);
        }

        public void SetDirection(Vector3 ufoDirection)
            => _movement.Direction = ufoDirection.normalized;

        public void SetSpeed(float speed)
            => _movement.MaxSpeed = speed;

        public IDisposable SubscribeOnDestroy(IAsteroid.OnDestroyed call) {
            OnDestroyEvent += call;

            return new ActionDisposable(() => { OnDestroyEvent -= call; });
        }

        public IDisposable SubscribeOnSpawnParticles(IAsteroid.OnSpawnParticle call) {
            OnSpawnParticleEvent += call;

            return new ActionDisposable(() => { OnSpawnParticleEvent -= call; });
        }

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
            }

            OnDestroyEvent?.Invoke(this);

            SetDirection(Vector3.zero);

            // @todo Pool
            Destroy(gameObject);
        }

        private void SpawnParticles() {
            _movementDirection = _movement.Direction;

            var speed = Random.Range(particlesSpeed.From, particlesSpeed.To);
            var currentPosition = _rigidBody.transform.position;
            var eps = 1;

            foreach (var particle in particles) {
                var go = Instantiate(particle, currentPosition, Quaternion.identity)
                    .GetComponent<IAsteroid>();

                go.SetDirection(Quaternion.Euler(0, 0, particlesAngle * eps) * _movementDirection);
                go.SetSpeed(speed);

                OnSpawnParticleEvent?.Invoke(go);

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
