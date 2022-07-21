using System.Linq;
using UnityEngine;

namespace MegameAsteroids.Components {
    public class PlaySfxSound : MonoBehaviour {
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool playOnAwake;

        private static AudioSource _sfxSource;
        private static AudioSource SfxSource => _sfxSource == null ? FindAudioSource() : _sfxSource;

        // @todo remove this
        private static AudioSource FindAudioSource()
            => _sfxSource = FindObjectsOfType<AudioSource>()
                .First(source => source.name.Equals("AudioSfxSource"));

        private void Start() {
            if (playOnAwake) {
                PlayOnShot();
            }
        }

        [ContextMenu("PlayOnShot")]
        public void PlayOnShot() => SfxSource.PlayOneShot(clip);
    }
}
