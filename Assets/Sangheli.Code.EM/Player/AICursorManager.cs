using System.Collections.Generic;
using UnityEngine;

public class AICursorManager : MonoBehaviour
{
    [SerializeField] private CursorController aiCursorPrefab;
    [SerializeField] private Transform cursorParent;
    [SerializeField] private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };

    private readonly List<CursorController> _aiCursors = new();

    public void StartRound()
    {
        var save = G.I.SaveProvider.Get();
        var param = save.GetParamContainer();
        var maxCursors = param.GetMaxLevel("aiCursorCount");
        var count = Mathf.Clamp(Mathf.RoundToInt(param.GetValue("aiCursorCount")), 0, maxCursors);
        var damagePercent = param.GetValue("aiCursorDamagePercent");
        var radiusPercent = param.GetValue("aiCursorRadiusPercent");
        var fireRatePercent = param.GetValue("aiCursorFireRatePercent");

        // Создать недостающие курсоры
        while (_aiCursors.Count < count)
        {
            var aiCursor = Instantiate(aiCursorPrefab, cursorParent);
            aiCursor.cursorInput = new RandomCursorInput();
            aiCursor.gameObject.SetActive(true);
            aiCursor.Toggle(true);
            SetAICursorColor(aiCursor, _aiCursors.Count);
            _aiCursors.Add(aiCursor);
        }
        
        // Отключить лишние
        for (int i = 0; i < _aiCursors.Count; i++) _aiCursors[i].gameObject.SetActive(i < count);
        
        // Применить параметры
        for (int i = 0; i < count; i++)
        {
            var aiCursor = _aiCursors[i];
            aiCursor.SetAIMode(
                damagePercent,
                radiusPercent,
                fireRatePercent
            );
        }
        
        ToggleAll(true);
    }

    private void SetAICursorColor(CursorController cursor, int index)
    {
        var view = cursor.cursorView;
        if (view != null && view.viewRender != null) view.color = colors[Mathf.Clamp(index, 0, colors.Length - 1)];
    }

    public void ToggleAll(bool state)
    {
        foreach (var aiCursor in _aiCursors) aiCursor.Toggle(state);
    }
} 