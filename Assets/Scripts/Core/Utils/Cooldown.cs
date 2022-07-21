using UnityEngine;

namespace MegameAsteroids.Core.Utils {
    public class Cooldown {
        public bool IsReady => TimesUp <= Time.time;

        private readonly float _delayValue;
        private float TimesUp { get; set; }

        public Cooldown(float delayValue) {
            _delayValue = delayValue;

            TimesUp = Time.time;
        }

        public void Reset() {
            var startTime = Time.time;
            TimesUp = Time.time + _delayValue;
        }
    }
}
