using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Utils;
using MegameAsteroids.UserInput;
using MegameAsteroids.View.Creatures.Player;
using MegameAsteroids.View.Weapons;
using UnityEngine;

namespace MegameAsteroids.View.Armory {
    public class BulletWeapon : MonoBehaviour {
        [SerializeField] private Transform prefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private float cooldown = .3f;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private UserInputHandler _userInput;
        private ShipView _ship;
        private Cooldown _shotCooldown;

        private void Awake() {
            // @todo fix this
            _userInput = GetComponent<UserInputHandler>();
            _ship = GetComponent<ShipView>();

            _shotCooldown = new Cooldown(cooldown);
        }

        private void Start() {
            _trash.Retain(_userInput.SubscribeOnFire(OnFire));
        }

        private void OnDestroy()
            => _trash.Dispose();

        private void OnFire() {
            if (!_shotCooldown.IsReady) {
                return;
            }

            var go = Instantiate(prefab, spawnPosition.position, Quaternion.identity).GetComponent<BulletView>();
            go.SetDirection(_ship.ShotDirection);

            _shotCooldown.Reset();
        }
    }
}
