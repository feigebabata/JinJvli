using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace FGUFW.Core
{
    static public class ConfigDatabase
    {
        static public string GetConfig(string key,string defaultValue="")
        {
            return PlayerPrefs.GetString(key,defaultValue);
        }

        static public int GetConfig(string key,int defaultValue=0)
        {
            return PlayerPrefs.GetInt(key,defaultValue);
        }

        static public float GetConfig(string key,float defaultValue=0)
        {
            return PlayerPrefs.GetFloat(key,defaultValue);
        }

        static public void SetConfig(string key,string value)
        {
            PlayerPrefs.SetString(key,value);
        }

        static public void SetConfig(string key,int value)
        {
            PlayerPrefs.SetInt(key,value);
        }

        static public void SetConfig(string key,float value)
        {
            PlayerPrefs.SetFloat(key,value);
        }

        /// <summary>
        /// 获取配置文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback">返回值为json</param>
        static public void GetWebConfig(string url,Action<string> callback)
        {
            getWebConfig(url,callback).StartIO();
        }

        static IEnumerator getWebConfig(string url,Action<string> callback)
        {
            UnityWebRequest uwr = new UnityWebRequest(new Uri(url));
            uwr.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            if(uwr.result == UnityWebRequest.Result.Success)
            {
                callback(uwr.downloadHandler.text);
            }
            else
            {
                callback(null);
            }
        }

    }
}