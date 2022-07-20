using MegameAsteroids.Core.Disposables;
using MegameAsteroids.UserInput;
using UnityEngine;

namespace MegameAsteroids {
    public class BulletWeapon : MonoBehaviour {
        [SerializeField] private Transform prefab;
        [SerializeField] private Transform spawnPosition;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private UserInputHandler _userInput;

        private void Awake() {
            _userInput = GetComponent<UserInputHandler>();
        }

        private void Start() {
            _trash.Retain(_userInput.SubscribeOnFire(OnFire));
        }

        private void OnDestroy()
            => _trash.Dispose();

        private void OnFire() {
            var go = Instantiate(prefab, spawnPosition.position, Quaternion.identity);
        }
    }
}
