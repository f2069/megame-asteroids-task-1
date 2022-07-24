using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Core.Utils;
using MegameAsteroids.UserInput;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;
using UnityEngine.Pool;

namespace MegameAsteroids.View.Armory {
    public class BulletWeapon : MonoBehaviour {
        [SerializeField] private Transform prefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private float cooldown = .3f;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private UserInputHandler _userInput;
        private ShipView _ship;
        private Cooldown _shotCooldown;
        private IObjectPool<IBullet> _bulletPool;

        private void Awake() {
            _userInput = GetComponent<UserInputHandler>();
            _ship = GetComponent<ShipView>();

            _shotCooldown = new Cooldown(cooldown);

            _bulletPool = new ObjectPool<IBullet>(
                CreateBullet,
                ActionOnRelease,
                ActionOnDestroy
            );
        }

        private void Start() {
            _trash.Retain(_userInput.SubscribeOnFire(OnFire));
        }

        private void OnDestroy() {
            _trash.Dispose();

            _bulletPool.Clear();
        }

        private IBullet CreateBullet() {
            var bulletGo = SpawnUtils.I.Spawn(prefab, spawnPosition.position)
                                     .GetComponent<IBullet>();

            bulletGo.SetPool(_bulletPool);

            return bulletGo;
        }

        private void ActionOnRelease(IBullet obj)
            => obj.ReleaseFromPool();

        private void ActionOnDestroy(IBullet obj)
            => obj.RetainInPool();

        private void OnFire() {
            if (!_shotCooldown.IsReady) {
                return;
            }

            var go = _bulletPool.Get();

            go.SetPosition(spawnPosition.position);
            go.SetDirection(_ship.ShotDirection);

            _shotCooldown.Reset();
        }
    }
}