using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyView : MonoBehaviour
{
    public HalfBarView hpBarView;
    public DeathVfx deathVfx;
    public DeathVfx hitVfx;
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
    public void PlayHitVfx() => hitVfx.Play();
    

    public virtual void ResetItem()
    {
        render.ResetItem();
        render.ResetCoroutines();

        SetHitValue(0);
        SetFillValue(0);
        
        render.ApplyPropertyView(render.OriginalColor);
        render.ApplyBorderColor();
    }

    public void SetSortingOrder(SortingOrderData data)
    {
        var baseOrder = 1000 - (int) transform.position.z;
        baseOrder *= 10;
        
        data.ramka = baseOrder+2;
        data.view = baseOrder+1;
        data.back = baseOrder;

        render.SetSortingOrder(data);
        deathVfx.SetOrder(data.ramka+1);
        hitVfx.SetOrder(data.ramka+1);
        hpBarView.runningBarImage.SetSortingOrder(data.back);
    }

    public virtual IEnumerator DeathAnimation()
    {
        yield return render.DeathAnimation();
    }
}