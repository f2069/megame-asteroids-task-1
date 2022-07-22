using System;
using UnityEngine;

namespace MegameAsteroids.Core.Interfaces {
    public interface IDamagable {
        public bool IsImmortal { get; }

        public delegate void OnDead(Transform attacker);

        public void TakeDamage(float damageValue, Transform attacker);

        public void Kill(Transform attacker);

        public void ResetState();

        public void SetImmortal(bool immortalState);

        public IDisposable SubscribeOnDead(OnDead call);
    }
}
