using System.Collections;
using UnityEngine;

namespace FGUFW.Core
{
    static public class CoroutineHelper
    {
		static public Coroutine Start(this IEnumerator self)
		{
			return CoroutineCore.I.StartCoroutine(self);
		}

		static public void Stop(this Coroutine self)
		{
			CoroutineCore.I.StopCoroutine(self);
		}

		static public int StartIO(this IEnumerator self)
		{
			return CoroutineCore.I.StartIO(self);
		}
    }    
}