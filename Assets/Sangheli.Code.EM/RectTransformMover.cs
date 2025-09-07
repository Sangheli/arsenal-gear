using UnityEngine;

public class RectTransformMover : MonoBehaviour
{
    [SerializeField] private RectTransform[] targetArr;        // кого двигаем
    [SerializeField] private Vector2 minAnchorOffset;     // смещение внутрь от границ контейнера (в UI единицах)
    [SerializeField] private Vector2 maxAnchorOffset;     // смещение внутрь от границ контейнера (в UI единицах)

    public float factor = 2;
    private RectTransform _parentRect;
    private Vector2? _halfSize;
    
    private void Awake()
    {
        if (targetArr[0] != null)
            _parentRect = targetArr[0].parent as RectTransform;
    }

    private void MoveToWorldPosition(Vector3 worldPos)
    {
        if (targetArr[0] == null || _parentRect == null) return;
        _halfSize ??= _parentRect.rect.size * 0.5f;
        
        var localPos = _parentRect.InverseTransformPoint(worldPos);
        var anchoredPos = (Vector2)localPos + _halfSize.Value;
        anchoredPos *= factor;

        // Clamp
        anchoredPos.x = Mathf.Clamp(anchoredPos.x, minAnchorOffset.x, maxAnchorOffset.x);
        anchoredPos.y = Mathf.Clamp(anchoredPos.y, minAnchorOffset.y, maxAnchorOffset.y);

        foreach (var x in targetArr) x.anchoredPosition = anchoredPos;
    }
    
    private void LateUpdate()
    {
        MoveToWorldPosition(G.I.playerCursor.transform.position);
    }
}