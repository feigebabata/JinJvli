using System;
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
    }
}