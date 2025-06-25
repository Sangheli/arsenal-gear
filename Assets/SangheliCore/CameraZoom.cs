using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Для проверки, находится ли курсор над UI

public class CameraZoom : MonoBehaviour
{
    public Camera _camera;  // Камера для изменения размера
    public float zoomSpeed = 0.5f;  // Скорость зума
    public float minSize = 5f;  // Минимальный размер
    public float maxSize = 15f;  // Максимальный размер
    public float smoothTime = 0.2f;  // Время для плавного зума
    public Slider zoomSlider;  // Слайдер для изменения зума

    public float targetSize;  // Целевой размер камеры
    private float currentSize;  // Текущий размер камеры
    public float lastSize;  // Последнее значение размера камеры

    public bool CanZoom;
    public bool CanZoomManual;
    private bool isUpdatingSlider = false;
    
    private void OnValidate()
    {
        _camera ??= GetComponent<Camera>();
    }
    
    public void Init()
    {
        CanZoom = true;
        CanZoomManual = false;
        // Инициализируем начальный размер
        var orthographicSize = _camera.orthographicSize;
        
        targetSize = orthographicSize;
        currentSize = orthographicSize;
        lastSize = currentSize;

        // Устанавливаем начальное значение слайдера
        zoomSlider.value = currentSize;
        zoomSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    private void Update()
    {
        if(!CanZoom)
            return;

        if (CanZoomManual && !EventSystem.current.IsPointerOverGameObject())
        {
            var scrollInput = Mouse.current != null ? Mouse.current.scroll.ReadValue().y : 0f;
            if (scrollInput != 0) targetSize -= scrollInput * zoomSpeed * currentSize;
        }

        targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

        // Проверяем, если значение изменилось
        if (Mathf.Abs(targetSize - lastSize) > 0.01f)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime / smoothTime);
            _camera.orthographicSize = currentSize;
            lastSize = currentSize;
            
            // Обновляем значение слайдера, если зум изменяется
            SetSlider(currentSize);
        }
    }

    public void ResetCam(float targetSize1)
    {
        SetSlider(targetSize1);
        lastSize = targetSize1;
        targetSize = targetSize1;
        currentSize = targetSize1;
    }

    public void SetSlider(float currentSize)
    {
        isUpdatingSlider = true;
        zoomSlider.value = (currentSize - minSize) / (maxSize - minSize);
        isUpdatingSlider = false;
    }

    // Слушатель для слайдера
    private void OnSliderValueChanged(float value)
    {
        if(!CanZoomManual)
            return;
        
        if(isUpdatingSlider)
            return;
        
        if (!(Mathf.Abs(value - lastSize) > 0.01f)) 
            return;
        
        targetSize = minSize + value * (maxSize-minSize);
    }

    public void Zoom(float value)
    {
        targetSize = value;
    }
}