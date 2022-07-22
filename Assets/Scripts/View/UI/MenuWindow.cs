using MegameAsteroids.Core.Disposables;
using MegameAsteroids.UserInput;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MegameAsteroids.View.UI {
    public class MenuWindow : MonoBehaviour {
        [SerializeField] private UserInputHandler userInput;
        [SerializeField] private Text controlsText;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private void OnDisable()
            => _trash.Dispose();

        public void OnNewGame() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
