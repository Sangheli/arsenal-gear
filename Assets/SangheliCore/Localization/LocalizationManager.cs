using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalizationManager
{
    private readonly Dictionary<string, string> _localizedText = new();
    
    // Supported languages
    private readonly string[] _supportedLanguages = { "en", "ru", "tr" };

    public LocalizationManager() => InitializeLocalization();

    private void InitializeLocalization()
    {
        if (!_supportedLanguages.Contains(LocalizationBridge.Lang))
            LocalizationBridge.Lang = "en";

        LoadLocalizedText(LocalizationBridge.Lang);
    }
    
    private void LoadLocalizedText(string language)
    {
        var csvFile = Resources.Load<TextAsset>("Localization/localization_file");
        
        if (csvFile != null)
        {
            ParseCsv(csvFile.text, language);
            return;
        }

        Debug.LogWarning("Localization file not found.");
    }

    private void ParseCsv(string csvText, string language)
    {
        var lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0) return;

        // Parse header to find language column index
        var headers = lines[0].Split(';');
        var languageColumnIndex = Array.IndexOf(headers, language);

        // If language not found, fallback to English
        if (languageColumnIndex == -1) languageColumnIndex = Array.IndexOf(headers, "en");

        // If still not found, default to second column (index 1)
        if (languageColumnIndex == -1) languageColumnIndex = 1;

        // Parse data rows
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(';');
            if (parts.Length <= languageColumnIndex) continue;
            
            var key = parts[0].Trim();
            var value = parts[languageColumnIndex].Replace("\\n", "\n");
            _localizedText[key] = value;
        }
    }
    
    public string GetLocalizedValue(string key) => 
        _localizedText.TryGetValue(key, out var value) ? value : key;
}