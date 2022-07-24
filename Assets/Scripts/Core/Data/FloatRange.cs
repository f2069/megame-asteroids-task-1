using System;
using UnityEngine;

namespace MegameAsteroids.Core.Data {
    [Serializable]
    public class FloatRange {
        [SerializeField] private float from = 2f;
        [SerializeField] private float to = 5f;

        public float From => from;
        public float To => to;

        public bool Valid()
            => from <= to;
    }
}