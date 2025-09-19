public class ContinueManagerSimple : IContinueManager
{
    private bool _hasContinued;

    public bool CanContinue(int saveLevel)
    {
        if (saveLevel <= LevelDataConfigRuntime.SkipTutorialLevels)
            return false;
        
        if (!G.I.Data.continueSettings.continueEnabled)
            return false;
            
        if (G.I.Data.continueSettings.oneContinuePerSession && _hasContinued)
            return false;
            
        return true;
    }
    
    public virtual void OnContinue()
    {
        if (!CanContinue(1000000000))
            return;
        
        _hasContinued = true;
        G.I.OnGameContinue();
    }

    public void Reset() => _hasContinued = false;
}