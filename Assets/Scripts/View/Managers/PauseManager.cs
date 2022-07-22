using MegameAsteroids.Core.Disposables;
using MegameAsteroids.UserInput;
using UnityEngine;

namespace MegameAsteroids.View.Managers {
    public class PauseManager : MonoBehaviour {
        [SerializeField] private UserInputHandler userInput;
        [SerializeField] private GlobalUserInput globalUserInput;
        [SerializeField] private Transform menuWindow;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private bool _gameOnPause;
        private float _defaultTimeScale;

        private void Awake() {
            _defaultTimeScale = Time.timeScale;
        }

        private void Start() {
            _trash.Retain(globalUserInput.SubscribeOnEscape(OnEscape));
        }

        private void OnDisable()
            => _trash.Dispose();

        private void SwitchPause() {
            _gameOnPause = !_gameOnPause;

            userInput.SwitchLock(_gameOnPause);
            AudioListener.pause = _gameOnPause;
            Time.timeScale = _gameOnPause ? 0 : _defaultTimeScale;

            menuWindow.gameObject.SetActive(_gameOnPause);
        }

        private void OnEscape() {
            SwitchPause();
        }
    }
}
