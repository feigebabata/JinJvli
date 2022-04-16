using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace FGUFW.ECS
{
    public sealed partial class World:IDisposable
    {
        /// <summary>
        /// 无效实体UID
        /// </summary>
        public const int ENTITY_NONE = 0;

        /// <summary>
        /// 存单例组件的实体
        /// </summary>
        public const int ENTITY_SINGLE = -1;

        /// <summary>
        /// 实体索引 依次加一
        /// </summary>
        private int _createEntityIndex=0;
        private List<ISystem> _systems = new List<ISystem>();

        /// <summary>
        /// 按组件类型存储的组件集
        /// </summary>
        /// <returns></returns>
        private Dictionary<int,List<IComponent>> _compsDict = new Dictionary<int, List<IComponent>>();
        
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

        public void Dispose()
        {
            foreach (var kv in _compsDict)
            {
                foreach (var item in kv.Value)
                {
                    item.Dispose();
                }
            }
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

        /// <summary>
        /// 添加和修改单个实体的组件 枚举组件别用这个
        /// </summary>
        /// <param name="entityUID"></param>
        /// <param name="component"></param>
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

        /// <summary>
        /// 移除实体的组件
        /// </summary>
        /// <param name="entityUID"></param>
        /// <param name="componentType"></param>
        public void RemoveComponent(int entityUID,int componentType)
        {
            if(!_compsDict.ContainsKey(componentType))return;

            var _comps = _compsDict[componentType];
            int index = _comps.FindIndex(comp=>comp.EntityUID==entityUID);

            if(index==-1)return;
            int lastIndex = _comps.Count-1;
            _comps[index].Dispose();
            _comps[index]=_comps[lastIndex];
            _comps.RemoveAt(lastIndex);           
        }

        /// <summary>
        /// 获取单个实体的组件
        /// </summary>
        /// <param name="entityUID"></param>
        /// <param name="compType"></param>
        /// <param name="comp"></param>
        /// <returns>存在返回true</returns>
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

        /// <summary>
        /// 获取实体的组件 在实体必包含这个组建的条件下使用
        /// </summary>
        public T GetComponent<T>(int entityUID) where T:struct,IComponent
        {
            var compType = ComponentHelper.GetType<T>();
            if(!_compsDict.ContainsKey(compType))return default(T);
            var comps = _compsDict[compType];
            for (int i = 0; i < _compsDict[compType].Count; i++)
            {
                if(comps[i].EntityUID==entityUID)
                {
                    return (T)comps[i];
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取实体满足匹配条件的组件
        /// </summary>
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

        /// <summary>
        /// 获取ITargetEntity集合中指定实体的T类型组件集合
        /// </summary>
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


        /// <summary>
        /// 获取同个类型的所有组件 非拷贝 不要在遍历时删除
        /// </summary>
        public List<IComponent> GetAllComponent<T>() where T:struct,IComponent
        {
            var compType = ComponentHelper.GetType<T>();
            return GetAllComponent(compType);
        }

        /// <summary>
        /// 获取同个类型的所有组件 非拷贝 不要在遍历时删除
        /// </summary>
        /// <param name="compType"></param>
        /// <returns></returns>
        public List<IComponent> GetAllComponent(int compType)
        {
            if(!_compsDict.ContainsKey(compType))return null;
            return _compsDict[compType];
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

        /// <summary>
        /// 获取实体的原型
        /// </summary>
        /// <param name="entityUID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 批量同步Job中计算的数据 在Job外使用
        /// </summary>
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
        /// 设置枚举组件 会把同枚举的其他组件挤下去 非枚举组件别用
        /// </summary>
        /// <param name="enum_item"></param>
        /// <param name="comp"></param>
        /// <typeparam name="E"></typeparam>
        public void SetEnumComponent<E>(IComponent comp) where E:Enum
        {
            var compType = comp.Type;
            var entityUID = comp.EntityUID;
            foreach (E enum_item in Enum.GetValues(typeof(E)))
            {
                var t = ComponentHelper.GetEnumComponentType(enum_item);
                var ct = ComponentHelper.GetType(t);
                if(ct!=compType)
                {
                    RemoveComponent(entityUID,ct);
                }
            }
            AddOrSetComponent(entityUID,comp);
        }

        /// <summary>
        /// 移除同枚举的所有组件
        /// </summary>
        /// <param name="entityUID"></param>
        /// <typeparam name="E"></typeparam>
        public void RemoveEnumComponent<E>(int entityUID) where E:Enum
        {
            foreach (E enum_item in Enum.GetValues(typeof(E)))
            {
                var t = ComponentHelper.GetEnumComponentType(enum_item);
                var ct = ComponentHelper.GetType(t);
                RemoveComponent(entityUID,ct);
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