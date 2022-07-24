using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Core.Utils;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;
using UnityEngine.Pool;

namespace MegameAsteroids.View.Armory {
    public abstract class BaseBulletWeapon : MonoBehaviour {
        [SerializeField] protected Transform prefab;
        [SerializeField] protected Transform spawnPosition;

        protected readonly CompositeDisposable Trash = new CompositeDisposable();

        protected ShipView Ship;
        protected ObjectPool<IBullet> BulletPool;

        protected virtual void Awake() {
            BulletPool = new ObjectPool<IBullet>(
                CreateBullet,
                ActionOnRelease,
                ActionOnDestroy
            );
        }

        protected virtual void OnDestroy() {
            Trash.Dispose();

            BulletPool.Clear();
        }

        private IBullet CreateBullet() {
            var bulletGo = SpawnUtils.I.Spawn(prefab, spawnPosition.position)
                                     .GetComponent<IBullet>();

            bulletGo.SetPool(BulletPool);

            return bulletGo;
        }

        private void ActionOnRelease(IBullet obj)
            => obj.ReleaseFromPool();

        private void ActionOnDestroy(IBullet obj)
            => obj.RetainInPool();
    }
}
