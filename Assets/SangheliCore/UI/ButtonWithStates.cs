using System;
using System.Collections.Generic;
using UnityEngine.UI;

[Serializable]
public class ButtonWithStates
{
    public Button Button;
    public List<GameState> GameStateList;

    public void Init()
    {
        Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // foreach (var x in GameStateList) 
        //     G.I.GameStateUpdate.Invoke(x);
    }
}
