using System;
using System.Net;
using UnityEngine;

namespace FGUFW.Core
{
    static public class StringHelper
    {
        static public Uri ToUri(this string text)
        {
            return new Uri(text);
        }
        
        static public IPAddress ToIP(this string text)
        {
            return IPAddress.Parse(text);
        }

        public static int ToInt32(this string text)
        {
            return int.Parse(text);
        }
        
        public static float ToFloat(this string text)
        {
            return float.Parse(text);
        }

        public static T FromJson<T>(this string text)
        {
            return JsonUtility.FromJson<T>(text);
        }
    }
}