using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextUpAnimated : MonoBehaviour, IReturnable, IResetable
{
    public CanvasGroup canvasGroup;
    public GameObject view;
    public Text txt;
    public float speed;
    public float fadeDuration = 1f;

    private Color _originalColor;
    private Vector3 _startPos;
    private float _elapsed;

    public GameObject GameObject => gameObject;
    public GameObject Prefab { get; set; }

    public void ResetItem()
    {
        txt.text = string.Empty;
        txt.color = _originalColor;
        transform.localPosition = _startPos;
        _elapsed = 0f;
    }

    private void Awake()
    {
        _originalColor = txt.color;
        _startPos = transform.localPosition;
    }

    public IEnumerator AnimateUp(string value)
    {
        canvasGroup.alpha = 1;
        txt.text = value;
        _elapsed = 0f;
        
        var duration = fadeDuration;
        var startAlpha = _originalColor.a;
        var t = 0f;
        var startPos = transform.position;
        var endPos = startPos + Vector3.up * speed * duration;
        transform.SetParent(null);
        
        while (t < duration)
        {
            var progress = t / duration;
            transform.position = Vector3.Lerp(startPos, endPos, progress);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);
            yield return null;
            t += Time.deltaTime;
        }

        canvasGroup.alpha = 0;
        txt.color = Color.clear;
    }
}