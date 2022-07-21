using System;
using MegameAsteroids.Components;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Models.Movement;
using MegameAsteroids.UserInput;
using UnityEngine;

namespace MegameAsteroids.View.Creatures.Player {
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D), typeof(AudioSource), typeof(HealthComponent))]
    public class ShipView : MonoBehaviour {
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float accelerationSpeed = 15f;
        [SerializeField] [Range(0f, 360f)] private float rotateSpeed = 180f;

        public delegate void IsDead();

        private event IsDead OnDeadEvent;

        public Vector2 ShotDirection => _shipMovement.Direction;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private Camera _mainCamera;
        private UserInputHandler _userInput;
        private Rigidbody2D _rigidBody;
        private AudioSource _audioSource;
        private ShipMovement _shipMovement;
        private HealthComponent _heathComponent;

        private bool _isAccelerate;
        private PlaySfxSound _playSfxSound;

        private void Awake() {
            var fixedDeltaTime = Time.fixedDeltaTime;

            _mainCamera = Camera.main;

            _userInput = GetComponent<UserInputHandler>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();

            _heathComponent = GetComponent<HealthComponent>();

            // @todo fix this ?
            _playSfxSound = GetComponent<PlaySfxSound>();

            _shipMovement = new ShipMovement(
                _mainCamera,
                maxSpeed * fixedDeltaTime,
                accelerationSpeed * fixedDeltaTime,
                rotateSpeed,
                _rigidBody.rotation
            );
        }

        private void Start() {
            _trash.Retain(_userInput.SubscribeOnAcceleration(OnAcceleration));
            _trash.Retain(_userInput.SubscribeOnRotate(OnRotate));

            _trash.Retain(_heathComponent.SubscribeOnDead(OnLivesEnd));
        }

        public IDisposable SubscribeOnDead(IsDead call) {
            OnDeadEvent += call;

            return new ActionDisposable(() => { OnDeadEvent -= call; });
        }

        private void OnLivesEnd() {
            OnDeadEvent?.Invoke();
            _playSfxSound.PlayOnShot();

            Destroy(gameObject);
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
