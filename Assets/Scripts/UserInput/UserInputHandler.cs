using System;
using MegameAsteroids.Core.Data;
using MegameAsteroids.Core.Dictionares;
using MegameAsteroids.Core.Disposables;
using UnityEngine;

namespace MegameAsteroids.UserInput {
    public class UserInputHandler : MonoBehaviour {
        public delegate void OnFired();

        public delegate void OnRotated(float rotate);

        public delegate void OnRotatedVector(Vector2 rotate);

        public delegate void OnAccelerated(float accelerate);

        private event OnFired OnFireEvent;
        private event OnRotated OnRotatedEvent;
        private event OnRotatedVector OnRotatedVectorEvent;
        private event OnAccelerated OnAccelerationEvent;

        private readonly CompositeDisposable _trash = new CompositeDisposable();
        private GameSettings _gameSettings;

        private bool _isLocked;
        private bool _isMouseKeyboardScheme;

        private bool CanProcessInput
            => !_isLocked;

        private void Start() {
            _gameSettings = GameSettings.I;

            _trash.Retain(_gameSettings.InputWithMouse.Subscribe(OnInputTypeChange, true));
        }

        private void OnDestroy() {
            _trash.Dispose();
        }

        private void OnInputTypeChange(bool newValue, bool oldValue)
            => _isMouseKeyboardScheme = newValue;

        private void Update() {
            if (!CanProcessInput) {
                return;
            }

            GetAcceleration();
            GetRotation();
            GetFireInputDown();
        }

        private void GetAcceleration() {
            var accelerate = Input.GetAxisRaw(InputConstants.AxisNameVertical);

            if (_isMouseKeyboardScheme && accelerate == 0) {
                var mouseButtonPressed = Input.GetButton(InputConstants.ButtonNameAccelerateMouse);

                OnAccelerationEvent?.Invoke(mouseButtonPressed ? 1 : 0);

                return;
            }

            OnAccelerationEvent?.Invoke(accelerate);
        }

        private void GetRotation() {
            if (_isMouseKeyboardScheme) {
                OnRotatedVectorEvent?.Invoke(Input.mousePosition);

                return;
            }

            var rotate = Input.GetAxisRaw(InputConstants.AxisNameHorizontal);

            OnRotatedEvent?.Invoke(rotate);
        }

        private void GetFireInputDown() {
            if (
                (Input.GetButtonDown(InputConstants.ButtonNameFire))
                || (_isMouseKeyboardScheme && Input.GetButtonDown(InputConstants.ButtonNameFireMouse))
            ) {
                OnFireEvent?.Invoke();
            }
        }

        public IDisposable SubscribeOnRotate(OnRotated call) {
            OnRotatedEvent += call;

            return new ActionDisposable(() => { OnRotatedEvent -= call; });
        }

        public IDisposable SubscribeOnRotateVector(OnRotatedVector call) {
            OnRotatedVectorEvent += call;

            return new ActionDisposable(() => { OnRotatedVectorEvent -= call; });
        }

        public IDisposable SubscribeOnFire(OnFired call) {
            OnFireEvent += call;

            return new ActionDisposable(() => { OnFireEvent -= call; });
        }

        public IDisposable SubscribeOnAcceleration(OnAccelerated call) {
            OnAccelerationEvent += call;

            return new ActionDisposable(() => { OnAccelerationEvent -= call; });
        }

        public void SwitchLock(bool setLock)
            => _isLocked = setLock;
    }
}
