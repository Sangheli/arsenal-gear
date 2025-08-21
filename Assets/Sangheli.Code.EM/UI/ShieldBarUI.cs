public class ShieldBarUI : HealthBarUI
{
    public void ToggleShieldView(bool available) => gameObject.SetActive(available);
} 