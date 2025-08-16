using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SangheliCore
{
    public class LocalizationManager
    {
        private readonly Dictionary<string, string> _localizedText = new();
    
        private static readonly List<string> LangFallbackRu = new() { "be", "kk", "uk", "uz", };
        private static readonly List<string> LangFallbackEs = new() { "gl", "ca", "eu" };

        public LocalizationManager(string lang) => InitializeLocalization(lang);

        private void InitializeLocalization(string lang) => LoadLocalizedText(GetLang(lang));
        private const char Splitter = ',';

        private static string GetLang(string lang)
        {
            if (LangFallbackRu.Contains(lang))
                lang = "ru";
            
            if (LangFallbackEs.Contains(lang))
                lang = "es";

            return lang;
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

            var languageColumnIndex = GetLanguageColumnIndex(language, lines);

            // Parse data rows
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = SplitCsvLine(line);
                if (parts.Length <= languageColumnIndex) continue;
            
                var key = parts[0].Trim();
                var value = parts[languageColumnIndex].Replace("\\n", "\n").Replace("\"", "");
                _localizedText[key] = value;
            }
        }
    
        private static int GetLanguageColumnIndex(string language, string[] lines)
        {
            // Parse header to find language column index
            var headers = lines[0].Split(Splitter);
            var languageColumnIndex = Array.IndexOf(headers, language);

            // If language not found, fallback to English
            if (languageColumnIndex == -1) languageColumnIndex = Array.IndexOf(headers, "en");

            // If still not found, default to second column (index 1)
            if (languageColumnIndex == -1) languageColumnIndex = 1;
            return languageColumnIndex;
        }

        private static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                
                switch (c)
                {
                    case '"' when i == 0 || line[i - 1] != '\\':
                        inQuotes = !inQuotes;
                        break;
                    case ',' when !inQuotes:
                        result.Add(current);
                        current = "";
                        break;
                    default:
                        current += c;
                        break;
                }
            }

            result.Add(current);
            return result.ToArray();
        }

        public string Get(string key) => 
            _localizedText.TryGetValue(key, out var value) ? value : key;
    }
}