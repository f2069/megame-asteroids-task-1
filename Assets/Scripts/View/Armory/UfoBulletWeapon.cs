using System;
using System.Collections;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Armory {
    public class UfoBulletWeapon : MonoBehaviour {
        [SerializeField] private Transform prefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private FloatRange shotDelay;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private ShipView _ship;
        private Coroutine _coroutine;

        private void Awake() {
            // @todo fix this
            _ship = FindObjectOfType<ShipView>();
        }

        private void Start() {
            if (!shotDelay.Valid()) {
                throw new ArgumentException("Invalid values.", nameof(shotDelay));
            }

            if (_ship == null) {
                return;
            }

            _trash.Retain(_ship.SubscribeOnDead(PlayerIsDead));

            _coroutine = StartCoroutine(AttackState());
        }

        private void PlayerIsDead() {
            TryStopCoroutine();
        }

        private IEnumerator AttackState() {
            while (enabled) {
                yield return new WaitForSeconds(Random.Range(shotDelay.From, shotDelay.To));

                var goTransform = Instantiate(prefab, spawnPosition.position, Quaternion.identity);
                var bullet = goTransform.GetComponent<IBullet>();

                bullet.SetDirection(_ship.transform.position - goTransform.position);
            }
        }

        private void TryStopCoroutine() {
            if (_coroutine == null) {
                return;
            }

            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private void OnDestroy() {
            TryStopCoroutine();

            _trash.Dispose();
        }
    }
}
