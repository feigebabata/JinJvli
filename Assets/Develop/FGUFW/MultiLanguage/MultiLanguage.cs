using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FGUFW.Core
{
    
    static public class MultiLanguage
    {
        
        static private IReadOnlyDictionary<string, IReadOnlyList<string>> languageConfig;
        
        static public int LanguageIndex{get; private set;}
        static public Action OnLanguageChanged;

        static public async Task<TextAsset> InitConfig()
        {
            var task = Addressables.LoadAssetAsync<TextAsset>("FGUFW/MultiLanguageConfig").Task;
            var textAsset = await task;
            languageConfig = textAsset.text.ToCsvDict();
            return await task;
        }

        static public IReadOnlyList<string> GetLanguageNames()
        {
            return languageConfig["*"];
        }

        static public string GetLanguageText(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError($"[GetLanguageText]出错.id:{id}");
                return "";
            }
            string text = null;
            if(languageConfig.ContainsKey(id))
            {
                text = languageConfig[id][LanguageIndex+1];
            }
            else
            {
                text = id;
                // Debug.LogWarning("多语言无TextID:"+text);
            }
            return text;
        }

        static public void SetLanguage(int index)
        {
            LanguageIndex = index;
            Worlds.GameMap.GameRecordDatas.RecordDatas.LanguageIndex = index;
            Worlds.GameMap.GameRecordDatas.Save();
            OnLanguageChanged?.Invoke();
        }
    }
}