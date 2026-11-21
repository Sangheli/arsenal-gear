using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Text;

public class DamageTextManager : MonoBehaviour
{
    public TextUpAnimated prefabText;
    private bool _hasDamageThisFrame;

    private readonly List<TextUpAnimated> _spawned = new();
    private readonly Dictionary<TextUpAnimated, CancellationTokenSource> _activeAnimations = new();

    public void AddDamage(DamageResult damageResult, float CurrentDamageScale) => 
        ShowDamageText(damageResult, CurrentDamageScale);

    private void ShowDamageText(DamageResult damageResult, float CurrentDamageScale)
    {
        if(damageResult.TotalDamage<=0 && damageResult.TotalShieldDamage<=0)
            return;
        
        var sb = new StringBuilder();
        
        if (damageResult.TotalDamage > 0)
            sb.Append($"<color=#FF2222>-{NumberFormatter.Format(damageResult.TotalDamage, 2)}{G.I.localization.Get("hp_short")}</color>");
        
        if (damageResult.TotalShieldDamage > 0)
        {
            if (sb.Length > 0) sb.Append("\n");
            sb.Append($"<color=#22FF22>-{NumberFormatter.Format(damageResult.TotalShieldDamage, 2)}{G.I.localization.Get("shield_short")}</color>");
        }
        
        var textObj = G.I.ObjectPool.Get(prefabText);
        _spawned.Add(textObj);
        var transform1 = textObj.transform;
        transform1.position = GetPos(CurrentDamageScale);
        var cancellation = new CancellationTokenSource();
        _activeAnimations[textObj] = cancellation;
        _ = AnimateAndReturn(textObj, sb.ToString(), cancellation.Token).LogExceptions();
    }

    private static Vector3 GetPos(float CurrentDamageScale)
    {
        var pos = G.I.playerCursor.view.transform.position;
        var shift = 0.7f + CurrentDamageScale / 2f;
        pos.x -= shift;
        pos.z = 0;
        return pos;
    }

    private async UniTask AnimateAndReturn(TextUpAnimated textObj, string text, CancellationToken cancellationToken)
    {
        await textObj.AnimateUp(text, cancellationToken);
        G.I.ObjectPool.Return(textObj);
        _spawned.Remove(textObj);
        if (_activeAnimations.TryGetValue(textObj, out var cancellation))
        {
            _activeAnimations.Remove(textObj);
            cancellation.Dispose();
        }
    }

    public void StartRound()
    {
        foreach (var x in _spawned)
        {
            if (_activeAnimations.TryGetValue(x, out var cancellation))
            {
                cancellation.Cancel();
                cancellation.Dispose();
            }
            G.I.ObjectPool.Return(x);
        }
        
        _spawned.Clear();
        _activeAnimations.Clear();
    }

    private void OnDestroy()
    {
        foreach (var cancellation in _activeAnimations.Values)
        {
            cancellation.Cancel();
            cancellation.Dispose();
        }
        _activeAnimations.Clear();
    }
} 