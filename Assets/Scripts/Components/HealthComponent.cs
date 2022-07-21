using System;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Interfaces;
using UnityEngine;

namespace MegameAsteroids.Components {
    public class HealthComponent : MonoBehaviour, IDamagable {
        [SerializeField] private int livesCount = 1;

        public delegate void OnDead();

        private event OnDead OnDeadEvent;

        private bool _isDead;

        public void TakeDamage() {
            if (_isDead) {
                return;
            }

            livesCount = Math.Max(0, livesCount - 1);

            if (livesCount != 0) {
                return;
            }

            _isDead = true;

            OnDeadEvent?.Invoke();
        }

        public IDisposable SubscribeOnDead(OnDead call) {
            OnDeadEvent += call;

            return new ActionDisposable(() => { OnDeadEvent -= call; });
        }
    }
}
