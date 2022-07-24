using UnityEngine;

namespace MegameAsteroids.Core.Interfaces {
    public interface ISetDirection {
        public void SetDirection(Vector2 newDirection);

        public void SetPosition(Vector2 newPosition);
    }
}
