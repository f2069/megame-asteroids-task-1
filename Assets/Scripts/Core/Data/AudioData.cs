using System;
using UnityEngine;

namespace MegameAsteroids.Core.Data {
    [Serializable]
    public class AudioData {
        [SerializeField] private string id;
        [SerializeField] private AudioClip clip;

        public string Id => id;
        public AudioClip Clip => clip;
    }
}