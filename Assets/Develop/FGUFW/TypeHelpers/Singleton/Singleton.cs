using System;
using UnityEngine;

namespace FGUFW.Core
{
	public abstract class Singleton<T> where T : class, new()
	{
		private static T _instance;
		public static T I
		{
			get
			{
				if (Singleton<T>._instance == null)
				{
					Singleton<T>._instance = Activator.CreateInstance<T>();
					if (Singleton<T>._instance != null)
					{
						(Singleton<T>._instance as Singleton<T>).Init();
					}
				}

				return Singleton<T>._instance;
			}
		}

		public static void Release()
		{
			if (Singleton<T>._instance != null)
			{
				Singleton<T>._instance = (T)((object)null);
			}
		}

		public virtual void Init()
		{

		}

		public abstract void Dispose();

	}
}
