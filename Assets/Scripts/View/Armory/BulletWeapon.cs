using MegameAsteroids.Core.Utils;
using MegameAsteroids.UserInput;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;

namespace MegameAsteroids.View.Armory {
    public class BulletWeapon : BaseBulletWeapon {
        [SerializeField] private float cooldown = .3f;

        private UserInputHandler _userInput;
        private Cooldown _shotCooldown;

        protected override void Awake() {
            base.Awake();

            _userInput = GetComponent<UserInputHandler>();
            Ship = GetComponent<ShipView>();

            _shotCooldown = new Cooldown(cooldown);
        }

        private void Start() {
            Trash.Retain(_userInput.SubscribeOnFire(OnFire));
        }

        private void OnFire() {
            if (!_shotCooldown.IsReady) {
                return;
            }

            var go = BulletPool.Get();

            go.SetPosition(spawnPosition.position);
            go.SetDirection(Ship.ShotDirection);

            _shotCooldown.Reset();
        }
    }
}
