using System;
using UnityEngine;

namespace MegameAsteroids.Core.Data {
    [Serializable]
    public class TimeRange {
        [SerializeField] [Range(0f, 10f)] private float from = 2f;
        [SerializeField] [Range(1f, 10f)] private float to = 5f;

        public float From => from;
        public float To => to;

        public bool Valid()
            => from <= to;
    }
}
