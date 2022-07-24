using MegameAsteroids.Core.Disposables;
using MegameAsteroids.UserInput;
using UnityEngine;

namespace MegameAsteroids.View.Managers {
    public class PauseManager : MonoBehaviour {
        [SerializeField] private UserInputHandler userInput;
        [SerializeField] private GlobalUserInput globalUserInput;
        [SerializeField] private Transform menuWindow;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private GameSession _gameSession;
        private bool _gameOnPause;
        private float _defaultTimeScale;

        private void Awake() {
            _defaultTimeScale = Time.timeScale;
        }

        private void Start() {
            _trash.Retain(globalUserInput.SubscribeOnEscape(OnEscape));

            _gameSession = GameSession.I;
        }

        private void OnDisable()
            => _trash.Dispose();

        public void SetPause(bool pauseState) {
            _gameOnPause = pauseState;

            userInput.SwitchLock(_gameOnPause);
            AudioListener.pause = _gameOnPause;
            Time.timeScale = _gameOnPause ? 0 : _defaultTimeScale;

            menuWindow.gameObject.SetActive(_gameOnPause);
        }

        private void OnEscape() {
            if (!_gameSession.NewGameWasStarted) {
                return;
            }

            SetPause(!_gameOnPause);
        }
    }
}
