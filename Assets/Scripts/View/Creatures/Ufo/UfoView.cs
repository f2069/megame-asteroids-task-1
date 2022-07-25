using System;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Core.Utils;
using MegameAsteroids.Models.Movement;
using UnityEngine;

namespace MegameAsteroids.View.Creatures.Ufo {
    [SelectionBase]
    [RequireComponent(
        typeof(Rigidbody2D),
        typeof(Collider2D),
        typeof(AudioSource)
    )]
    [RequireComponent(
        typeof(HealthComponent),
        typeof(RewardComponent)
    )]
    public class UfoView : MonoBehaviour, IUfo {
        [SerializeField] private LayerMask destroyingLayers;
        [SerializeField] private float speed = 3.5f;

        [Header("Audio")] [SerializeField] private AudioClip explosionEffect;

        public IReward RewardComponent
            => _rewardComponent;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private Camera _camera;
        private UfoMovement _movement;
        private Rigidbody2D _rigidBody;
        private IDamagable _heathComponent;
        private AudioSource _audioSfxSource;
        private AudioSource _audioSource;
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
            _audioSource = GetComponent<AudioSource>();
            _audioSfxSource = AudioUtils.I.SfxSource;
        }

        private void Start() {
            _audioSource.volume = GameSettings.I.SfxVolume / 100;

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

            _audioSfxSource.PlayOneShot(explosionEffect);

            Destroy(gameObject);
        }
    }
}
