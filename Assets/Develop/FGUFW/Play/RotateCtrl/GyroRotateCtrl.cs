using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Play
{
    public class GyroRotateCtrl : MonoBehaviour
    {
        public float Weight = 500;
        private float offsetY=90;
        private Vector3 oldPos;

        // Start is called before the first frame update
        void Awake()
        {
            Input.gyro.enabled = true;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            // if(Input.GetMouseButtonDown(0))
            // {
            //     oldPos = Input.mousePosition;
            // }
            if(Input.touchCount>0)
            {
                // Vector3 scroll = Input.mousePosition - oldPos;
                offsetY += Input.touches[0].deltaPosition.x/Screen.width*Weight;
                oldPos = Input.mousePosition;
            }
            Debug.Log(offsetY);
            var que = new Quaternion(Input.gyro.attitude.x,Input.gyro.attitude.y,-Input.gyro.attitude.z,-Input.gyro.attitude.w);
            que = Quaternion.Euler(90,offsetY,0) * que;
            transform.rotation = Quaternion.Lerp(transform.rotation,que,0.5f);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            Input.gyro.enabled = false;
        }
    }
}
