using System;
using UnityEngine;

namespace MegameAsteroids.Core.Interfaces {
    public interface IDamagable {
        public delegate void OnDead(Transform attacker);

        public void TakeDamage(float damageValue, Transform attacker);

        public void Kill(Transform attacker);

        public IDisposable SubscribeOnDead(OnDead call);
    }
}
