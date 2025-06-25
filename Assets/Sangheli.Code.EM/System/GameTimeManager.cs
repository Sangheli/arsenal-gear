using UnityEngine;

public class GameTimeManager
{
    private float _shiftTime;
    private float _minusShift;
    private bool _isTimeActive;
    
    public void Init()
    {
        _shiftTime = 0f;
        _minusShift = -Time.time;
        _isTimeActive = true;
    }
    
    public void Restore(float savedTime)
    {
        _shiftTime = savedTime;
        _minusShift = -Time.time;
        _isTimeActive = true;
    }
    
    public float GetCurrentTime() => 
        _isTimeActive ? _shiftTime + _minusShift + Time.time : 0f;
} 