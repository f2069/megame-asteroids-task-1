using System;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Interfaces;
using UnityEngine;

namespace MegameAsteroids.Components {
    public class HealthComponent : MonoBehaviour, IDamagable {
        [SerializeField] private float healthPoint = 1f;
        public bool IsImmortal { get; private set; }

        private event IDamagable.OnDead OnDeadEvent;

        private bool _isDead;
        private float _startHealthPoint;

        private void Awake() {
            _startHealthPoint = healthPoint;
        }

        public void ResetState() {
            healthPoint = _startHealthPoint;
            _isDead = IsImmortal = false;
        }

        public void SetImmortal(bool immortalState)
            => IsImmortal = immortalState;

        public void TakeDamage(float damageValue, Transform attacker) {
            if (_isDead || IsImmortal) {
                return;
            }

            healthPoint = Math.Max(0, healthPoint - damageValue);

            if (healthPoint != 0) {
                return;
            }

            _isDead = true;

            OnDeadEvent?.Invoke(attacker);
        }

        public void Kill(Transform attacker) {
            if (_isDead || IsImmortal) {
                return;
            }

            healthPoint = 0;
            _isDead = true;

            OnDeadEvent?.Invoke(attacker);
        }

        public IDisposable SubscribeOnDead(IDamagable.OnDead call) {
            OnDeadEvent += call;

            return new ActionDisposable(() => { OnDeadEvent -= call; });
        }
    }
}