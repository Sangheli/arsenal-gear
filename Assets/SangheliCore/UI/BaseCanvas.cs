using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BaseCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    public List<GameState> States;
    public List<ButtonWithStates> buttons;
    
    private void OnValidate()
    {
        _canvasGroup ??= GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        foreach (var x in buttons) x.Init();
    }

    public virtual void Init()
    {
    }
    
    public void Toggle(GameState gameState)
    {
        var show = States.Contains(gameState);

        //TODO remove
        // Debug.Log($"[Canvas][{gameObject.name}] [Show][{show}] [GameState][{gameState}]");
        _canvasGroup.alpha = show ? 1 : 0;
        _canvasGroup.interactable = show;
        _canvasGroup.blocksRaycasts = show;
    }
}
