using UnityEngine;
using UnityEngine.UI;

public class TextAnim : MonoBehaviour
{
    public Text text;
    public bool playBounce;
    public BounceAnim bounceAnim;

    public void ResetView()
    {
        transform.localScale = Vector3.one;
        bounceAnim.ResetView();
    }

    public void SetText(int value, int baseValue)
    {
        text.text = baseValue switch
        {
            < 1000 => $"{value:D3}/{baseValue:D3}",
            < 10000 => $"{value:D4}/{baseValue:D4}",
            < 100000 => $"{value:D5}/{baseValue:D5}",
            _ => $"{value:D6}/{baseValue:D6}"
        };

        if (playBounce) bounceAnim.Play();
    }

    public void SetText(char value)
    {
        text.text = value.ToString();
        if (playBounce) bounceAnim.Play();
    }
}
