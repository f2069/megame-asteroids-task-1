using System;
using MegameAsteroids.Core.Dictionares;
using MegameAsteroids.Core.Disposables;
using UnityEngine;

namespace MegameAsteroids.UserInput {
    public class GlobalUserInput : MonoBehaviour {
        public delegate void OnEscape();

        private event OnEscape OnEscEvent;

        private void Update() {
            GetEscButton();
        }

        private void GetEscButton() {
            if (Input.GetButtonDown(InputConstants.ButtonNameCancel)) {
                OnEscEvent?.Invoke();
            }
        }

        public IDisposable SubscribeOnEscape(OnEscape call) {
            OnEscEvent += call;

            return new ActionDisposable(() => { OnEscEvent -= call; });
        }
    }
}
