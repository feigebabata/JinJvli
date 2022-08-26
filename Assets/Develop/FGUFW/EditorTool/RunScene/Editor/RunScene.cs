using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using FGUFW.Core;

public class RunScene:Editor
{
    [MenuItem("RunScene/Launcher")]
    static void runLauncherScene()
    {
        EditorSceneManager.OpenScene("Assets/Develop/Worlds/Launcher/Launcher.unity");
        EditorApplication.EnterPlaymode();
    }

    [MenuItem("文件夹/程序持续存储文件夹")]
    static void openDir()
    {
        WinPlatform.OpenExplorer(Application.persistentDataPath);
    }
}
