using System.Linq;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Utils;
using UnityEngine;

namespace MegameAsteroids.View.Managers {
    public class GameSession : MonoBehaviour {
        public static GameSession Instance { get; private set; }

        public bool NewGameWasStarted { get; set; }

        private void Awake() {
            var existsSession = GetExistsSession();
            if (existsSession != null) {
                LoadSettings();
                Destroy(gameObject);

                return;
            }

            Instance = this;
            LoadSettings();

            // save session
            DontDestroyOnLoad(this);
        }

        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }

        private GameSession GetExistsSession() {
            var session = FindObjectsOfType<GameSession>();

            return session.FirstOrDefault(gameSession => gameSession != this);
        }

        private void LoadSettings() {
            AudioUtils.Instance.SfxSource.volume = GameSettings.Instance.SfxVolume / 100;
        }
    }
}
