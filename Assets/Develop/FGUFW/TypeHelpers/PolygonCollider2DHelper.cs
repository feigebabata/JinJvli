using UnityEngine;

namespace FGUFW.Core
{
    static public class PolygonCollider2DHelper
    {
        static public void ToEllipse(this PolygonCollider2D self, float width,float height,int pointCount)
        {
            var points = VectorHelper.Ellipse(Vector2.zero,width,height,0,pointCount);
            self.SetPath(0,points);
        }
    }
}