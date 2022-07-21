using MegameAsteroids.Components;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Models.Movement;
using UnityEngine;

namespace MegameAsteroids.View.Creatures.Ufo {
    [SelectionBase, RequireComponent(typeof(Rigidbody2D), typeof(HealthComponent))]
    public class UfoView : MonoBehaviour {
        [SerializeField] private float speed = 3.5f;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private Camera _camera;
        private UfoMovement _movement;
        private Rigidbody2D _rigidBody;
        private HealthComponent _heathComponent;
        private PlaySfxSound _playSfxSound;

        private void Awake() {
            _camera = Camera.main;

            _movement = new UfoMovement(_camera, speed);
            _rigidBody = GetComponent<Rigidbody2D>();
            _heathComponent = GetComponent<HealthComponent>();

            // @todo fix this ?
            _playSfxSound = GetComponent<PlaySfxSound>();

            // @todo replace this
            SetDirection(Vector3.left);
        }

        private void Start() {
            _trash.Retain(_heathComponent.SubscribeOnDead(OnDead));
        }

        private void OnDestroy()
            => _trash.Dispose();

        private void FixedUpdate() {
            var deltaTime = Time.deltaTime;

            var newPosition = _movement.GetNextPosition(_rigidBody.position, deltaTime);
            _rigidBody.position = newPosition;
        }

        public void SetDirection(Vector3 ufoDirection)
            => _movement.Direction = ufoDirection.normalized;

        private void OnDead() {
            _playSfxSound.PlayOnShot();

            // @todo Pool
            Destroy(gameObject);
        }
    }
}
