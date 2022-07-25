using System;
using MegameAsteroids.Core.Disposables;
using UnityEngine;

namespace MegameAsteroids.Core.Properties {
    [Serializable]
    public abstract class ObservableProperty<TPropertyType> {
        [SerializeField] protected TPropertyType currentValue;

        public delegate void OnPropertyChanged(TPropertyType newValue, TPropertyType oldValue);

        protected event OnPropertyChanged OnChanged;

        public virtual TPropertyType Value {
            get => currentValue;
            set => SetValue(value);
        }

        protected void SetValue(TPropertyType value) {
            var isEquals = currentValue?.Equals(value) ?? false;
            if (isEquals) {
                return;
            }

            var oldValue = currentValue;

            currentValue = value;

            OnChangedCallback(value, oldValue);
        }

        protected virtual void OnChangedCallback(TPropertyType newvalue, TPropertyType oldvalue) {
            OnChanged?.Invoke(newvalue, oldvalue);
        }

        public IDisposable Subscribe(OnPropertyChanged call, bool invoke = false) {
            OnChanged += call;

            if (invoke) {
                call(currentValue, currentValue);
            }

            return new ActionDisposable(() => OnChanged -= call);
        }
    }
}
