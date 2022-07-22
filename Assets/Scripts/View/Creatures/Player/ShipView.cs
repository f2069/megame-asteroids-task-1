using System;
using System.Collections;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Movement;
using MegameAsteroids.UserInput;
using UnityEngine;

namespace MegameAsteroids.View.Creatures.Player {
    [SelectionBase]
    [RequireComponent(
        typeof(Rigidbody2D),
        typeof(AudioSource),
        typeof(HealthComponent)
    )]
    [RequireComponent(
        typeof(Collider2D),
        typeof(PlaySfxSound),
        typeof(UserInputHandler)
    )]
    public class ShipView : MonoBehaviour {
        [Header("Respawn")] [SerializeField] private byte livesAmount = 3;
        [SerializeField] private float immortalTime = 3f;
        [SerializeField] private float blinkTime = .5f;

        [Header("Movement")] [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float accelerationSpeed = 15f;
        [SerializeField] [Range(0f, 360f)] private float rotateSpeed = 180f;

        [Header("Damage")] [SerializeField] private LayerMask destroyingLayers;

        public delegate void IsDead();

        private event IsDead OnDeadEvent;

        public Vector2 ShotDirection => _shipMovement.Direction;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private Camera _camera;
        private UserInputHandler _userInput;
        private Rigidbody2D _rigidBody;
        private AudioSource _audioSource;
        private ShipMovement _shipMovement;
        private IDamagable _heathComponent;
        private PlaySfxSound _playSfxSound;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

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

            // @todo fix this ?
            _playSfxSound = GetComponent<PlaySfxSound>();

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
            _trash.Retain(_userInput.SubscribeOnAcceleration(OnAcceleration));
            _trash.Retain(_userInput.SubscribeOnRotate(OnRotate));

            _trash.Retain(_heathComponent.SubscribeOnDead(OnHealthEnd));
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

            var newPosition = _shipMovement.GetNextPosition(_rigidBody.position, deltaTime);
            _rigidBody.position = newPosition;

            var newAngle = _shipMovement.Rotate(deltaTime);
            _rigidBody.SetRotation(newAngle);
        }

        public IDisposable SubscribeOnDead(IsDead call) {
            OnDeadEvent += call;

            return new ActionDisposable(() => { OnDeadEvent -= call; });
        }

        private void OnHealthEnd(Transform _) {
            _collider.enabled = false;

            _playSfxSound.PlayOnShot();

            livesAmount = (byte) Math.Max(0, livesAmount - 1);
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
            // @todo refactor this
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

        private void OnAcceleration(float accelerate)
            => _isAccelerate = accelerate > 0;

        private void OnRotate(float rotate)
            => _shipMovement.RotateDirection = rotate;
    }
}
