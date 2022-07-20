using MegameAsteroids.Models.Movement;
using UnityEngine;

namespace MegameAsteroids.View.Creatures.Ufo {
    [SelectionBase, RequireComponent(typeof(Rigidbody2D))]
    public class UfoView : MonoBehaviour {
        [SerializeField] private float speed = 3.5f;

        private Camera _camera;
        private UfoMovement _movement;
        private Rigidbody2D _rigidBody;

        private void Awake() {
            _camera = Camera.main;

            _movement = new UfoMovement(_camera, speed);
            _rigidBody = GetComponent<Rigidbody2D>();

            // @todo replace this
            SetDirection(Vector3.left);
        }

        private void FixedUpdate() {
            var deltaTime = Time.deltaTime;

            var newPosition = _movement.GetNextPosition(_rigidBody.position, deltaTime);
            _rigidBody.position = newPosition;
        }

        public void SetDirection(Vector3 ufoDirection)
            => _movement.Direction = ufoDirection.normalized;
    }
}
