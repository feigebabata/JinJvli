// using System;
// using Unity.Collections.LowLevel.Unsafe;

// namespace FGUFW.Core
// {
//     static public class UnsafeListHelper
//     {
//         static public int IndexOf<T>(this UnsafeList<T> array,T val) where T : unmanaged
//         {
//             if(array.IsCreated)return -1;
//             int length = array.Length;
//             for (int i = 0; i < length; i++)
//             {
//                 if(array[i].Equals(val))return i;
//             }
//             return -1;
//         }

//         static public int IndexOf<T>(this UnsafeList<T> array,Predicate<T> match) where T : unmanaged
//         {
//             int length = array.Length;
//             for (int i = 0; i < length; i++)
//             {
//                 if(match(array[i]))return i;
//             }
//             return -1;
//         }

//         static public void Remove<T>(this UnsafeList<T> array,Predicate<T> match) where T : unmanaged
//         {
//             int idx = array.IndexOf(match);
//             if(idx != -1)array.RemoveAt(idx);
//         }
//     }

// }
