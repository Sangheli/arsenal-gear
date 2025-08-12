using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using TMPro;
using System.Collections;

public class SaveRow : MonoBehaviour
{
    public Button buttonSave;
    public Button buttonLoad;
    public TMP_Text label;             // Label to display date/time text
    public Text label2;             // Label to display date/time text

    public int _saveSlot = 0;           // Save slot number or ID
    private IDataSaveProvider saveProvider => G.I.SaveProvider;
    private const string SaveKeyPrefix = "SaveSlot_";

    private void Start()
    {
        // _saveSlot = transform.GetSiblingIndex();
        if(buttonSave!=null) buttonSave.onClick.AddListener(Save);
        if(buttonSave!=null) UpdateLabel();
        
        if(buttonLoad!=null) buttonLoad.onClick.AddListener(Load);
        if(buttonLoad!=null) UpdateLabelLoad();
    }

    private void Save()
    {
        var sendAnalytics = true; 
        PlayerPrefs.SetString(SaveKeyPrefix + _saveSlot, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        saveProvider.Save(_saveSlot, sendAnalytics: sendAnalytics, endRoundType: EndRoundFadeEnum.None); // your actual save logic here
        UpdateLabel();
    }

    private void Load()
    {
        var key = SaveKeyPrefix + _saveSlot;
        var datetime = PlayerPrefs.GetString(key, null);
        if(datetime==null)
            return;
        
        saveProvider.ApplyFromSave(_saveSlot);
        // G.I.RestoreGameTime();
        // G.I.Container.moneyView.Init();

        StartCoroutine(LoadWithDelay());
    }

    private IEnumerator LoadWithDelay()
    {
        yield return new WaitForSeconds(0.25f);
        G.I.Container.cameraZoom.CanZoomManual = true;
    }

    private void UpdateLabel()
    {
        var key = SaveKeyPrefix + _saveSlot;
        var datetime = PlayerPrefs.GetString(key, null);

        if (string.IsNullOrEmpty(datetime))
        {
            if (label != null) label.text = $"Save game";
            if (label2 != null) label2.text = $"Save game";
        }
        else
        {
            if (label != null) label.text = $"Saved\n{datetime}";
            if (label2 != null) label2.text = $"Saved\n{datetime}";    
        }
    }

    public void UpdateLabelLoad()
    {
        var key = SaveKeyPrefix + _saveSlot;
        var datetime = PlayerPrefs.GetString(key, null);
        gameObject.SetActive(!string.IsNullOrEmpty(datetime));
        
        if (string.IsNullOrEmpty(datetime))
        {
            if (label != null) label.text = $"Save not found";
            if (label2 != null) label2.text = $"Save not found";    
        }
        else
        {
            if (label != null) label.text = $"Load\n{datetime}";
            if (label2 != null) label2.text = $"Load\n{datetime}";    
        }
    }
}