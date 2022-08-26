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
            string[] lines = csvText.ToCsvLines();
            for (int i = startLine; i < lines.Length; i++)
            {
                string[] nodes = lines[i].Trim().Split(',');
                var length = nodes.Length;
                for (int j = 0; j < length; j++)
                {
                    var node = nodes[j];
                    if(node.Contains("\n"))
                    {
                        node = node.Substring(1,node.Length-2);
                        nodes[j] = node;
                    }
                }
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

        static public string[,] ToCsvTable(this string csvText)
        {
            csvText = csvText.Trim();
            string[] arr = csvText.ToCsvLines();
            int y = arr.Length;
            string[,] lines = null;
            for (int i = 0; i < y; i++)
            {
                var items = arr[i].Split(',');
                int x = items.Length;
                if(lines==null)lines = new string[x,y];
                for (int j = 0; j < x; j++)
                {
                    lines[j,i]=items[j];
                }
            }
            return lines;
        }

        static public string[] ToCsvLines(this string csvText)
        {
            csvText = csvText.Trim();
            return csvText.Split(new string[]{"\r\n"},StringSplitOptions.None);
        }

        static public T ToEnum<T>(this string self) where T:Enum
        {
            return (T)Enum.Parse(typeof(T),self);
        }

    }
}