using System.Linq;
using MegameAsteroids.Core.Dictionares;
using UnityEngine;

namespace MegameAsteroids.Core.Utils {
    public static class AudioUtils {
        public static AudioSource SfxSource { get; }

        static AudioUtils() {
            SfxSource = Object
                        .FindObjectsOfType<AudioSource>()
                        .First(source => source.CompareTag(TagConstants.SfxAudioSource.ToString()))
                        .GetComponent<AudioSource>();
        }
    }
}
