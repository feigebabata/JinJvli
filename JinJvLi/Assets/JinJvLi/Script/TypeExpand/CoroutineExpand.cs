using System;
using UnityEngine;

public static class CoroutineExpand
{
    public static bool Run(this Coroutine _coro)
    {
        return Coroutines.Inst.Run(_coro);
    }
    public static bool HasZH_CN(this string _text)
    {
        return Regex.IsMatch(_text,"^[\u4e00-\u9fa5]$");
    }
}