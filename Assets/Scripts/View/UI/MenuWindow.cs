using MegameAsteroids.Core.Dictionares;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.View.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MegameAsteroids.View.UI {
    public class MenuWindow : MonoBehaviour {
        [SerializeField] private Text controlsText;
        [SerializeField] private Transform continueButton;
        [SerializeField] private PauseManager pauseManager;

        private readonly CompositeDisposable _trash = new CompositeDisposable();
        private GameSession _gameSession;

        private void Start() {
            pauseManager.SetPause(true);

            _gameSession = GameSession.I;

            continueButton.gameObject.SetActive(_gameSession.NewGameWasStarted);

            if (_gameSession.NewGameWasStarted) {
                OnResumeGame();
            }
        }

        private void OnDestroy()
            => _trash.Dispose();

        public void OnNewGame() {
            _gameSession.NewGameWasStarted = true;

            pauseManager.SetPause(false);

            SceneManager.LoadScene(SceneConstants.Names.MainLevel.ToString());
        }

        public void OnResumeGame() {
            pauseManager.SetPause(false);
        }

        public void OnSwitchControl(bool value) {
            // @todo

            controlsText.text = value ? "Управление: клавиатура + мышь" : "Управление: клавиатура";
        }

        public void OnExitGame() {
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
}