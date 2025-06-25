using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasCursor : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform canvasRt;
    public RectTransform clickImage;
    
    private Coroutine _clickRoutine;
    private readonly WaitForSeconds _waitForSeconds = new(0.1f);
    
    private void OnValidate()
    {
        canvas ??= GetComponent<Canvas>();
        canvasRt ??= canvas.GetComponent<RectTransform>();
    }

    private void Awake()
    {
        clickImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        DetectMouseClick();
    }

    private void DetectMouseClick()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_clickRoutine != null)
                StopCoroutine(_clickRoutine);

            _clickRoutine = StartCoroutine(ShowCursor());
        }
    }

    private IEnumerator ShowCursor()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvasRt, 
            Mouse.current.position.ReadValue(),
            canvas.worldCamera, 
            out var pos
        );
     
        clickImage.gameObject.SetActive(true);
        clickImage.anchoredPosition = pos;
        yield return _waitForSeconds;
        clickImage.gameObject.SetActive(false);
    }
}