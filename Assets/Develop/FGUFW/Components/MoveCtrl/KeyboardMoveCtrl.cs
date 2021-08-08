using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Play
{
    public class KeyboardMoveCtrl : MonoBehaviour
    {
        public Action<Vector2> OnMove;
        // Update is called once per frame
        void LateUpdate()
        {
            Vector2 dir = Vector2.zero;
            if(Input.GetKey(KeyCode.W))
            {
                dir+=Vector2.up;
            }
            if(Input.GetKey(KeyCode.S))
            {
                dir+=Vector2.down;
            }
            if(Input.GetKey(KeyCode.A))
            {
                dir+=Vector2.left;
            }
            if(Input.GetKey(KeyCode.D))
            {
                dir+=Vector2.right;
            }
            if(dir!=Vector2.zero)
            {
                OnMove?.Invoke(dir.normalized);
            }
        }
    }

}
