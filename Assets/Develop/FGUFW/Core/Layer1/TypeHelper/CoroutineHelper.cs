using System.Collections;
using UnityEngine;

namespace FGUFW.Core
{
    static public class CoroutineHelper
    {
		/// <summary>
		/// 启动协程 需要主动结束协程调用Coroutine.Stop()
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		static public Coroutine Start(this IEnumerator self)
		{
			return CoroutineCore.I.StartCoroutine(self);
		}

		static public void Stop(this Coroutine self)
		{
			CoroutineCore.I.StopCoroutine(self);
		}

		/// <summary>
		/// 文件读写或网络请求用这个 CoroutineCore内部做了并行数量的限制
		/// </summary>
		/// <param name="self"></param>
		/// <returns>返回值可以用在CoroutineCore.I.StopIO(int id) 来结束加载协程</returns>
		static public int StartIO(this IEnumerator self)
		{
			return CoroutineCore.I.StartIO(self);
		}
    }    
}