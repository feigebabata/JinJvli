using System;

public static class CharExpand
{
    public static bool IsZH_CN(this char _char)
    {
        int cn_from = Convert.ToInt32("4e00", 16); //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
        int cn_end = Convert.ToInt32("9fff", 16);

        return _char >= cn_from && _char <= cn_end;
    }
}