using MegameAsteroids.Core.Utils;
using UnityEngine;

namespace MegameAsteroids.Components {
    public class PlaySfxSound : MonoBehaviour {
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool playOnAwake;

        private void Start() {
            if (playOnAwake) {
                PlayOnShot();
            }
        }

        [ContextMenu("PlayOnShot")]
        public void PlayOnShot() => AudioUtils.SfxSource.PlayOneShot(clip);
    }
}
