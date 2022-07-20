using UnityEngine;

namespace MegameAsteroids.Models.Movement {
    public class ShipMovement : BaseMovement {
        public float RotateDirection;

        private readonly float _accelerationSpeed;
        private readonly float _rotateSpeed;

        private Vector2 ForwardVector => Quaternion.Euler(0, 0, _rotationDegrees) * Vector3.up;
        private Vector2 Acceleration { get; set; }

        private float _rotationDegrees;
        private bool _isAccelerate;

        public ShipMovement(Camera mainCamera, float maxSpeed, float accelerationSpeed, float rotateSpeed)
            : base(mainCamera, maxSpeed) {
            _accelerationSpeed = accelerationSpeed;
            _rotateSpeed = rotateSpeed;
        }

        public override Vector2 GetNextPosition(Vector2 currentPosition)
            => ScreenLoopPosition(currentPosition + Acceleration);

        public void Accelerate(float deltaTime) {
            Acceleration += ForwardVector * (_accelerationSpeed * deltaTime);
            Acceleration = Vector2.ClampMagnitude(Acceleration, MaxSpeed);
        }

        public float Rotate(float deltaTime) {
            if (RotateDirection != 0) {
                _rotationDegrees = Mathf.Repeat(_rotationDegrees + RotateDirection * _rotateSpeed * deltaTime, 360f);
            }

            return _rotationDegrees;
        }
    }
}
