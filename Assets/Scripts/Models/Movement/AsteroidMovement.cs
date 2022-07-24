using UnityEngine;

namespace MegameAsteroids.Models.Movement {
    public class AsteroidMovement : BaseMovement {
        public AsteroidMovement(Camera mainCamera, float maxSpeed) : base(mainCamera, maxSpeed) {
        }

        public override Vector2 GetNextPosition(Vector2 currentPosition, float deltaTime)
            => ScreenLoopPosition(currentPosition + MaxSpeed * deltaTime * Direction);
    }
}