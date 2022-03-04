

using System;

namespace FGUFW.Core
{
    static public class DelegateHelper
    {
        static public void ForEach<T>(this Delegate d,Action<T> callback) where T:Delegate
        {
            var arr = d.GetInvocationList();
            foreach (T item in arr)
            {
                callback(item);
            }
            
        }
    }
}