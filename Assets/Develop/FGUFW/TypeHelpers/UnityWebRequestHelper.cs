using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FGUFW.Core
{
    static public class UnityWebRequestHelper
    {
        static public IEnumerator StreamingCopy(string originPath,string savePath,Action<string> complete)
        {
            var url = new Uri(Path.Combine(Application.streamingAssetsPath,originPath));
            UnityWebRequest uwr = new UnityWebRequest(url);
            uwr.downloadHandler = new DownloadHandlerFile(savePath);
            yield return uwr.SendWebRequest();
            complete?.Invoke(uwr.error);
        }
    }
}