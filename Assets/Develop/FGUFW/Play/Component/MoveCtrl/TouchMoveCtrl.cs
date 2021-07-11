using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Play
{
    public class TouchMoveCtrl : MonoBehaviour
    {
        public Vector4 Range=new Vector4(0,0.5f,0,1);
        public Action<Vector2> OnMove;
        private int _touchID=-1;

        // Update is called once per frame
        void LateUpdate()
        {
            if(Input.touchCount>0)
            {
                if(_touchID==-1)
                {
                    foreach (var touch in Input.touches)
                    {
                        if(touch.phase==TouchPhase.Began && touchInRange(touch,new Vector4(Range.x*Screen.width,Range.y*Screen.width,Range.z*Screen.height,Range.w*Screen.height)))
                        {
                            _touchID = touch.fingerId;
                        }
                    }
                }
                else
                {
                    if(!Array.Exists<Touch>(Input.touches,touchMatch))
                    {
                        _touchID=-1;
                        return;
                    }
                    Touch touch = Input.GetTouch(_touchID);
                    if(touch.phase==TouchPhase.Moved || touch.phase==TouchPhase.Stationary)
                    {
                        var dir = touch.position-touch.rawPosition;
                        OnMove?.Invoke(dir.normalized);
                    }
                }
            }
            else
            {
                _touchID=-1;
            }
        }

        private bool touchMatch(Touch obj)
        {
            return obj.fingerId == _touchID;
        }

        bool touchInRange(Touch touch,Vector4 range)
        {
            return touch.position.x>range.x && touch.position.x<range.y && touch.position.y>range.z && touch.position.y<range.w;
        }


    }

}
