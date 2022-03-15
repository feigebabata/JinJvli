using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace FGUFW.ECS
{
    public sealed partial class World
    {
        public const int ENTITY_NONE = 0;
        public const int ENTITY_SINGLE = -1;

        private int _createEntityIndex=0;
        private List<ISystem> _systems = new List<ISystem>();
        private Dictionary<int,List<IComponent>> _compsDict = new Dictionary<int, List<IComponent>>();
        private Dictionary<int,List<IComponent>> _filterCacheDict = new Dictionary<int, List<IComponent>>();
        public float Time{get;private set;}
        public int FrameIndex{get;private set;}
        public float DeltaTime{get;private set;}
        public float TimeScale=1;
        public bool Pause=false;


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

        public bool GetComponent<T>(int entityUID,out T comp) where T:struct,IComponent
        {
            comp = default(T);
            var compType = ComponentHelper.GetType<T>();
            if(!_compsDict.ContainsKey(compType))return false;
            var comps = _compsDict[compType];
            for (int i = 0; i < _compsDict[compType].Count; i++)
            {
                if(comps[i].EntityUID==entityUID)
                {
                    comp = (T)comps[i];
                    return true;
                }
            }
            return false;
        }

        public bool GetComponent<T>(Predicate<T> match,out T comp) where T:struct,IComponent
        {
            var compType = ComponentHelper.GetType<T>();
            if(_compsDict.ContainsKey(compType))
            {
                comp = default(T);
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
            comp = default(T);
            return false;
        }

        public NativeArray<T> GetComponents<T,V>(NativeArray<V> targetEntitys) where V:struct,ITargetEntity,IComponent where T:struct,IComponent
        {
            var targetType = ComponentHelper.GetType<V>();
            if(!_compsDict.ContainsKey(targetType))return default(NativeArray<T>);

            int length = targetEntitys.Length;
            var compType = ComponentHelper.GetType<T>();
            NativeArray<T> result = new NativeArray<T>(length,Allocator.TempJob);
            for (int i = 0; i < length; i++)
            {
                var entityUID = targetEntitys[i].EntityUID;
                var comps = _compsDict[compType];
                for (int j = 0; j < comps.Count; j++)
                {
                    if(comps[j].EntityUID==entityUID)
                    {
                        result[i] = (T)comps[j];
                        break;
                    }
                }
            }
            return result;
        }

        public void OnUpdate(float deltaTime)
        {
            if(Pause || TimeScale==0f)return;
            DeltaTime = TimeScale * deltaTime;
            Time += deltaTime;
            foreach (var sys in _systems)
            {
                if(sys.Enabled)sys.OnUpdate();
            }
            FrameIndex++;
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

        /// <summary>
        /// 实体唯一索引 0:未知 -1:单例
        /// </summary>
        /// <returns>EntityUID</returns>
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



    }
}