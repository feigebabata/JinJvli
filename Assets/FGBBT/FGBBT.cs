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
       
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        // if(transform.childCount<3)
        // {
        //     return;
        // }
        // Vector3[] nodes = new Vector3[transform.childCount];
        // for (int i = 0; i < transform.childCount; i++)
        // {
        //     nodes[i] = transform.GetChild(i).position;
        // }
        // var points = Vector3Helper.Nodes2BezierCurve(nodes,1);
        
        // Gizmos.color = Color.green;
        // for (int i = 1; i < points.Count; i++)
        // {
        //     Gizmos.DrawLine(points[i],points[i-1]);
        // }
    }
    
}
