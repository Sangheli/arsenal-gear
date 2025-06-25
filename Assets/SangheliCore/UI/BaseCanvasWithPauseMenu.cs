using UnityEngine.UI;

public class BaseCanvasWithPauseMenu : BaseCanvas
{
    public Button buttonClose;
    
    public override void Init()
    {
        if(buttonClose!=null)
            buttonClose.onClick.AddListener(OnPauseMenu);
    }
    
    public static void OnPauseMenu()
    {
        // G.I.LastGameState = G.I.CurrentGameState;
        // G.I.GameStateUpdate.Invoke(GameState.UiPauseMenu);
    }
}
