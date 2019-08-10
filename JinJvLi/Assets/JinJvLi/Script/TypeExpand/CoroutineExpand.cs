using System;
using System.Collections;
using UnityEngine;

namespace JinJvli
{
    public static class CoroutineExpand
    {
        public static Coroutine Start(this IEnumerator _enumerator)
        {
            return Coroutines.Inst.Run(_enumerator);
        }

        public static void Stop(this Coroutine _enumerator)
        {
            Coroutines.Inst.Stop(_enumerator);
        }
    }
}