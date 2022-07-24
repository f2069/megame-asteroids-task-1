using MegameAsteroids.View.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace MegameAsteroids.View.UI {
    [RequireComponent(
        typeof(Text)
    )]
    public class PointsWidget : MonoBehaviour {
        [SerializeField] private RewardManager rewardManager;

        private Text _text;

        private void Awake() {
            _text = GetComponent<Text>();
        }

        private void Start() {
            rewardManager.SubscribeOnChange(ValueChanged);

            ValueChanged();
        }

        private void ValueChanged() {
            _text.text = rewardManager.CurrentScore().ToString();
        }
    }
}