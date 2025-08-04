using System;
using UnityEngine;

public class OrientationChecker : MonoBehaviour
{
    public event Action<bool> UpdateOrientation;
    private ScreenOrientation _lastOrientation;
    private bool IsHorizontal { get; set; }

    private void Awake()
    {
        _lastOrientation = Screen.orientation;
        UpdateFlag(_lastOrientation);
    }

    private void Update()
    {
        if (Screen.orientation == _lastOrientation) 
            return;
        
        _lastOrientation = Screen.orientation;
        UpdateFlag(_lastOrientation);
    }

    private void UpdateFlag(ScreenOrientation orientation)
    {
        IsHorizontal = orientation is ScreenOrientation.LandscapeLeft or ScreenOrientation.LandscapeRight;
        UpdateOrientation?.Invoke(IsHorizontal);
    }
}