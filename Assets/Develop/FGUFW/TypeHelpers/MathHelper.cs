using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FGUFW.Core
{
    static public class MathHelper
    {
        /// <summary>
        /// 区间索引
        /// </summary>
        /// <param name="count">区间分段</param>
        /// <param name="length">区间长</param>
        /// <param name="index">位置</param>
        /// <returns></returns>
        static public int IndexOf(int count ,float length,float index)
        {
            float t = count*index/length;
            int idx = (int)t;
            idx = Mathf.Clamp(idx,0,count-1);
            return idx;
        }

        static public int SortInt(float d)
        {
            if(d>0)
            {
                return 1;
            }
            else if(d<0)
            {
                return -1;
            }
            return 0;
        }

        static public int SortInt(double d)
        {
            if(d>0)
            {
                return 1;
            }
            else if(d<0)
            {
                return -1;
            }
            return 0;
        }
    }
}