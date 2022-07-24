using MegameAsteroids.Core.Utils;
using UnityEngine;

namespace MegameAsteroids.Components {
    public class PlaySfxSound : MonoBehaviour {
        [SerializeField] private AudioClip clip;

        [ContextMenu("PlayOnShot")]
        public void PlayOnShot() => AudioUtils.I.SfxSource.PlayOneShot(clip);
    }
}
