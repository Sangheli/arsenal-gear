using UnityEngine;

namespace SangheliCode
{
    [RequireComponent(typeof(PooledAudioPlayer))]
    [RequireComponent(typeof(PrefabSpawnerContainer))]
    public class SceneContainer : MonoBehaviour
    {
        [SerializeField] public UIManager uiManager;
        [SerializeField] public Camera camera;
        [SerializeField] public PrefabSpawnerContainer prefabSpawnerContainer;
        public PooledAudioPlayer pooledAudioPlayer;
        public CameraZoom cameraZoom;

        private void OnValidate()
        {
            pooledAudioPlayer ??= GetComponent<PooledAudioPlayer>();
            uiManager ??= GetComponent<UIManager>();
            prefabSpawnerContainer ??= GetComponent<PrefabSpawnerContainer>();
            camera ??= Camera.main;
        }
    }
}