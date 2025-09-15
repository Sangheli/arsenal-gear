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
        public CameraShake cameraShake;

        private void OnValidate()
        {
            pooledAudioPlayer ??= GetComponent<PooledAudioPlayer>();
            uiManager ??= GetComponent<UIManager>();
            prefabSpawnerContainer ??= GetComponent<PrefabSpawnerContainer>();
            camera ??= Camera.main;
            
            SetupCameraShake();
        }

        private void SetupCameraShake()
        {
            if (camera == null || cameraShake != null) 
                return;
            
            cameraShake = camera.GetComponent<CameraShake>();
            if (cameraShake == null) cameraShake = camera.gameObject.AddComponent<CameraShake>();
        }
    }
}