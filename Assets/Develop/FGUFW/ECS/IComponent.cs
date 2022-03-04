

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine;

namespace FGUFW.ECS
{
    public interface IComponent
    {
        int Type{get;}
        int EntityUID{get;set;}
    }

    static public class ComponentHelper
    {
        static private Dictionary<Type,int> CompTypeDict = new Dictionary<Type,int>();

        static public void CopyToNative<T>(this List<IComponent> self,NativeArray<T> nativeArray) where T:struct,IComponent
        {
            int length = self.Count;
            for (int i = 0; i < length; i++)
            {
                nativeArray[i]=(T)self[i];
            }
        }

        
        static public NativeArray<T> ToNativeArray<T>(this List<IComponent> self,Allocator allocator=Allocator.TempJob) where T:struct,IComponent
        {
            int length = self.Count;
            var nativeArray = new NativeArray<T>(length,allocator);
            for (int i = 0; i < length; i++)
            {
                nativeArray[i]=(T)self[i];
            }
            return nativeArray;
        }

        static public void Foreach<T0>(this List<IComponent>[] self,Action<T0> callback)
        {
            int length = self[0].Count;
            int t1_index = 0;
            for (int i = 0; i < length; i++)
            {
                var t0 = (T0)self[t1_index][i];
                callback(t0);
            }
        }

        static public void Foreach<T0,T1>(this List<IComponent>[] self,Action<T0,T1> callback) where T0:IComponent,new() where T1:IComponent,new() 
        {
            int length = self[0].Count;

            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();

            var t0_index = Array.FindIndex<List<IComponent>>(self,cs=>cs[0].Type==t0_Type);
            var t1_index = Array.FindIndex<List<IComponent>>(self,cs=>cs[0].Type==t1_Type);
            
            for (int i = 0; i < length; i++)
            {
                var t0 = (T0)self[t0_index][i];
                var t1 = (T1)self[t1_index][i];
                callback(t0,t1);
            }
        }

        static public void Foreach<T0,T1,T2>(this List<IComponent>[] self,Action<T0,T1,T2> callback) where T0:IComponent,new() where T1:IComponent,new()  where T2:IComponent,new() 
        {
            int length = self[0].Count;

            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();

            var t0_index = Array.FindIndex<List<IComponent>>(self,cs=>cs[0].Type==t0_Type);
            var t1_index = Array.FindIndex<List<IComponent>>(self,cs=>cs[0].Type==t1_Type);

            for (int i = 0; i < length; i++)
            {
                var t0 = (T0)self[0][i];
                var t1 = (T1)self[1][i];
                var t2 = (T2)self[2][i];
                callback(t0,t1,t2);
            }
        }

        static public int GetType<T>() where T:IComponent,new()
        {
            var t = typeof(T);
            if(!CompTypeDict.ContainsKey(t))
            {
                var t_obj = new T();
                CompTypeDict.Add(t,t_obj.Type);
            }
            return CompTypeDict[t];
        }

        [UnityEditor.MenuItem("FGUFW.ECS/CheckCompType")]
        static public void CheckCompType()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var compTypeName = typeof(IComponent).FullName;
            Dictionary<int,Type> record = new Dictionary<int, Type>();
            foreach (var type in types)
            {
                if(type.IsValueType && type.GetInterface(compTypeName)!=null)
                {
                    var val = (IComponent)Activator.CreateInstance(type);
                    var compType = val.Type;
                    if(!record.ContainsKey(compType))
                    {
                        record.Add(compType,type);
                    }
                    else
                    {
                        var old_type = record[compType];
                        Debug.LogError($"组件类型冲突 {compType} {type.FullName} : {old_type.FullName}");
                    }
                }
            }
            record.Clear();
        }
    }

}