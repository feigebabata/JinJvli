using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 namespace FGUFW.Play
 {
     
    [RequireComponent(typeof(Camera))]
    /// <summary>
    /// 使相机渲染宽不变
    /// </summary>
    public class CameraHorizontalFixed : MonoBehaviour
    {
        [Header("视锥比例")]
        public Vector2 ViewingCone;
    
        [Header("垂直距离")]
        public float Distance;
    
        void Start()
        {
            #if UNITY_EDITOR
                ViewingCone = new Vector2(Screen.width,Screen.height);
            #endif
            Camera camera = GetComponent<Camera>();
            setCameraPos(ViewingCone,new Vector2(Screen.width,Screen.height),camera.fieldOfView,Distance,transform.eulerAngles.x);
        }
    
        /// <summary>
        /// 修改相机位置
        /// </summary>
        /// <param name="oldViewingCone">参考视锥大小</param>
        /// <param name="newViewingCone">当前视锥大小</param>
        /// <param name="viewingConeAngle">视锥纵向夹角</param>
        /// <param name="viewingConeHeight">视锥与对象高度差</param>
        /// <param name="viewingConeEulerX">视锥X轴旋转</param>
        void setCameraPos(Vector2 oldViewingCone,Vector2 newViewingCone,float viewingConeAngle,float viewingConeHeight,float viewingConeEulerX)
        {
            float oldAngle = viewingConeHorizontalAngle(oldViewingCone,viewingConeAngle);
            float newAngle = viewingConeHorizontalAngle(newViewingCone,viewingConeAngle);
    
            #region 求斜边
            float tempAngle = (180-viewingConeEulerX-90)-viewingConeAngle/2;
            float hypotenuse = viewingConeHeight/Mathf.Cos(tempAngle*Mathf.Deg2Rad);
            #endregion
    
            float width = Mathf.Sin(oldAngle/2*Mathf.Deg2Rad)*hypotenuse;
            float height = Mathf.Cos(oldAngle/2*Mathf.Deg2Rad)*hypotenuse;
    
            float h2 = width/Mathf.Tan(newAngle/2*Mathf.Deg2Rad);
    
            float length = height - h2;
    
            Vector3 forward = transform.forward;
            forward = Quaternion.Euler(viewingConeAngle/2,0,0)*forward;
            
            Vector3 pos = transform.position+forward.normalized*length;
            transform.position = pos;
            // Debug.Log(pos);
        }
    
        /// <summary>
        /// 视锥大小
        /// </summary>
        /// <param name="viewingCone"></param>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        Vector2 viewingConeSize(Vector2 viewingCone,float angle,float distance)
        {
            float tan = Mathf.Tan(Mathf.Deg2Rad*angle/2);
            Vector2 size = Vector2.zero;
            size.y = tan*distance*2;
            size.x = viewingCone.x*size.y/viewingCone.y;
            return size;    
        }
    
        /// <summary>
        /// 视锥横向夹角
        /// </summary>
        /// <param name="viewingCone"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        float viewingConeHorizontalAngle(Vector2 viewingCone,float angle)
        {
            float h_angle=0;
            float distance=1;
            Vector2 size = viewingConeSize(viewingCone,angle,distance);
            h_angle = Mathf.Atan(size.x/2/distance)*Mathf.Rad2Deg*2;
            return h_angle;
        }
    }
 }