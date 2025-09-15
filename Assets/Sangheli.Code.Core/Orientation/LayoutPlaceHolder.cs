using UnityEngine;

public class LayoutPlaceHolder : MonoBehaviour
{
    public GameObject errorMessage;
    
    public void OnOrientationChanged(bool isHorizontal)
    {
        errorMessage.gameObject.SetActive(!isHorizontal);
    }
}