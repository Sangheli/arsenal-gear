using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DamageTextManager : MonoBehaviour
{
    public TextUpAnimated prefabText;
    private bool _hasDamageThisFrame;

    private readonly List<TextUpAnimated> _spawned = new();
    private readonly Dictionary<TextUpAnimated, Coroutine> _activeCoroutines = new();

    public void AddDamage(DamageResult damageResult, float CurrentDamageScale) => 
        StartCoroutine(ShowDamageText(damageResult, CurrentDamageScale));

    private IEnumerator ShowDamageText(DamageResult damageResult, float CurrentDamageScale)
    {
        if(damageResult.TotalDamage<=0 && damageResult.TotalShieldDamage<=0)
            yield break;
        
        var sb = new StringBuilder();
        
        if (damageResult.TotalDamage > 0)
            sb.Append($"<color=#FF2222>-{NumberFormatter.Format(damageResult.TotalDamage, 0)}HP</color>");
        
        if (damageResult.TotalShieldDamage > 0)
        {
            if (sb.Length > 0) sb.Append("\n");
            sb.Append($"<color=#22FF22>-{NumberFormatter.Format(damageResult.TotalShieldDamage, 0)}S</color>");
        }
        
        var textObj = G.I.ObjectPool.Get(prefabText);
        _spawned.Add(textObj);
        var transform1 = textObj.transform;
        transform1.position = GetPos(CurrentDamageScale);
        _activeCoroutines[textObj] = StartCoroutine(AnimateAndReturn(textObj, sb.ToString()));
    }

    private static Vector3 GetPos(float CurrentDamageScale)
    {
        var pos = G.I.playerCursor.view.transform.position;
        var shift = 0.7f + CurrentDamageScale / 2f;
        pos.x -= shift;
        pos.z = 0;
        return pos;
    }

    private IEnumerator AnimateAndReturn(TextUpAnimated textObj, string text)
    {
        yield return textObj.AnimateUp(text);
        G.I.ObjectPool.Return(textObj);
        _spawned.Remove(textObj);
        _activeCoroutines.Remove(textObj);
    }

    public void StartRound()
    {
        foreach (var x in _spawned)
        {
            if (_activeCoroutines.TryGetValue(x, out var coroutine)) StopCoroutine(coroutine);
            G.I.ObjectPool.Return(x);
        }
        
        _spawned.Clear();
        _activeCoroutines.Clear();
    }
} 