using System;
using System.Collections;
using MegameAsteroids.Core.Data;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Armory {
    public class UfoBulletWeapon : BaseBulletWeapon {
        [SerializeField] private FloatRange shotDelay;

        private Coroutine _coroutine;

        protected override void Awake() {
            base.Awake();

            Ship = FindObjectOfType<ShipView>();
        }

        private void Start() {
            if (!shotDelay.Valid()) {
                throw new ArgumentException("Invalid values.", nameof(shotDelay));
            }

            if (Ship == null) {
                return;
            }

            Trash.Retain(Ship.SubscribeOnDead(OnPlayerIsDead));

            _coroutine = StartCoroutine(AttackState());
        }

        private void OnPlayerIsDead()
            => TryStopCoroutine();

        private IEnumerator AttackState() {
            while (enabled) {
                yield return new WaitForSeconds(Random.Range(shotDelay.From, shotDelay.To));

                if (Ship == null) {
                    TryStopCoroutine();

                    yield break;
                }

                var bullet = BulletPool.Get();
                var goTransform = ((MonoBehaviour) bullet).transform;

                bullet.SetPosition(spawnPosition.position);
                bullet.SetDirection(Ship.transform.position - goTransform.position);
            }
        }

        private void TryStopCoroutine() {
            if (_coroutine == null) {
                return;
            }

            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        protected override void OnDestroy() {
            TryStopCoroutine();

            base.OnDestroy();
        }
    }
}
