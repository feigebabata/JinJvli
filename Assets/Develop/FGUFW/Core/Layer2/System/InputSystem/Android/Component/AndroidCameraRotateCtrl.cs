using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class AndroidCameraRotateCtrl : MonoBehaviour
    {
        public AndroidPlayerInput PlayerInput;

        // Start is called before the first frame update
        void Start()
        {
            Input.gyro.enabled = true;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var que = new Quaternion(Input.gyro.attitude.x,Input.gyro.attitude.y,-Input.gyro.attitude.z,-Input.gyro.attitude.w);
            que = Quaternion.Euler(90,PlayerInput==null?0:PlayerInput.CameraRotateY,0) * que;
            transform.rotation = Quaternion.Lerp(transform.rotation,que,0.5f);
        }
    }
}
