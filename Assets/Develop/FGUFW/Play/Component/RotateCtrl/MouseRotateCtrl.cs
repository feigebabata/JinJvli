using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Play
{
    public class MouseRotateCtrl : MonoBehaviour
    {
        public float Weight=5,Speed=0.5f;
        private float _mouseX,_mouseY;


        // Update is called once per frame
        void LateUpdate()
        {
            _mouseX += Input.GetAxis("Mouse X");
            _mouseY += Input.GetAxis("Mouse Y");

            float x = _mouseX*Speed;
            float y = _mouseY*Speed;
            _mouseX -= x;
            _mouseY -= y;

            transform.Rotate(Vector3.up*x*Weight,Space.World);
            transform.Rotate(Vector3.left*y*Weight,Space.Self);
        }
    }
}
