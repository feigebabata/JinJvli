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
        static public MultiLanguangeFont MultiLanguangeFonts;

        static public void InitConfig()
        {
            var loader = Addressables.LoadAssetAsync<TextAsset>("FGUFW/MultiLanguageConfig");
            var textAsset = loader.WaitForCompletion();
            languageConfig = textAsset.text.ToCsvDict();
            MultiLanguangeFonts = Addressables.LoadAssetAsync<MultiLanguangeFont>("Assets/Develop/FGUFW/MultiLanguage/MultiLanguangeFonts.asset").WaitForCompletion();
        }

        static public Font GetMultiLanguangeFont()
        {
            if(MultiLanguangeFonts==null)
            {
                #if UNITY_EDITOR
                    MultiLanguangeFonts = UnityEditor.AssetDatabase.LoadAssetAtPath<MultiLanguangeFont>("Assets/Develop/FGUFW/MultiLanguage/MultiLanguangeFonts.asset");
                #endif
            }
            return MultiLanguangeFonts.Fonts[LanguageIndex];
        }

        static public IReadOnlyList<string> GetLanguageNames()
        {
            return languageConfig["*"];
        }

        static public string GetLanguageText(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Debug.LogError($"[GetLanguageText]出错.id:{id}");
                return string.Empty;
            }
            string text = null;
            if(languageConfig.ContainsKey(id) && languageConfig[id].Count>LanguageIndex+1)
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
            // Debug.Log($"语言切换:{index}:{GetLanguageNames()[index+1]}");
            LanguageIndex = index;
            #if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlaying)
            {
                Worlds.GameMap.GameRecordDatas.RecordDatas.LanguageIndex = index;
                Worlds.GameMap.GameRecordDatas.Save();
                OnLanguageChanged?.Invoke();
            }
            #else
            Worlds.GameMap.GameRecordDatas.RecordDatas.LanguageIndex = index;
            Worlds.GameMap.GameRecordDatas.Save();
            OnLanguageChanged?.Invoke();
            #endif
        }

        /// <summary>
        /// 转当前语言
        /// </summary>
        /// <returns></returns>
        static public string ToCL(this string text)
        {
            return GetLanguageText(text);
        }
    }
}