using System.Collections;
using System.Collections.Generic;
using FGUFW.Core;
using UnityEngine;

public class ShowUdpTime : MonoBehaviour
{
    public GUIStyle UIStyle;

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label(FrameSyncSystem.Millisecond+"ms",UIStyle);
    }
}
