using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MegameAsteroids.Core.Utils {
    public class SpawnUtils : IDisposable {
        private const string DefaultContainerName = "------ Spawn ------";

        private Transform _defaultParentTransform;

        private static SpawnUtils _instance;

        public static SpawnUtils Instance => _instance ??= new SpawnUtils();

        private SpawnUtils() {
        }

        private void InitDefaultContainer()
            => _defaultParentTransform = new GameObject(DefaultContainerName).transform;

        public Transform Spawn(
            Transform prefab,
            Vector3 position,
            Transform parentTransform = null
        ) {
            if (_defaultParentTransform == null) {
                _instance.InitDefaultContainer();
            }

            parentTransform = parentTransform == null ? _defaultParentTransform : parentTransform;

            return Object.Instantiate(prefab, position, Quaternion.identity, parentTransform);
        }

        public void Dispose() {
            _instance = null;
        }
    }
}
