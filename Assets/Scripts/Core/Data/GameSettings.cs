using MegameAsteroids.Core.Properties;
using UnityEngine;

namespace MegameAsteroids.Core.Data {
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings", order = 0)]
    public class GameSettings : ScriptableObject {
        [SerializeField] [Range(0f, 100f)] private float sfxVolume = 80f;
        [SerializeField] private BoolObservableProperty inputWithMouse;

        public float SfxVolume => sfxVolume;
        public BoolObservableProperty InputWithMouse => inputWithMouse;

        public static GameSettings Instance => _instance == null ? LoadGameSettings() : _instance;

        private static GameSettings _instance;

        private static GameSettings LoadGameSettings()
            => _instance = Resources.Load<GameSettings>("GameSettings");
    }
}
