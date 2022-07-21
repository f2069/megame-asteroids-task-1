using MegameAsteroids.Core.Disposables;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;

namespace MegameAsteroids.View.Managers {
    public class LevelManager : MonoBehaviour {
        [SerializeField] private ShipView ship;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private void Start() {
            ship.SubscribeOnDead(PlayerIsDead);
        }

        private void PlayerIsDead() {
            Debug.Log("End game");
        }

        private void OnDisable()
            => _trash.Dispose();
    }
}
