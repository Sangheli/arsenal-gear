using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class DamageTextManager : MonoBehaviour
{
    public Text damageText;
    public float textAnimationTime = 0.3f;
    public Color textStartColor = Color.red;
    public Color textEndColor = Color.white;
    
    private Vector3 _originalScaleText;
    private float _accumulatedDamage;
    private float _accumulatedShieldDamage;
    private bool _hasDamageThisFrame;
    
    private void Awake()
    {
        var transform1 = damageText.transform;
        
        _originalScaleText = transform1.localScale;
        transform1.localScale = Vector3.zero;
    }

    public void AddDamage(float damage, float shieldDamage = 0f)
    {
        _accumulatedDamage += damage;
        _accumulatedShieldDamage += shieldDamage;
        _hasDamageThisFrame = true;
    }
    
    public void LateUpdate()
    {
        if (!_hasDamageThisFrame) 
            return;
        
        ShowDamageText(_accumulatedDamage, _accumulatedShieldDamage);
        _hasDamageThisFrame = false;
        _accumulatedDamage = 0f;
        _accumulatedShieldDamage = 0f;
    }
    
    private void ShowDamageText(float totalDamage, float shieldDamage)
    {
        if(totalDamage<=0 && shieldDamage<=0)
            return;
        var sb = new StringBuilder();
        if (totalDamage > 0)
            sb.Append($"-{NumberFormatter.Format(totalDamage, 0)}HP");
        if (shieldDamage > 0)
        {
            if (sb.Length > 0) sb.Append(" ");
            sb.Append($"(-{NumberFormatter.Format(shieldDamage, 0)} SHIELD)");
        }
        damageText.text = sb.ToString();
        StartCoroutine(GameAnimation.AnimateDamageText(damageText, _originalScaleText, textAnimationTime, textStartColor, textEndColor));
    }
} 