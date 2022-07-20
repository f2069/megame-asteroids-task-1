using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Models.Movement;
using MegameAsteroids.UserInput;
using UnityEngine;

namespace MegameAsteroids {
    public class Ship : MonoBehaviour {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float accelerationSpeed;
        [SerializeField] private float rotateSpeed;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private Camera _mainCamera;
        private UserInputHandler _userInput;
        private Rigidbody2D _rigidBody;
        private AudioSource _audioSource;
        private ShipMovement _shipMovement;

        private bool _isAccelerate;

        private void Awake() {
            _mainCamera = Camera.main;

            _userInput = GetComponent<UserInputHandler>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();

            _shipMovement = new ShipMovement(_mainCamera, maxSpeed, accelerationSpeed, rotateSpeed);
        }

        private void Start() {
            _trash.Retain(_userInput.SubscribeOnAcceleration(OnAcceleration));
            _trash.Retain(_userInput.SubscribeOnRotate(OnRotate));
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

            var newPosition = _shipMovement.GetNextPosition(_rigidBody.position);
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
