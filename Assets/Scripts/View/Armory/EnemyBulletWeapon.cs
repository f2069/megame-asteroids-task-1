using System;
using System.Collections;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.View.Creatures.Player;
using MegameAsteroids.View.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MegameAsteroids.View.Armory {
    public class EnemyBulletWeapon : MonoBehaviour {
        [SerializeField] private Transform prefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private TimeRange shotDelay;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private ShipView _ship;
        private Coroutine _coroutine;

        private void Awake() {
            // @todo fix this
            _ship = FindObjectOfType<ShipView>();
        }

        private void Start() {
            _coroutine = StartCoroutine(AttackState());

            if (!shotDelay.Valid()) {
                throw new ArgumentException("Invalid values.", nameof(shotDelay));
            }
        }

        private IEnumerator AttackState() {
            while (enabled) {
                var go = Instantiate(prefab, spawnPosition.position, Quaternion.identity).GetComponent<BulletView>();

                go.SetDirection(_ship.transform.position - go.transform.position);

                yield return new WaitForSeconds(Random.Range(2f, 5f));
            }
        }

        private void TryStopCoroutine() {
            if (_coroutine == null) {
                return;
            }

            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private void OnDestroy()
            => TryStopCoroutine();
    }
}
