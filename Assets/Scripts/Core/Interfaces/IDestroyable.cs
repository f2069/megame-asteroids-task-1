using System;
using UnityEngine;

namespace MegameAsteroids.Core.Interfaces {
    public interface IDestroyable<T> {
        public delegate void OnDestroyed(T target, Transform attacker);

        public IDisposable SubscribeOnDestroy(OnDestroyed call);
    }
}
