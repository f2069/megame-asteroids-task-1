using System;
using System.Linq;
using MegameAsteroids.Core.Dictionares;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MegameAsteroids.Core.Utils {
    public sealed class AudioUtils : IDisposable {
        public AudioSource SfxSource { get; private set; }

        private static AudioUtils _instance;

        public static AudioUtils Instance {
            get {
                if (_instance != null) {
                    return _instance;
                }

                _instance = new AudioUtils();

                _instance.InitSources();

                return _instance;
            }
        }

        private AudioUtils() {
        }

        private void InitSources() {
            var sourceObject = Object
                               .FindObjectsOfType<AudioSource>()
                               .FirstOrDefault(source => source.CompareTag(TagConstants.SfxAudioSource.ToString()));

            if (sourceObject != null) {
                SfxSource = sourceObject.GetComponent<AudioSource>();
            }
        }

        public void Dispose() {
            _instance = null;
        }
    }
}
