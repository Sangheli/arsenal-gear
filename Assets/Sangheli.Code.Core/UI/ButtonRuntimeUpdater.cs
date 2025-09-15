using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonRuntimeUpdater : MonoBehaviour
{
    [SerializeField] private HoldButton holdButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private bool updateVisual = true;
    [SerializeField] private bool updateText = true;

    private bool _initDone;
    
    private void Awake()
    {
        // if(G.I !=null)
        //     G.I.OnGameInit += OnInit;
    }

    private void Start()
    {
        OnInit();
    }

    private void OnValidate()
    {
        Init();
    }

    private bool _subs;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        holdButton ??= GetComponent<HoldButton>();
        audioSource ??= GetComponent<AudioSource>();
        button ??= GetComponent<Button>();
        buttonText ??= button.GetComponentInChildren<TMP_Text>();

        if (!_subs && button != null)
        {
            _subs = true;
            button.onClick.AddListener(PlaySound);
        }

        if(audioSource!=null)
            audioSource.playOnAwake = false;
    }

    private void OnInit()
    {
        if(_initDone || G.I == null || !G.I.ActiveForRuntimeButtons)
            return;

        _initDone = true;
        
        Init();
        
        if (holdButton != null)
            holdButton.OnHold += OnHold;
        
        if(updateVisual)
            OnUpdateVisual();
        
        if(updateText  && buttonText!=null)
            UpdateText();
    }

    private void OnUpdateVisual()
    {
        button.image.sprite = G.I.Data.spriteContainer.ButtonSprite;
    }

    private void UpdateText()
    {
        buttonText.font = G.I.Data.fontContainer.fontMain;
        buttonText.color = G.I.Data.fontContainer.fontMainColor;
    }

    private void OnHold(bool isHold)
    {
        if(isHold) PlaySound();
    }
    
    private void PlaySound()
    {
        G.I.Container.pooledAudioPlayer.PlayWithRandomPitch(G.I.Data.soundContainer.buttonClick);
    }
}
