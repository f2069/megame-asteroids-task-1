using System.Collections.Generic;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.View.Creatures.Player;
using UnityEngine;

namespace MegameAsteroids.View.UI {
    public class PlayerLivesWidget : MonoBehaviour {
        [SerializeField] private ShipView playerShip;
        [SerializeField] private Transform liveItemWidget;

        private readonly CompositeDisposable _trash = new CompositeDisposable();
        private readonly List<Transform> _createdItems = new List<Transform>();

        private void Awake() {
            _trash.Retain(playerShip.SubscribeOnChangeLives(ValueChanged));
        }

        private void OnDestroy()
            => _trash.Dispose();

        private void ValueChanged(byte newValue) {
            for (var i = _createdItems.Count; i < newValue; i++) {
                _createdItems.Add(Instantiate(liveItemWidget, transform));
            }

            for (var i = 0; i < newValue; i++) {
                _createdItems[i].gameObject.SetActive(true);
            }

            for (var i = newValue; i < _createdItems.Count; i++) {
                _createdItems[i].gameObject.SetActive(false);
            }
        }
    }
}
