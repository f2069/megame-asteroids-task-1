using System;
using MegameAsteroids.Core.Dictionares;
using MegameAsteroids.Core.Disposables;
using UnityEngine;

namespace MegameAsteroids.UserInput {
    public class UserInputHandler : MonoBehaviour {
        public delegate void OnFired();

        public delegate void OnRotated(float rotate);

        public delegate void OnAccelerated(float accelerate);

        private event OnFired OnFireEvent;
        private event OnRotated OnRotatedEvent;
        private event OnAccelerated OnAccelerationEvent;

        private void Update() {
            if (!CanProcessInput()) {
                return;
            }

            GetAcceleration();
            GetRotation();
            GetFireInputDown();
        }

        // @todo fix this
        private bool CanProcessInput() {
            return true;
        }

        private void GetAcceleration() {
            var accelerate = Mathf.Max(Input.GetAxisRaw(InputConstants.AxisNameVertical), 0f);

            OnAccelerationEvent?.Invoke(accelerate);
        }

        private void GetRotation() {
            var rotate = Input.GetAxisRaw(InputConstants.AxisNameHorizontal) * -1;

            OnRotatedEvent?.Invoke(rotate);
        }

        private void GetFireInputDown() {
            if (Input.GetButtonDown(InputConstants.ButtonNameFire)) {
                OnFireEvent?.Invoke();
            }
        }

        public IDisposable SubscribeOnRotate(OnRotated call) {
            OnRotatedEvent += call;

            return new ActionDisposable(() => { OnRotatedEvent -= call; });
        }

        public IDisposable SubscribeOnFire(OnFired call) {
            OnFireEvent += call;

            return new ActionDisposable(() => { OnFireEvent -= call; });
        }

        public IDisposable SubscribeOnAcceleration(OnAccelerated call) {
            OnAccelerationEvent += call;

            return new ActionDisposable(() => { OnAccelerationEvent -= call; });
        }
    }
}
