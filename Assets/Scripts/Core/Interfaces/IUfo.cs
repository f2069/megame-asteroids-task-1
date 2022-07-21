using System;

namespace MegameAsteroids.Core.Interfaces {
    public interface IUfo : ISetDirection {
        public delegate void OnDestroyed(IUfo target);

        public IDisposable SubscribeOnDestroy(OnDestroyed call);
    }
}
