using System;
using System.Collections;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Core.Utils;
using MegameAsteroids.Models.Movement;
using MegameAsteroids.UserInput;
using UnityEngine;

namespace MegameAsteroids.View.Creatures.Player {
    [SelectionBase]
    [RequireComponent(
        typeof(Rigidbody2D),
        typeof(Collider2D),
        typeof(HealthComponent)
    )]
    [RequireComponent(
        typeof(UserInputHandler),
        typeof(AudioSource)
    )]
    public class ShipView : MonoBehaviour {
        [Header("Respawn")] [SerializeField] private byte livesAmount = 3;
        [SerializeField] private float immortalTime = 3f;
        [SerializeField] private float blinkTime = .25f;

        [Header("Movement")] [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float accelerationSpeed = 15f;
        [SerializeField] [Range(0f, 360f)] private float rotateSpeed = 180f;

        [Header("Damage")] [SerializeField] private LayerMask destroyingLayers;

        [Header("Audio")] [SerializeField] private AudioClip thrustEffect;
        [SerializeField] private AudioClip explosionEffect;

        public delegate void IsDead();

        public delegate void IsLivesChange(byte newValue);

        private event IsDead OnDeadEvent;
        private event IsLivesChange OnIsLivesChangeEvent;

        public Vector2 ShotDirection => _shipMovement.Direction;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private Camera _camera;
        private UserInputHandler _userInput;
        private Rigidbody2D _rigidBody;
        private AudioSource _audioSfxSource;
        private AudioSource _audioSource;
        private ShipMovement _shipMovement;
        private IDamagable _heathComponent;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _targetPosition;

        private bool _isAccelerate;
        private Vector3 _startPosition;

        private void Awake() {
            var fixedDeltaTime = Time.fixedDeltaTime;

            _camera = Camera.main;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _userInput = GetComponent<UserInputHandler>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _audioSource = GetComponent<AudioSource>();
            _heathComponent = GetComponent<IDamagable>();

            _shipMovement = new ShipMovement(
                _camera,
                maxSpeed * fixedDeltaTime,
                accelerationSpeed * fixedDeltaTime,
                rotateSpeed,
                _rigidBody.rotation
            );

            _startPosition = transform.position;
        }

        private void Start() {
            _audioSource.clip = thrustEffect;
            _audioSource.volume = GameSettings.Instance.SfxVolume / 100;

            _audioSfxSource = AudioUtils.Instance.SfxSource;

            _trash.Retain(_userInput.SubscribeOnAcceleration(OnAcceleration));
            _trash.Retain(_userInput.SubscribeOnRotate(OnRotateDirection));
            _trash.Retain(_userInput.SubscribeOnRotateVector(OnRotateVector));

            _trash.Retain(_heathComponent.SubscribeOnDead(OnHealthEnd));

            OnIsLivesChangeEvent?.Invoke(livesAmount);
        }

        private void OnDestroy()
            => _trash.Dispose();

        private void Update() {
            var deltaTime = Time.deltaTime;

            if (_isAccelerate) {
                _shipMovement.Accelerate(deltaTime);
            }

            AudioPlayThrust();
        }

        private void FixedUpdate() {
            var deltaTime = Time.deltaTime;

            var currentPosition = _rigidBody.position;

            if (_targetPosition != Vector2.zero) {
                _shipMovement.SetDirectionToTarget(currentPosition, _targetPosition, deltaTime);
            }

            var newPosition = _shipMovement.GetNextPosition(currentPosition, deltaTime);
            _rigidBody.position = newPosition;

            var angle = _shipMovement.GetRotationAngle(deltaTime);
            _rigidBody.SetRotation(angle);
        }

        public IDisposable SubscribeOnDead(IsDead call) {
            OnDeadEvent += call;

            return new ActionDisposable(() => { OnDeadEvent -= call; });
        }

        public IDisposable SubscribeOnChangeLives(IsLivesChange call) {
            OnIsLivesChangeEvent += call;

            return new ActionDisposable(() => { OnIsLivesChangeEvent -= call; });
        }

        private void OnHealthEnd(Transform _) {
            _collider.enabled = false;

            _audioSfxSource.PlayOneShot(explosionEffect);

            livesAmount = (byte) Math.Max(0, livesAmount - 1);
            OnIsLivesChangeEvent?.Invoke(livesAmount);

            if (livesAmount == 0) {
                OnDeadEvent?.Invoke();

                Destroy(gameObject);

                return;
            }

            Respawn();
        }

        private void Respawn() {
            _spriteRenderer.enabled = false;

            _userInput.SwitchLock(true);
            _shipMovement.ResetState();

            _heathComponent.ResetState();
            _heathComponent.SetImmortal(true);

            transform.position = _startPosition;

            StartCoroutine(Blink());

            _userInput.SwitchLock(false);
        }

        private IEnumerator Blink() {
            var steps = Mathf.CeilToInt(immortalTime / blinkTime);

            for (var i = 0; i < steps; i++) {
                yield return new WaitForSeconds(blinkTime);
                _spriteRenderer.enabled = !_spriteRenderer.enabled;
            }

            _heathComponent.SetImmortal(false);
            _spriteRenderer.enabled = true;
            _collider.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var isTotalDestroy = other.gameObject.IsInLayer(destroyingLayers);
            if (!isTotalDestroy || _heathComponent.IsImmortal) {
                return;
            }

            _collider.enabled = false;

            _heathComponent.Kill(other.transform);
        }

        private void AudioPlayThrust() {
            if (_isAccelerate) {
                if (!_audioSource.isPlaying) {
                    _audioSource.Play();
                }
            } else {
                if (_audioSource.isPlaying) {
                    _audioSource.Stop();
                }
            }
        }

        private void OnAcceleration(float accelerate) {
            _isAccelerate = accelerate > 0;

            _shipMovement.SetDirectionForward();
        }

        private void OnRotateDirection(float direction) {
            _targetPosition = Vector2.zero;
            _shipMovement.RotateDirection = direction;
        }

        private void OnRotateVector(Vector2 rotate) {
            _targetPosition = _camera.ScreenToWorldPoint(rotate);
        }
    }
}
