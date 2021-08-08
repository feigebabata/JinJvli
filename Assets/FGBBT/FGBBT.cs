using System;
using System.Collections;
using System.Collections.Generic;
using FGUFW.Core;
using UnityEngine;

public class FGBBT : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalAssetSystem.I.ToString();

        var loader = new PrefabLoadHandel("GamePlay.StepGrid.DefaultModule.StopPanel");
        loader.Completed+=Completed;
        GlobalMessenger.M.Broadcast(GlobalMsgID.LoadPrefab,loader);
    }

    private void Completed(UnityEngine.Object obj)
    {
        Debug.Log(obj.name);
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        GlobalAssetSystem.I.Dispose();
    }
    
}
