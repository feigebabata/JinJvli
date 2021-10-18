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

    }
}