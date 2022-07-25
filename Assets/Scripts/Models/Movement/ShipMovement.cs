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

        public float GetRotationAngle(float deltaTime) {
            if (RotateDirection != 0) {
                _rotationDegrees = Mathf.Repeat(_rotationDegrees + RotateDirection * _rotateSpeed * deltaTime, 360f);
            }

            return _rotationDegrees;
        }

        public void SetDirectionForward()
            => Direction = Quaternion.Euler(0, 0, _rotationDegrees) * Vector3.up;

        public void SetDirectionToTarget(Vector2 currentPosition, Vector2 target, float deltaTime) {
            RotateDirection = 0f;

            var targetVector = (target - currentPosition).normalized;

            var newDirection = Vector3.RotateTowards(
                Direction,
                targetVector,
                _rotateSpeed * deltaTime * Mathf.Deg2Rad,
                0f
            ).normalized;

            var signedAngle = Vector3.SignedAngle(
                Vector3.up,
                newDirection,
                Vector3.forward
            );

            Direction = newDirection;
            _rotationDegrees = signedAngle;
        }

        public override void ResetState() {
            base.ResetState();

            _acceleration = Vector2.zero;
            _rotationDegrees = 0;
        }
    }
}
