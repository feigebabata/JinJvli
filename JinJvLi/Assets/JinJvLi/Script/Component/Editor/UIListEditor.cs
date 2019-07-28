using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIList))]
public class UIListEditor : Editor
{
    UIList m_uiList;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("ResetUIList"))
        {
            if(m_uiList==null)
            {
                m_uiList = target as UIList;
            }
            m_uiList.ResetUIList();
        }
        if(GUILayout.Button("Clear"))
        {
            if(m_uiList==null)
            {
                m_uiList = target as UIList;
            }
            m_uiList.Clear();
        }
    }
}
