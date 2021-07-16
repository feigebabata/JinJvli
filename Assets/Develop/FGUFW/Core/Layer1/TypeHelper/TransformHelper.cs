using System;
using System.Collections;
using UnityEngine;

namespace FGUFW.Core
{
    static public class TransformHelper
    {
        static public RectTransform AsRT(this Transform t)
        {
            return t as RectTransform;
        }
        static public RectTransform GetChildRT(this Transform t,int index)
        {
            return t.GetChild(index) as RectTransform;
        }

        static public IEnumerator MoveWorld(this Transform transform,Vector3 endPos,float time)
        {
            float startTime = Time.time;
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.position = Vector3.Lerp(transform.position,endPos,t);
                yield return null;
            }
            transform.position = endPos;
        }

        static public IEnumerator MoveLocal(this Transform transform,Vector3 endPos,float time)
        {
            float startTime = Time.time;
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.localPosition = Vector3.Lerp(transform.localPosition,endPos,t);
                yield return null;
            }
            transform.localPosition = endPos;
        }

        static public IEnumerator RotateLocal(this Transform transform,Vector3 endAngle,float time)
        {
            float startTime = Time.time;
            Quaternion rotation = Quaternion.Euler(endAngle);
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.localRotation = Quaternion.Lerp(transform.localRotation,rotation,t);
                yield return null;
            }
            transform.localRotation = rotation;
        }

        static public IEnumerator RotateWorld(this Transform transform,Vector3 endAngle,float time)
        {
            float startTime = Time.time;
            Quaternion rotation = Quaternion.Euler(endAngle);
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.rotation = Quaternion.Lerp(transform.rotation,rotation,t);
                yield return null;
            }
            transform.rotation = rotation;
        }
    }
}