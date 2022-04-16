
using System;
using System.Collections.Generic;
using Unity.Collections;

namespace FGUFW.ECS
{
    public sealed partial class World
    {

        /// <summary>
        /// 存储过滤函数Filter的缓存 减少GC
        /// </summary>
        /// <returns></returns>
        private Dictionary<int,List<IComponent>> _filterCacheDict = new Dictionary<int, List<IComponent>>();

        private List<IComponent> getFilterCache(int compType)
        {
            if(!_filterCacheDict.ContainsKey(compType))_filterCacheDict.Add(compType,new List<IComponent>());
            _filterCacheDict[compType].Clear();
            return _filterCacheDict[compType];
        }

        public void Filter(Archetype archetype,Action<List<IComponent>[]> callback)
        {
            //找到组件数最少的组件类型 提高匹配效率
            int minCountType = -1,minCount = int.MaxValue;

            //若凑不齐原型中的组件类型集合则直接退出
            int compTypeCount = 0;
            foreach (var kv in _compsDict)
            {
                if(archetype.Contains(kv.Key))
                {
                    if(kv.Value.Count<minCount)
                    {
                        minCount = kv.Value.Count;
                        minCountType = kv.Key;
                    }
                    compTypeCount++;
                }
            }
            if(minCountType == -1 || compTypeCount!=archetype.Length || minCount==0)return;

            //筛选出的结果 从缓存池获取筛选列表 减少GC
            var result = new List<IComponent>[archetype.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var compType = archetype.ComponentTypes[i];
                result[i] = getFilterCache(compType);
            }

            var minComps = _compsDict[minCountType];
            //筛选出的实体数量
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                //用数量少的组件类型的每一个实体 匹配是否满足实体原型 并添加进筛选结果
                var entityUID = item.EntityUID;
                for (int i = 0; i < result.Length; i++)
                {
                    var compType = archetype.ComponentTypes[i];
                    if(compType==item.Type)
                    {
                        addOrSetResultCache(result[i],item,resultIndex);
                    }
                    else
                    {
                        IComponent comp;
                        if(GetComponent(entityUID,compType,out comp))
                        {
                            addOrSetResultCache(result[i],comp,resultIndex);
                        }
                        else
                        {
                            resultIndex--;
                            continue;
                        }
                    }
                }
                resultIndex++;
            }

            //去除筛选结果中多添加的组件
            for (int i = 0; i < result.Length; i++)
            {
                int resultCount = result[i].Count;
                if(resultCount!=resultIndex)
                {
                    result[i].RemoveAt(resultIndex);
                }
            }

            callback(result);
        }

        public void FilterMatch(Archetype archetype,Func<IComponent[],bool> match,Action<List<IComponent>[]> callback)
        {

            int minCountType = -1,minCount = int.MaxValue;
            int compTypeCount = 0;
            foreach (var kv in _compsDict)
            {
                if(archetype.Contains(kv.Key))
                {
                    if(kv.Value.Count<minCount)
                    {
                        minCount = kv.Value.Count;
                        minCountType = kv.Key;
                    }
                    compTypeCount++;
                }
            }
            if(minCountType == -1 || compTypeCount!=archetype.Length || minCount==0)return;

            var result = new List<IComponent>[archetype.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var compType = archetype.ComponentTypes[i];
                result[i] = getFilterCache(compType);
            }

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                for (int i = 0; i < result.Length; i++)
                {
                    var compType = archetype.ComponentTypes[i];
                    if(compType==item.Type)
                    {
                        addOrSetResultCache(result[i],item,resultIndex);
                    }
                    else
                    {
                        IComponent comp;
                        if(GetComponent(entityUID,compType,out comp))
                        {
                            addOrSetResultCache(result[i],comp,resultIndex);
                        }
                        else
                        {
                            resultIndex--;
                            continue;
                        }
                    }
                }
                resultIndex++;
            }

            for (int i = 0; i < result.Length; i++)
            {
                int resultCount = result[i].Count;
                if(resultCount!=resultIndex)
                {
                    result[i].RemoveAt(resultIndex);
                }
            }

            callback(result);
        }

        public void FilterJob<T0>(Action<NativeArray<T0>> callback) where T0:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();

            if(!_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0)return;

            int entityCount = _compsDict[t0_Type].Count;
            var nt0 = new NativeArray<T0>(entityCount,Allocator.TempJob);

            for (int i = 0; i < entityCount; i++)
            {
                nt0[i]=(T0)_compsDict[t0_Type][i];    
            }

            callback(nt0);
            nt0.Dispose();
        }

        public void FilterJob<T0,T1>(Action<NativeArray<T0>,NativeArray<T1>> callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            
            var t1_cache = getFilterCache(t1_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                IComponent comp;
                
                if(GetComponent(entityUID,t0_Type,out comp))
                {
                    addOrSetResultCache(t0_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t1_Type,out comp))
                {
                    addOrSetResultCache(t1_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
            }
            callback(nt0,nt1);
            nt0.Dispose();
            nt1.Dispose();
        }

        public void FilterJob<T0,T1,T2>(Action<NativeArray<T0>,NativeArray<T1>,NativeArray<T2> > callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        where T2:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();
            var t2_Type = ComponentHelper.GetType<T2>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
             || !_compsDict.ContainsKey(t2_Type) || _compsDict[t2_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }
            if(_compsDict[t2_Type].Count<minCount)
            {
                minCount = _compsDict[t2_Type].Count;
                minCountType = _compsDict[t2_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            var t1_cache = getFilterCache(t1_Type);
            var t2_cache = getFilterCache(t2_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                IComponent comp;
                
                if(GetComponent(entityUID,t0_Type,out comp))
                {
                    addOrSetResultCache(t0_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t1_Type,out comp))
                {
                    addOrSetResultCache(t1_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t2_Type,out comp))
                {
                    addOrSetResultCache(t2_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);
            var nt2 = new NativeArray<T2>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
                nt2[i] = (T2)t2_cache[i];
            }
            callback(nt0,nt1,nt2);
            nt0.Dispose();
            nt1.Dispose();
            nt2.Dispose();
        }

        public void FilterJob<T0,T1,T2,T3>(Action<NativeArray<T0>,NativeArray<T1>,NativeArray<T2>,NativeArray<T3> > callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        where T2:struct,IComponent
        where T3:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();
            var t2_Type = ComponentHelper.GetType<T2>();
            var t3_Type = ComponentHelper.GetType<T3>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
             || !_compsDict.ContainsKey(t2_Type) || _compsDict[t2_Type].Count==0
             || !_compsDict.ContainsKey(t3_Type) || _compsDict[t3_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }
            if(_compsDict[t2_Type].Count<minCount)
            {
                minCount = _compsDict[t2_Type].Count;
                minCountType = _compsDict[t2_Type][0].Type;
            }
            if(_compsDict[t3_Type].Count<minCount)
            {
                minCount = _compsDict[t3_Type].Count;
                minCountType = _compsDict[t3_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            var t1_cache = getFilterCache(t1_Type);
            var t2_cache = getFilterCache(t2_Type);
            var t3_cache = getFilterCache(t3_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                IComponent comp;
                
                if(GetComponent(entityUID,t0_Type,out comp))
                {
                    addOrSetResultCache(t0_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t1_Type,out comp))
                {
                    addOrSetResultCache(t1_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t2_Type,out comp))
                {
                    addOrSetResultCache(t2_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t3_Type,out comp))
                {
                    addOrSetResultCache(t3_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);
            var nt2 = new NativeArray<T2>(resultIndex,Allocator.TempJob);
            var nt3 = new NativeArray<T3>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
                nt2[i] = (T2)t2_cache[i];
                nt3[i] = (T3)t3_cache[i];
            }
            callback(nt0,nt1,nt2,nt3);
            nt0.Dispose();
            nt1.Dispose();
            nt2.Dispose();
            nt3.Dispose();
        }


        public void FilterJob<T0,T1,T2,T3,T4>(Action<NativeArray<T0>,NativeArray<T1>,NativeArray<T2>,NativeArray<T3>,NativeArray<T4> > callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        where T2:struct,IComponent
        where T3:struct,IComponent
        where T4:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();
            var t2_Type = ComponentHelper.GetType<T2>();
            var t3_Type = ComponentHelper.GetType<T3>();
            var t4_Type = ComponentHelper.GetType<T4>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
             || !_compsDict.ContainsKey(t2_Type) || _compsDict[t2_Type].Count==0
             || !_compsDict.ContainsKey(t3_Type) || _compsDict[t3_Type].Count==0
             || !_compsDict.ContainsKey(t4_Type) || _compsDict[t4_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }
            if(_compsDict[t2_Type].Count<minCount)
            {
                minCount = _compsDict[t2_Type].Count;
                minCountType = _compsDict[t2_Type][0].Type;
            }
            if(_compsDict[t3_Type].Count<minCount)
            {
                minCount = _compsDict[t3_Type].Count;
                minCountType = _compsDict[t3_Type][0].Type;
            }
            if(_compsDict[t4_Type].Count<minCount)
            {
                minCount = _compsDict[t4_Type].Count;
                minCountType = _compsDict[t4_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            var t1_cache = getFilterCache(t1_Type);
            var t2_cache = getFilterCache(t2_Type);
            var t3_cache = getFilterCache(t3_Type);
            var t4_cache = getFilterCache(t4_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                IComponent comp;
                
                if(GetComponent(entityUID,t0_Type,out comp))
                {
                    addOrSetResultCache(t0_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t1_Type,out comp))
                {
                    addOrSetResultCache(t1_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t2_Type,out comp))
                {
                    addOrSetResultCache(t2_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t3_Type,out comp))
                {
                    addOrSetResultCache(t3_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t4_Type,out comp))
                {
                    addOrSetResultCache(t4_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);
            var nt2 = new NativeArray<T2>(resultIndex,Allocator.TempJob);
            var nt3 = new NativeArray<T3>(resultIndex,Allocator.TempJob);
            var nt4 = new NativeArray<T4>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
                nt2[i] = (T2)t2_cache[i];
                nt3[i] = (T3)t3_cache[i];
                nt4[i] = (T4)t4_cache[i];
            }
            callback(nt0,nt1,nt2,nt3,nt4);
            nt0.Dispose();
            nt1.Dispose();
            nt2.Dispose();
            nt3.Dispose();
            nt4.Dispose();
        }

        public void FilterJob<T0,T1,T2,T3,T4,T5>(Action<NativeArray<T0>,NativeArray<T1>,NativeArray<T2>,NativeArray<T3>,NativeArray<T4>,NativeArray<T5> > callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        where T2:struct,IComponent
        where T3:struct,IComponent
        where T4:struct,IComponent
        where T5:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();
            var t2_Type = ComponentHelper.GetType<T2>();
            var t3_Type = ComponentHelper.GetType<T3>();
            var t4_Type = ComponentHelper.GetType<T4>();
            var t5_Type = ComponentHelper.GetType<T5>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
             || !_compsDict.ContainsKey(t2_Type) || _compsDict[t2_Type].Count==0
             || !_compsDict.ContainsKey(t3_Type) || _compsDict[t3_Type].Count==0
             || !_compsDict.ContainsKey(t4_Type) || _compsDict[t4_Type].Count==0
             || !_compsDict.ContainsKey(t5_Type) || _compsDict[t5_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }
            if(_compsDict[t2_Type].Count<minCount)
            {
                minCount = _compsDict[t2_Type].Count;
                minCountType = _compsDict[t2_Type][0].Type;
            }
            if(_compsDict[t3_Type].Count<minCount)
            {
                minCount = _compsDict[t3_Type].Count;
                minCountType = _compsDict[t3_Type][0].Type;
            }
            if(_compsDict[t4_Type].Count<minCount)
            {
                minCount = _compsDict[t4_Type].Count;
                minCountType = _compsDict[t4_Type][0].Type;
            }
            if(_compsDict[t5_Type].Count<minCount)
            {
                minCount = _compsDict[t5_Type].Count;
                minCountType = _compsDict[t5_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            var t1_cache = getFilterCache(t1_Type);
            var t2_cache = getFilterCache(t2_Type);
            var t3_cache = getFilterCache(t3_Type);
            var t4_cache = getFilterCache(t4_Type);
            var t5_cache = getFilterCache(t5_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                IComponent comp;
                
                if(GetComponent(entityUID,t0_Type,out comp))
                {
                    addOrSetResultCache(t0_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t1_Type,out comp))
                {
                    addOrSetResultCache(t1_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t2_Type,out comp))
                {
                    addOrSetResultCache(t2_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t3_Type,out comp))
                {
                    addOrSetResultCache(t3_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t4_Type,out comp))
                {
                    addOrSetResultCache(t4_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t5_Type,out comp))
                {
                    addOrSetResultCache(t5_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);
            var nt2 = new NativeArray<T2>(resultIndex,Allocator.TempJob);
            var nt3 = new NativeArray<T3>(resultIndex,Allocator.TempJob);
            var nt4 = new NativeArray<T4>(resultIndex,Allocator.TempJob);
            var nt5 = new NativeArray<T5>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
                nt2[i] = (T2)t2_cache[i];
                nt3[i] = (T3)t3_cache[i];
                nt4[i] = (T4)t4_cache[i];
                nt5[i] = (T5)t5_cache[i];
            }
            callback(nt0,nt1,nt2,nt3,nt4,nt5);
            nt0.Dispose();
            nt1.Dispose();
            nt2.Dispose();
            nt3.Dispose();
            nt4.Dispose();
            nt5.Dispose();
        }

        public void FilterJob<T0,T1,T2,T3,T4,T5,T6>(Action<NativeArray<T0>,NativeArray<T1>,NativeArray<T2>,NativeArray<T3>,NativeArray<T4>,NativeArray<T5>,NativeArray<T6> > callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        where T2:struct,IComponent
        where T3:struct,IComponent
        where T4:struct,IComponent
        where T5:struct,IComponent
        where T6:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();
            var t2_Type = ComponentHelper.GetType<T2>();
            var t3_Type = ComponentHelper.GetType<T3>();
            var t4_Type = ComponentHelper.GetType<T4>();
            var t5_Type = ComponentHelper.GetType<T5>();
            var t6_Type = ComponentHelper.GetType<T6>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
             || !_compsDict.ContainsKey(t2_Type) || _compsDict[t2_Type].Count==0
             || !_compsDict.ContainsKey(t3_Type) || _compsDict[t3_Type].Count==0
             || !_compsDict.ContainsKey(t4_Type) || _compsDict[t4_Type].Count==0
             || !_compsDict.ContainsKey(t5_Type) || _compsDict[t5_Type].Count==0
             || !_compsDict.ContainsKey(t6_Type) || _compsDict[t6_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }
            if(_compsDict[t2_Type].Count<minCount)
            {
                minCount = _compsDict[t2_Type].Count;
                minCountType = _compsDict[t2_Type][0].Type;
            }
            if(_compsDict[t3_Type].Count<minCount)
            {
                minCount = _compsDict[t3_Type].Count;
                minCountType = _compsDict[t3_Type][0].Type;
            }
            if(_compsDict[t4_Type].Count<minCount)
            {
                minCount = _compsDict[t4_Type].Count;
                minCountType = _compsDict[t4_Type][0].Type;
            }
            if(_compsDict[t5_Type].Count<minCount)
            {
                minCount = _compsDict[t5_Type].Count;
                minCountType = _compsDict[t5_Type][0].Type;
            }
            if(_compsDict[t6_Type].Count<minCount)
            {
                minCount = _compsDict[t6_Type].Count;
                minCountType = _compsDict[t6_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            var t1_cache = getFilterCache(t1_Type);
            var t2_cache = getFilterCache(t2_Type);
            var t3_cache = getFilterCache(t3_Type);
            var t4_cache = getFilterCache(t4_Type);
            var t5_cache = getFilterCache(t5_Type);
            var t6_cache = getFilterCache(t6_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                IComponent comp;
                
                if(GetComponent(entityUID,t0_Type,out comp))
                {
                    addOrSetResultCache(t0_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t1_Type,out comp))
                {
                    addOrSetResultCache(t1_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t2_Type,out comp))
                {
                    addOrSetResultCache(t2_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t3_Type,out comp))
                {
                    addOrSetResultCache(t3_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t4_Type,out comp))
                {
                    addOrSetResultCache(t4_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t5_Type,out comp))
                {
                    addOrSetResultCache(t5_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t6_Type,out comp))
                {
                    addOrSetResultCache(t6_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);
            var nt2 = new NativeArray<T2>(resultIndex,Allocator.TempJob);
            var nt3 = new NativeArray<T3>(resultIndex,Allocator.TempJob);
            var nt4 = new NativeArray<T4>(resultIndex,Allocator.TempJob);
            var nt5 = new NativeArray<T5>(resultIndex,Allocator.TempJob);
            var nt6 = new NativeArray<T6>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
                nt2[i] = (T2)t2_cache[i];
                nt3[i] = (T3)t3_cache[i];
                nt4[i] = (T4)t4_cache[i];
                nt5[i] = (T5)t5_cache[i];
                nt6[i] = (T6)t6_cache[i];
            }
            callback(nt0,nt1,nt2,nt3,nt4,nt5,nt6);
            nt0.Dispose();
            nt1.Dispose();
            nt2.Dispose();
            nt3.Dispose();
            nt4.Dispose();
            nt5.Dispose();
            nt6.Dispose();
        }

        public void FilterJob<T0,T1,T2,T3,T4,T5,T6,T7>(Action<NativeArray<T0>,NativeArray<T1>,NativeArray<T2>,NativeArray<T3>,NativeArray<T4>,NativeArray<T5>,NativeArray<T6>,NativeArray<T7> > callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        where T2:struct,IComponent
        where T3:struct,IComponent
        where T4:struct,IComponent
        where T5:struct,IComponent
        where T6:struct,IComponent
        where T7:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();
            var t2_Type = ComponentHelper.GetType<T2>();
            var t3_Type = ComponentHelper.GetType<T3>();
            var t4_Type = ComponentHelper.GetType<T4>();
            var t5_Type = ComponentHelper.GetType<T5>();
            var t6_Type = ComponentHelper.GetType<T6>();
            var t7_Type = ComponentHelper.GetType<T7>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
             || !_compsDict.ContainsKey(t2_Type) || _compsDict[t2_Type].Count==0
             || !_compsDict.ContainsKey(t3_Type) || _compsDict[t3_Type].Count==0
             || !_compsDict.ContainsKey(t4_Type) || _compsDict[t4_Type].Count==0
             || !_compsDict.ContainsKey(t5_Type) || _compsDict[t5_Type].Count==0
             || !_compsDict.ContainsKey(t6_Type) || _compsDict[t6_Type].Count==0
             || !_compsDict.ContainsKey(t7_Type) || _compsDict[t7_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }
            if(_compsDict[t2_Type].Count<minCount)
            {
                minCount = _compsDict[t2_Type].Count;
                minCountType = _compsDict[t2_Type][0].Type;
            }
            if(_compsDict[t3_Type].Count<minCount)
            {
                minCount = _compsDict[t3_Type].Count;
                minCountType = _compsDict[t3_Type][0].Type;
            }
            if(_compsDict[t4_Type].Count<minCount)
            {
                minCount = _compsDict[t4_Type].Count;
                minCountType = _compsDict[t4_Type][0].Type;
            }
            if(_compsDict[t5_Type].Count<minCount)
            {
                minCount = _compsDict[t5_Type].Count;
                minCountType = _compsDict[t5_Type][0].Type;
            }
            if(_compsDict[t6_Type].Count<minCount)
            {
                minCount = _compsDict[t6_Type].Count;
                minCountType = _compsDict[t6_Type][0].Type;
            }
            if(_compsDict[t7_Type].Count<minCount)
            {
                minCount = _compsDict[t7_Type].Count;
                minCountType = _compsDict[t7_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            var t1_cache = getFilterCache(t1_Type);
            var t2_cache = getFilterCache(t2_Type);
            var t3_cache = getFilterCache(t3_Type);
            var t4_cache = getFilterCache(t4_Type);
            var t5_cache = getFilterCache(t5_Type);
            var t6_cache = getFilterCache(t6_Type);
            var t7_cache = getFilterCache(t7_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;
                IComponent comp;
                
                if(GetComponent(entityUID,t0_Type,out comp))
                {
                    addOrSetResultCache(t0_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t1_Type,out comp))
                {
                    addOrSetResultCache(t1_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t2_Type,out comp))
                {
                    addOrSetResultCache(t2_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t3_Type,out comp))
                {
                    addOrSetResultCache(t3_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t4_Type,out comp))
                {
                    addOrSetResultCache(t4_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t5_Type,out comp))
                {
                    addOrSetResultCache(t5_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t6_Type,out comp))
                {
                    addOrSetResultCache(t6_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }
                
                if(GetComponent(entityUID,t7_Type,out comp))
                {
                    addOrSetResultCache(t7_cache,comp,resultIndex);
                }
                else
                {
                    continue;
                }

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);
            var nt2 = new NativeArray<T2>(resultIndex,Allocator.TempJob);
            var nt3 = new NativeArray<T3>(resultIndex,Allocator.TempJob);
            var nt4 = new NativeArray<T4>(resultIndex,Allocator.TempJob);
            var nt5 = new NativeArray<T5>(resultIndex,Allocator.TempJob);
            var nt6 = new NativeArray<T6>(resultIndex,Allocator.TempJob);
            var nt7 = new NativeArray<T7>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
                nt2[i] = (T2)t2_cache[i];
                nt3[i] = (T3)t3_cache[i];
                nt4[i] = (T4)t4_cache[i];
                nt5[i] = (T5)t5_cache[i];
                nt6[i] = (T6)t6_cache[i];
                nt7[i] = (T7)t6_cache[i];
            }
            callback(nt0,nt1,nt2,nt3,nt4,nt5,nt6,nt7);
            nt0.Dispose();
            nt1.Dispose();
            nt2.Dispose();
            nt3.Dispose();
            nt4.Dispose();
            nt5.Dispose();
            nt6.Dispose();
            nt7.Dispose();
        }

        public void FilterMatchJob<T0,T1>(Func<T0,T1,bool> match, Action<NativeArray<T0>,NativeArray<T1>> callback) 
        where T0:struct,IComponent
        where T1:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
            var t1_Type = ComponentHelper.GetType<T1>();

            if( !_compsDict.ContainsKey(t0_Type) || _compsDict[t0_Type].Count==0
             || !_compsDict.ContainsKey(t1_Type) || _compsDict[t1_Type].Count==0
            )return;

            int minCount = int.MaxValue;
            int minCountType = -1;
            if(_compsDict[t0_Type].Count<minCount)
            {
                minCount = _compsDict[t0_Type].Count;
                minCountType = _compsDict[t0_Type][0].Type;
            }
            if(_compsDict[t1_Type].Count<minCount)
            {
                minCount = _compsDict[t1_Type].Count;
                minCountType = _compsDict[t1_Type][0].Type;
            }

            var t0_cache = getFilterCache(t0_Type);
            
            var t1_cache = getFilterCache(t1_Type);

            var minComps = _compsDict[minCountType];
            int resultIndex = 0;
            foreach (var item in minComps)
            {
                var entityUID = item.EntityUID;

                IComponent t0;
                if(GetComponent(entityUID,t0_Type,out t0))
                {
                    addOrSetResultCache(t0_cache,t0,resultIndex);
                }
                else
                {
                    continue;
                }
                
                IComponent t1;
                if(GetComponent(entityUID,t1_Type,out t1))
                {
                    addOrSetResultCache(t1_cache,t1,resultIndex);
                }
                else
                {
                    continue;
                }

                if(!match((T0)t0,(T1)t1))continue;

                resultIndex++;
            }
            
            var nt0 = new NativeArray<T0>(resultIndex,Allocator.TempJob);
            var nt1 = new NativeArray<T1>(resultIndex,Allocator.TempJob);

            for (int i = 0; i < resultIndex; i++)
            {
                nt0[i] = (T0)t0_cache[i];
                nt1[i] = (T1)t1_cache[i];
            }
            callback(nt0,nt1);
            nt0.Dispose();
            nt1.Dispose();
        }



    }
}