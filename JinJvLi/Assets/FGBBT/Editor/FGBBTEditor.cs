using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FGBBT))]
public class FGBBTEditor : Editor
{
    FGBBT fgbbt;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!fgbbt)
        {
            fgbbt = target as FGBBT;
        }
        if (GUILayout.Button("KeyDown"))
        {
            fgbbt.KeyDown();
        }
        if (GUILayout.Button("End"))
        {
            fgbbt.End();
        }
        if (GUILayout.Button("Play"))
        {
            fgbbt.Play();
        }
    }
}
