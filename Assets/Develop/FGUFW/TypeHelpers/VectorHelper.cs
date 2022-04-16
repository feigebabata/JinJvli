using Unity.Mathematics;
using UnityEngine;

namespace FGUFW.Core
{
    static public class VectorHelper
    {
        /// <summary>
        /// 约等于 ≈
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        static public bool Approximately(Vector3 v1,Vector3 v2)
        {
            return Mathf.Approximately(v1.x,v2.x) && Mathf.Approximately(v1.y,v2.y) && Mathf.Approximately(v1.z,v2.z);
        }

        /// <summary>
        /// 约等于 ≈
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        static public bool Approximately(Vector2 v1,Vector2 v2)
        {
            return Mathf.Approximately(v1.x,v2.x) && Mathf.Approximately(v1.y,v2.y);
        }


        /// <summary>
        /// 加速运动的轨迹
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="startVelocity">初速度</param>
        /// <param name="gravityVelocity">加速度</param>
        /// <param name="t">时刻</param>
        /// <returns></returns>
        static public Vector3 Acceleration(Vector3 startPoint,Vector3 startVelocity,Vector3 gravityVelocity,float t)
        {
            Vector3 offset = Vector3.zero;
            offset.x = getMovingDistance(startVelocity.x,gravityVelocity.x,t);
            offset.z = getMovingDistance(startVelocity.z,gravityVelocity.z,t);
            offset.y = getMovingDistance(startVelocity.y,gravityVelocity.y,t);
            return startPoint+offset;
        }    
        
        /// <summary>
        /// 加速运动位移
        /// </summary>
        /// <param name="startSpeed">初速度</param>
        /// <param name="acceleration">加速度</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        static private float getMovingDistance(float startSpeed,float acceleration,float time)
        {
            return startSpeed*time+acceleration*time*time/2;
        } 

        /// <summary>
        /// 求加速运动 起点到终点的初速度
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <param name="gravityVelocity">加速度</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        static public Vector3 AccelerationStartVelocity(Vector3 startPoint,Vector3 endPoint,Vector3 gravityVelocity,float time)
        {
            Vector3 space = endPoint-startPoint;
            Vector3 startVelocity = Vector3.zero;
            startVelocity.x = getMovingStartVelocity(space.x,gravityVelocity.x,time);
            startVelocity.y = getMovingStartVelocity(space.y,gravityVelocity.y,time);
            startVelocity.z = getMovingStartVelocity(space.z,gravityVelocity.z,time);
            return startVelocity;
        }    
        
        /// <summary>
        /// 获取移动初速度 加速运动的初速度
        /// </summary>
        /// <param name="distance">距离</param>
        /// <param name="acceleration">加速度</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        static private float getMovingStartVelocity(float distance,float acceleration,float time)
        {
            return (distance-acceleration*time*time/2)/time;
        }

        /// <summary>
        /// 点是否在椭圆内 水平方向 无旋转
        /// </summary>
        /// <param name="point"></param>
        /// <param name="center">中心点</param>
        /// <param name="width">椭圆宽</param>
        /// <param name="height">椭圆高</param>
        /// <returns></returns>
        static public bool PointInEllipse(float3 point,float3 center,float width,float height)
        {
            float a = width/2;
            float b = height/2;

            float X = center.x;
            float Y = center.y;
            float x = point.x;
            float y = point.y;

            float cc = ((x-X)*(x-X)) / (a*a) + ((y-Y)*(y-Y)) / (b*b);

            return cc<=1f;
        }
        

        /// <summary>
        /// 椭圆形2D
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width">长半轴</param>
        /// <param name="height">短半轴</param>
        /// <param name="rotation"></param>
        /// <param name="pointCount"></param>
        /// <returns></returns>
        static public Vector2[] Ellipse(Vector2 center, float width, float height, float rotation,int pointCount)
        {
            Vector2[] points = new Vector2[pointCount];
            for (float i = 0; i < pointCount; i++)
            {
                float rate = i/pointCount;
                float angle = 2*Mathf.PI*rate;
                points[(int)i] = getPointOnEllipse(center,width,height,angle,rotation);
            }
            return points;
        }

        /// <summary>
        /// 椭圆边上的点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        static private Vector2 getPointOnEllipse(Vector2 center, float width, float height, float angle, float rotation)
        {
            float dLiXin = Mathf.Atan2(width*Mathf.Sin(angle), height*Mathf.Cos(angle));//离心角
            float x = width*Mathf.Cos(dLiXin)*Mathf.Cos(rotation) - height*Mathf.Sin(dLiXin)*Mathf.Sin(rotation) + center.x;
            float y = width*Mathf.Cos(dLiXin)*Mathf.Sin(rotation) + height*Mathf.Sin(dLiXin)*Mathf.Cos(rotation) + center.y;
            return new Vector2(x, y);
        }

        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        static public float Dot(Vector3 v1,Vector3 v2)
        {
            return (v1.x*v2.x+v1.y*v2.y+v1.z+v2.z)/(Mathf.Pow((v1.x*v1.x+v1.y*v1.y+v1.z+v1.z),0.5f)*Mathf.Pow((v2.x*v2.x+v2.y*v2.y+v2.z+v2.z),0.5f));
        }

        /// <summary>
        /// 向量求模
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static public float Magnitude(Vector3 v)
        {
            return Mathf.Pow(v.x*v.x+v.y*v.y+v.z+v.z,0.5f);
        }

        /// <summary>
        /// 计算AB与CD两条线段的交点.
        /// </summary>
        /// <param name="a">A点</param>
        /// <param name="b">B点</param>
        /// <param name="c">C点</param>
        /// <param name="d">D点</param>
        /// <param name="intersectPos">AB与CD的交点</param>
        /// <returns>是否相交 true:相交 false:未相交</returns>
        static public bool TryGetIntersectPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 intersectPos)
        {
            intersectPos = Vector3.zero;

            Vector3 ab = b - a;
            Vector3 ca = a - c;
            Vector3 cd = d - c;

            Vector3 v1 = Vector3.Cross(ca, cd);

            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return false;
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return false;
            }

            Vector3 ad = d - a;
            Vector3 cb = b - c;
            // 快速排斥
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x)
            || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)
            || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
            )
                return false;

            // 跨立试验
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
                && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v2 = Vector3.Cross(cd, ab);
                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                intersectPos = a + ab * ratio;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 取y轴夹角 12点方向顺时针
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        static public float Angle_Y(Vector3 dir)
        {
            dir = dir.normalized;
            float angle = 0;
            if(dir.x>=0)
            {
                angle = Vector3.Angle(Vector3.up,dir);
            }
            else
            {
                angle = 360 - Vector3.Angle(Vector3.up,dir);
            }
            return angle%360;
        }

        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="l_p1"></param>
        /// <param name="l_p2"></param>
        /// <returns></returns>
        static public float PointLineSpace(Vector3 point, Vector3 l_p1,Vector3 l_p2)
        {
            if(l_p1==l_p2)return Vector3.Distance(l_p1,point);
            
            Vector3 v1 = l_p1 - l_p2;
            Vector3 v2 = point - l_p2;

            //平行四边形面积
            float area = Vector3.Cross(v1,v2).magnitude;

            //高等于面积除底
            float space = area/v1.magnitude;
            return space;
        }

        /// <summary>
        /// 点到线段的距离 点到显得距离不能越过线段的两点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="l_p1"></param>
        /// <param name="l_p2"></param>
        /// <returns></returns>
        static public float PointInLineSpace(Vector3 point, Vector3 l_p1,Vector3 l_p2)
        {
            if(l_p1==l_p2)return Vector3.Distance(l_p1,point);

            Vector3 v1 = l_p1 - l_p2;
            Vector3 v2 = point - l_p2;

            //投影
            Vector3 f = Vector3.Project(v2,v1);
            float space = 0;
            if(f.normalized==v1.normalized)
            {
                if(f.magnitude>v1.magnitude)
                {
                    //点在l_p1侧
                    space = Vector3.Distance(point,l_p1);
                }
                else
                {
                    space = Vector3.Distance(point,l_p2+f);
                }
            }
            else
            {
                //点在l_p2侧
                space = Vector3.Distance(point,l_p2);
            }
            
            return space;
        }


    }
}