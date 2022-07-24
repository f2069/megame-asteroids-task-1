using UnityEngine.Pool;

namespace MegameAsteroids.Core.Interfaces {
    public interface ISetPool<T> where T : class {
        public void SetPool(IObjectPool<T> pool);

        public void ReleaseFromPool();

        public void RetainInPool();
    }
}
