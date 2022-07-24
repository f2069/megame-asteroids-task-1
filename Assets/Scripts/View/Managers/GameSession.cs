using System.Linq;
using UnityEngine;

namespace MegameAsteroids.View.Managers {
    public class GameSession : MonoBehaviour {
        public static GameSession I { get; private set; }

        public bool NewGameWasStarted { get; set; }

        private void Awake() {
            var existsSession = GetExistsSession();
            if (existsSession != null) {
                Destroy(gameObject);

                return;
            }

            I = this;

            // save session
            DontDestroyOnLoad(this);
        }

        private GameSession GetExistsSession() {
            var session = FindObjectsOfType<GameSession>();

            return session.FirstOrDefault(gameSession => gameSession != this);
        }
    }
}