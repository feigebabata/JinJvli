using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public static class ColorHelper
    {
        static public string RichText(this Color color,string text)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }
    }
}
