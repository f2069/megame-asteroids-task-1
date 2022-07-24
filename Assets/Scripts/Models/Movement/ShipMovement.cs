using UnityEngine;

namespace MegameAsteroids.Models.Movement {
    public class ShipMovement : BaseMovement {
        public float RotateDirection;

        private readonly float _accelerationSpeed;
        private readonly float _rotateSpeed;

        private Vector2 _acceleration;

        private float _rotationDegrees;
        private bool _isAccelerate;

        private float _currentSpeed;

        public override Vector2 Direction => Quaternion.Euler(0, 0, _rotationDegrees) * Vector3.up;

        public ShipMovement(Camera mainCamera,
                            float maxSpeed,
                            float accelerationSpeed,
                            float rotateSpeed,
                            float rigidBodyRotation)
            : base(mainCamera, maxSpeed) {
            _accelerationSpeed = accelerationSpeed;
            _rotateSpeed = rotateSpeed;

            _rotationDegrees = rigidBodyRotation;
        }

        public override Vector2 GetNextPosition(Vector2 currentPosition, float deltaTime)
            => ScreenLoopPosition(currentPosition + _acceleration);

        public void Accelerate(float deltaTime) {
            _acceleration += Direction * (_accelerationSpeed * deltaTime);
            _acceleration = Vector2.ClampMagnitude(_acceleration, MaxSpeed);
        }

        public float Rotate(float deltaTime) {
            if (RotateDirection != 0) {
                _rotationDegrees = Mathf.Repeat(_rotationDegrees + RotateDirection * _rotateSpeed * deltaTime, 360f);
            }

            return _rotationDegrees;
        }

        public override void ResetState() {
            base.ResetState();

            _acceleration = Vector2.zero;
            _rotationDegrees = 0;
        }
    }
}
