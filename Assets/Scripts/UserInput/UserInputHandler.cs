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

        private bool _isLocked;

        private void Update() {
            if (!CanProcessInput()) {
                return;
            }

            GetAcceleration();
            GetRotation();
            GetFireInputDown();
        }

        private bool CanProcessInput()
            => !_isLocked;

        private void GetAcceleration() {
            // var mouseButtonPressed = Input.GetButton(InputConstants.ButtonNameAccelerateMouse);
            // OnAccelerationEvent?.Invoke(mouseButtonPressed ? 1 : 0);

            var accelerate = Input.GetAxisRaw(InputConstants.AxisNameVertical);
            OnAccelerationEvent?.Invoke(accelerate);
        }

        private void GetRotation() {
            // var mousePosition = Input.mousePosition;
            var rotate = Input.GetAxisRaw(InputConstants.AxisNameHorizontal);

            OnRotatedEvent?.Invoke(rotate);
        }

        private void GetFireInputDown() {
            // if (Input.GetButtonDown(InputConstants.ButtonNameFireMouse)) {
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

        public void SwitchLock(bool setLock) {
            _isLocked = setLock;
        }
    }
}