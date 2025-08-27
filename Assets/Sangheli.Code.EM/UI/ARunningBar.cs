using UnityEngine;

public abstract class ARunningBar : MonoBehaviour
{
    protected RunningBarValue RunningBarValue;
    
    public void Set(RunningBarValue runningBarValue)
    {
        RunningBarValue = runningBarValue;
    }
    
    public abstract void UpdateView();
    public abstract void SetCurrentValue(float value);
    public abstract void SetSortingOrder(int order);
}