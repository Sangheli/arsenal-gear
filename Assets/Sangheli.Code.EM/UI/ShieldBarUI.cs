public class ShieldBarUI : HealthBarUI
{
    public void ToggleShieldView(bool available)
    {
        gameObject.SetActive(available);

        if (!available) 
            return;
        
        foreach (var x in halfBarViewArr) 
            x.Show(1, false, useStartValue: true);
    }
} 