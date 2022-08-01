using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Utils;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;
using UnityEngine.Events;

namespace MegameAsteroids.View.Managers {
    public class LevelManager : MonoBehaviour {
        [SerializeField] private ShipView ship;
        [SerializeField] private UnityEvent onStart;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private GameSession _gameSession;

        private void Start() {
            _gameSession = GameSession.Instance;

            if (!_gameSession.NewGameWasStarted) {
                return;
            }

            ship.SubscribeOnDead(PlayerIsDead);
            onStart?.Invoke();
        }

        private void PlayerIsDead() {
            Debug.Log("PlayerIsDead");
        }

        private void OnDestroy() {
            _trash.Dispose();

            AudioUtils.Instance.Dispose();
            SpawnUtils.Instance.Dispose();
        }
    }
}