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
            
            // Автоматически добавляем CameraShake на камеру если его нет
            if (camera != null && cameraShake == null)
            {
                cameraShake = camera.GetComponent<CameraShake>();
                if (cameraShake == null)
                {
                    cameraShake = camera.gameObject.AddComponent<CameraShake>();
                }
            }
        }
    }
}