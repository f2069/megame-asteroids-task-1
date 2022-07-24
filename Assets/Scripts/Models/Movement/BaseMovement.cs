using UnityEngine;

namespace MegameAsteroids.Models.Movement {
    public abstract class BaseMovement {
        public float MaxSpeed { get; set; }

        public virtual Vector2 Direction { get; set; }
        public float TotalDistance { get; protected set; }

        private readonly Camera _mainCamera;

        protected BaseMovement(Camera mainCamera, float maxSpeed) {
            _mainCamera = mainCamera;
            MaxSpeed = maxSpeed;
        }

        protected Vector2 ScreenLoopPosition(in Vector2 newPosition) {
            var viewportPoint = _mainCamera.WorldToViewportPoint(newPosition);

            viewportPoint.x = Mathf.Repeat(viewportPoint.x, 1f);
            viewportPoint.y = Mathf.Repeat(viewportPoint.y, 1f);

            return _mainCamera.ViewportToWorldPoint(viewportPoint);
        }

        public virtual void ResetState() {
            Direction = Vector2.zero;
            TotalDistance = 0;
        }

        public abstract Vector2 GetNextPosition(Vector2 currentPosition, float deltaTime);
    }
}