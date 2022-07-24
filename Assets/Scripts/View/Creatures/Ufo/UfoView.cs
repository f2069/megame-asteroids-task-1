using System;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Movement;
using UnityEngine;

namespace MegameAsteroids.View.Creatures.Ufo {
    [SelectionBase]
    [RequireComponent(
        typeof(Rigidbody2D),
        typeof(Collider2D),
        typeof(PlaySfxSound)
    )]
    [RequireComponent(
        typeof(HealthComponent),
        typeof(RewardComponent)
    )]
    public class UfoView : MonoBehaviour, IUfo {
        [SerializeField] private LayerMask destroyingLayers;
        [SerializeField] private float speed = 3.5f;

        public IReward RewardComponent
            => _rewardComponent;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private Camera _camera;
        private UfoMovement _movement;
        private Rigidbody2D _rigidBody;
        private IDamagable _heathComponent;
        private PlaySfxSound _playSfxSound;
        private Collider2D _collider;
        private IReward _rewardComponent;

        private event IUfo.OnDestroyed OnDestroyEvent;

        private void Awake() {
            _camera = Camera.main;

            _movement = new UfoMovement(_camera, speed);
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _heathComponent = GetComponent<IDamagable>();
            _rewardComponent = GetComponent<IReward>();

            // @todo fix this ?
            _playSfxSound = GetComponent<PlaySfxSound>();
        }

        private void Start() {
            _trash.Retain(_heathComponent.SubscribeOnDead(OnDead));
        }

        private void OnDestroy()
            => _trash.Dispose();

        private void FixedUpdate() {
            var deltaTime = Time.deltaTime;

            var newPosition = _movement.GetNextPosition(_rigidBody.position, deltaTime);
            _rigidBody.position = newPosition;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var isTotalDestroy = other.gameObject.IsInLayer(destroyingLayers);
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

        public IDisposable SubscribeOnDestroy(IUfo.OnDestroyed call) {
            OnDestroyEvent += call;

            return new ActionDisposable(() => { OnDestroyEvent -= call; });
        }

        private void OnDead(Transform attacker) {
            OnDestroyEvent?.Invoke(this, attacker);

            _playSfxSound.PlayOnShot();

            // @todo Pool
            Destroy(gameObject);
        }
    }
}