using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasCursor : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform canvasRt;
    public RectTransform clickImage;
    
    private CancellationTokenSource _clickCancellation;
    
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
            CancelClick();
            _clickCancellation = new CancellationTokenSource();
            _ = ShowCursor(_clickCancellation.Token).LogExceptions();
        }
    }

    private async Task ShowCursor(CancellationToken cancellationToken)
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
        await Task.Delay(TimeSpan.FromSeconds(0.1), cancellationToken);
        clickImage.gameObject.SetActive(false);
    }

    private void CancelClick()
    {
        if (_clickCancellation == null)
            return;

        _clickCancellation.Cancel();
        _clickCancellation.Dispose();
        _clickCancellation = null;
    }

    private void OnDestroy()
    {
        CancelClick();
    }
}
