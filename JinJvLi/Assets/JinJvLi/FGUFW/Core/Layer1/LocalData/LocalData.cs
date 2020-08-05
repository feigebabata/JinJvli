using UnityEngine;

namespace FGUFW.Core
{
    static public class LocalData
    {
        static public string LocalPath
        {
            get
            {
                return Application.persistentDataPath;
            }
        }

        static public string LocalReadonlyPath
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }

        static public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        
    }
}