using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

public class RunScene:Editor
{
    [MenuItem("RunScene/Launcher")]
    static void runLauncherScene()
    {
        EditorSceneManager.OpenScene("Assets/Develop/GamePlay/Launcher/Launcher.unity");
        EditorApplication.EnterPlaymode();
    }
}
