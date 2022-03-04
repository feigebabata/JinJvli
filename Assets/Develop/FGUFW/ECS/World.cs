using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace FGUFW.ECS
{
    public sealed class World
    {

        private List<ISystem> _systems = new List<ISystem>();
        private Dictionary<int,List<IComponent>> _compsDict = new Dictionary<int, List<IComponent>>();
        private Dictionary<int,List<IComponent>> _filterCacheDict = new Dictionary<int, List<IComponent>>();
        public bool Pause=false;
        private int _createEntityIndex=0;

        public void AddSystem(ISystem system)
        {
            _systems.Add(system);
            _systems.Sort((l,r)=>r.Order-l.Order);
        }

        public void RemoveSystem(ISystem system)
        {
            _systems.Add(system);
            _systems.Sort((l,r)=>r.Order-l.Order);
        }

        public void SetSystemEnable(int systemType,bool enable)
        {
            var sys = _systems.Find(sys=>sys.Type==systemType);
            if(sys != null)
            {
                sys.Enabled = enable;
            }
        }

        public void InitSystem()
        {
            foreach (var sys in _systems)
            {
                sys.OnInit(this);
            }
        }

        public void AddOrSetComponent(int entityUID,IComponent component)
        {
            component.EntityUID = entityUID;
            int c_t = component.Type;
            if(!_compsDict.ContainsKey(c_t))
            {
                _compsDict.Add(c_t,new List<IComponent>());
            }

            var _comps = _compsDict[c_t];
            int length = _comps.Count;
            for (int i = 0; i < length; i++)
            {
                if(_comps[i].EntityUID==entityUID)
                {
                    _comps[i] = component;
                    return;
                }
            }
            _comps.Add(component);
        }

        public void RemoveComponent(int entityUID,int componentType)
        {
            if(!_compsDict.ContainsKey(componentType))return;

            var _comps = _compsDict[componentType];
            int index = _comps.FindIndex(comp=>comp.EntityUID==entityUID);

            if(index==-1)return;
            int lastIndex = _comps.Count-1;
            _comps[index]=_comps[lastIndex];
            _comps.RemoveAt(lastIndex);           
        }

        public bool FindComponent<T>(Predicate<T> match,out T comp) where T:struct,IComponent
        {
            var compType = ComponentHelper.GetType<T>();
            if(_compsDict.ContainsKey(compType))
            {
                comp = new T();
                return false;
            }
            foreach (var c in _compsDict[compType])
            {
                var t_c = (T)c;
                if(match(t_c))
                {
                    comp = t_c;
                    return true;
                }
            }
            comp = new T();
            return false;
        }

        public void OnUpdate()
        {
            if(Pause)return;
            foreach (var sys in _systems)
            {
                if(sys.Enabled)sys.OnUpdate();
            }
        }


        private void addOrSetResultCache(List<IComponent> resultCache,IComponent comp,int index)
        {
            if(index<resultCache.Count)
            {
                resultCache[index] = comp;
            }
            else
            {
                resultCache.Add(comp);
            }
        }

        private List<IComponent> getFilterCache(int compType)
        {
            if(!_filterCacheDict.ContainsKey(compType))_filterCacheDict.Add(compType,new List<IComponent>());
            _filterCacheDict[compType].Clear();
            return _filterCacheDict[compType];
        }

        public bool GetComponent(int entityUID,int compType,out IComponent comp)
        {
            comp = null;
            if(!_compsDict.ContainsKey(compType))return false;
            var comps = _compsDict[compType];
            for (int i = 0; i < _compsDict[compType].Count; i++)
            {
                if(comps[i].EntityUID==entityUID)
                {
                    comp = comps[i];
                    return true;
                }
            }
            return false;
        }

        public Archetype GetArchetype(int entityUID)
        {
            var archetype = new Archetype();
            foreach (var kv in _compsDict)
            {
                if(kv.Value.Exists(comp=>comp.EntityUID==entityUID))
                {
                    archetype.Add(kv.Key);
                }
            }
            return archetype;
        }

        public void SetComponents<T>(NativeArray<T> nativeArray) where T:struct,IComponent
        {
            int length = nativeArray.Length;
            for (int i = 0; i < length; i++)
            {
                var comp = nativeArray[i];
                AddOrSetComponent(comp.EntityUID,comp);
            }
        }

        public int CreateEntity()
        {
            _createEntityIndex++;
            return _createEntityIndex;
        }

        public void DestroyEntity(int entityUID)
        {
            var compTypes = _compsDict.Keys;
            foreach (var compType in compTypes)
            {
                RemoveComponent(entityUID,compType);
            }
        }


        public void Filter(Archetype archetype,Action<List<IComponent>[]> callback)
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
            // Debug.Log($"filter {minCountType} {compTypeCount}");
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
        }

    }
}