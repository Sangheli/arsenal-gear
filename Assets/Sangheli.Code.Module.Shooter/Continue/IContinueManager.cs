public interface IContinueManager
{
    public bool CanContinue(int saveLevel);
    public void OnContinue();
    public void Reset();
}