using System.Linq;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Utils;
using UnityEngine;

namespace MegameAsteroids.View.Managers {
    public class GameSession : MonoBehaviour {
        public static GameSession I { get; private set; }

        public bool NewGameWasStarted { get; set; }

        private void Awake() {
            var existsSession = GetExistsSession();
            if (existsSession != null) {
                LoadSettings();
                Destroy(gameObject);

                return;
            }

            I = this;
            LoadSettings();

            // save session
            DontDestroyOnLoad(this);
        }

        private void OnDestroy() {
            if (I == this) {
                I = null;
            }
        }

        private GameSession GetExistsSession() {
            var session = FindObjectsOfType<GameSession>();

            return session.FirstOrDefault(gameSession => gameSession != this);
        }

        private void LoadSettings() {
            AudioUtils.I.SfxSource.volume = GameSettings.I.SfxVolume / 100;
        }
    }
}
