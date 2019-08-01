using System;
using System.Text.RegularExpressions;

public static class StringExpand
{
    public static bool IsZH_CN(this string _text)
    {
        int cn_from = Convert.ToInt32("4e00", 16); //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
        int cn_end = Convert.ToInt32("9fff", 16);
        for (int i = 0; i < _text.Length; i++)
        {
            if(_text[i]<cn_from || _text[i]>cn_end)
            {
                return false;
            }
        }

        return true;
    }
    public static bool HasZH_CN(this string _text)
    {
        return Regex.IsMatch(_text,"^[\u4e00-\u9fa5]$");
    }
}