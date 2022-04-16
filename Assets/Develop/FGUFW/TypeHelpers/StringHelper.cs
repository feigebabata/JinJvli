using System;
using System.Collections.Generic;
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

        public static IReadOnlyDictionary<string,IReadOnlyList<string>> ToCsvDict(this string csvText,int startLine=1)
        {
            Dictionary<string,IReadOnlyList<string>> dict = new Dictionary<string,IReadOnlyList<string>>();
            csvText = csvText.Trim();
            string[] lines = csvText.Split('\n');
            for (int i = startLine; i < lines.Length; i++)
            {
                string[] nodes = lines[i].Split(',');
                dict.Add(nodes[0],nodes);
            }
            return dict;
        }

        static public Color ToColor(this string self)
        {
            Color color;
            ColorUtility.TryParseHtmlString(self,out color);
            return color;
        }

        static public Dictionary<string,string> ToCsvLines(this string csvText)
        {
            var dict = new Dictionary<string,string>();
            csvText = csvText.Trim();
            string[] lines = csvText.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] nodes = lines[i].Split(',');
                dict.Add(nodes[0],lines[i]);
            }
            return dict;
        }

        static public T ToEnum<T>(this string self) where T:Enum
        {
            return (T)Enum.Parse(typeof(T),self);
        }

    }
}