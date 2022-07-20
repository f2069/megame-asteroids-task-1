using MegameAsteroids.Core.Disposables;
using MegameAsteroids.UserInput;
using MegameAsteroids.View.Creatures.Player;
using MegameAsteroids.View.Weapons;
using UnityEngine;

namespace MegameAsteroids.View.Armory {
    public class BulletWeapon : MonoBehaviour {
        [SerializeField] private Transform prefab;
        [SerializeField] private Transform spawnPosition;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private UserInputHandler _userInput;
        private ShipView _ship;

        private void Awake() {
            _userInput = GetComponent<UserInputHandler>();

            _ship = GetComponent<ShipView>();
        }

        private void Start() {
            _trash.Retain(_userInput.SubscribeOnFire(OnFire));
        }

        private void OnDestroy()
            => _trash.Dispose();

        private void OnFire() {
            var go = Instantiate(prefab, spawnPosition.position, Quaternion.identity).GetComponent<BulletView>();

            go.SetDirection(_ship.ShotDirection);
        }
    }
}
