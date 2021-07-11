using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Play
{
    /// <summary>
    /// 同步陀螺仪旋转姿态
    /// </summary>
    public class GyroRotateCtrl : MonoBehaviour
    {
        public Vector4 Range=new Vector4(0.5f,1,0,1);
        public float Weight = 500;
        private float _offsetY=90;
        private int _touchID=-1;

        // Start is called before the first frame update
        void Awake()
        {
            Input.gyro.enabled = true;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            _offsetY = GetOffsetY(_offsetY);
            rotationUpdate(transform,Input.gyro,_offsetY);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            Input.gyro.enabled = false;
        }

        float GetOffsetY(float offsetY)
        {
            if(_touchID==-1)
            {
                if(Input.touchCount>0)
                {
                    foreach (var touch in Input.touches)
                    {
                        if(touch.phase==TouchPhase.Began && touchInRange(touch,new Vector4(Range.x*Screen.width,Range.y*Screen.width,Range.z*Screen.height,Range.w*Screen.height)))
                        {
                            _touchID = touch.fingerId;
                        }
                    }
                }
            }
            else
            {
                Touch touch = Input.GetTouch(_touchID);
                if(touch.phase==TouchPhase.Moved)
                {
                    offsetY += Input.touches[0].deltaPosition.x/Screen.width*Weight;
                }
                if(touch.phase==TouchPhase.Canceled)
                {
                    _touchID=-1;
                }
            }
            return offsetY;
        }

        bool touchInRange(Touch touch,Vector4 range)
        {
            return touch.position.x>range.x && touch.position.x<range.y && touch.position.y>range.z && touch.position.y>range.w;
        }


        void rotationUpdate(Transform t, Gyroscope gyro,float offsetY)
        {
            var que = new Quaternion(gyro.attitude.x,gyro.attitude.y,-gyro.attitude.z,-gyro.attitude.w);
            que = Quaternion.Euler(90,offsetY,0) * que;
            t.rotation = Quaternion.Lerp(t.rotation,que,0.5f);
        }
    }
}
