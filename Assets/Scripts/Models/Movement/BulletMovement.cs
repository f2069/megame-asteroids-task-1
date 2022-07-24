using UnityEngine;

namespace MegameAsteroids.Models.Movement {
    public class BulletMovement : BaseMovement {
        public BulletMovement(Camera mainCamera, float maxSpeed) : base(mainCamera, maxSpeed) {
        }

        public override Vector2 GetNextPosition(Vector2 currentPosition, float deltaTime) {
            var step = MaxSpeed * deltaTime * Direction;
            TotalDistance += step.magnitude;

            return ScreenLoopPosition(currentPosition + step);
        }
    }
}