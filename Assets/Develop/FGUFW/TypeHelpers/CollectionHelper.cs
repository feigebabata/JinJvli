using System;
using System.Collections;
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

        static public int IndexOf(this Array self,object val,int startIndex=0)
        {
            if(self!=null)
            {
                return Array.IndexOf(self,val,startIndex);
            }
            return -1;
        }

        static public int IndexOf<T>(this Array self,Predicate<T> match,int startIndex=0)
        {
            if(self!=null)
            {
                int length = self.Length;
                for (int i = 0; i < length; i++)
                {
                    var t_obj = (T)self.GetValue(i);
                    if(match(t_obj))return i;
                }
            }
            return -1;
        }

        static public T Find<T>(this Array self,Predicate<T> match)
        {
            if(self!=null)
            {
                int length = self.Length;
                for (int i = 0; i < length; i++)
                {
                    var t_obj = (T)self.GetValue(i);
                    if(match(t_obj))return t_obj;
                }
            }
            return default(T);
        }

        
        /// <summary>
        /// 按权重随机
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="getWeight"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static public T RandomByWeight<T>(this IEnumerable collection,Func<T,float> getWeight)
        {
            float maxValue = 0;
            foreach (T item in collection)
            {
                maxValue += getWeight(item);
            }
            float val = UnityEngine.Random.Range(0,maxValue);
            float weight = 0;
            foreach (T item in collection)
            {
                weight += getWeight(item);
                if(val<weight)
                {
                    return item;
                }
            }
            return default(T);
        }

        static public T[] Copy<T>(this Array self)
        {
            if(self==null || self.Length==0)
            {
                return null;
            }
            T[] newArray = new T[self.Length];
            // for (int i = 0; i < self.Length; i++)
            // {
            //     newArray[i]=(T)self.GetValue(i);
            // }
            Array.Copy(self,newArray,self.Length);
            return newArray;
        }

    }
}