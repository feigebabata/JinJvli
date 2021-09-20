using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    static public class Vector3Helper
    {
        static public Vector3[] BezierCurve(Vector3 p1,Vector3 p2,Vector3 p3,float offset)
        {
            var count = (int)(((p1-p2).magnitude+(p2-p3).magnitude)/offset);
            var points = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                float t = (float)i/count;
                points[i] = bezierCurvePoint(p1,p2,p3,t);
            }
            return points;
        }

        static private Vector3 bezierCurvePoint(Vector3 p1,Vector3 p2,Vector3 p3,float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p1,p2,t) , Vector3.Lerp(p2,p3,t),t);
        }

        static public List<Vector3> Nodes2BezierCurve(Vector3[] nodes,float offset)
        {
            if(nodes==null)
            {
                Debug.LogError("贝塞尔曲线节点数组为空");
                return null;
            }

            if(nodes.Length<3)
            {
                Debug.LogError("贝塞尔曲线节点数小于3");
                return null;
            }

            List<Vector3> ps = new List<Vector3>();
            for (int i_1 = 2; i_1 < nodes.Length; i_1++)
            {
                ps.AddRange(BezierCurve(nodes[i_1-2],nodes[i_1-1],nodes[i_1],offset));
            }

            return ps;
        }

    }
}
