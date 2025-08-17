using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyView : MonoBehaviour
{
    public DeathVfx deathVfx;
    [FormerlySerializedAs("enemyRender")] public EnemyRender render;

    private void OnValidate()
    {
        render ??= GetComponent<EnemyRender>();
    }

    public virtual void SetHitValue(float value) => render.SetHitValue(value);
    public virtual void DoBounceAnimation(bool isCrit) => render.DoBounceAnimation(isCrit);
    public virtual void SetFillValue(float targetClipValue) => render.SetFillValue(targetClipValue);
    public virtual void FlashEffect(bool isCrit = false) => render.FlashEffect(isCrit);
    public virtual void CritColorEffect() => render.CritColorEffect();
    public void PlayDeathVfx() => deathVfx.Play();
    

    public virtual void ResetItem()
    {
        render.ResetItem();

        SetHitValue(0);
        SetFillValue(0);
        
        render.ApplyPropertyView(render.OriginalColor);
        render.ApplyBorderColor();
    }

    public void SetSortingOrder(SortingOrderData data)
    {
        render.SetSortingOrder(data);
        deathVfx.SetOrder(data.ramka);
    }

    public virtual IEnumerator DeathAnimation()
    {
        yield return render.DeathAnimation();
    }
}