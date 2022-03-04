using System;
using System.Collections.Generic;

namespace FGUFW.Core
{
    static public class CollectionHelper
    {
        static public T Random<T>(this List<T> self)
        {
            if(self==null || self.Count==0)
            {
                return default(T);
            }
            int idx = UnityEngine.Random.Range(0,self.Count);
            return self[idx];
        }

        static public T Random<T>(this Array self)
        {
            if(self==null || self.Length==0)
            {
                return default(T);
            }
            int idx = UnityEngine.Random.Range(0,self.Length);
            return (T)self.GetValue(idx);
        }
    }
}