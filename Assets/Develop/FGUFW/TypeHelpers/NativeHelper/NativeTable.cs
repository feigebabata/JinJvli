using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;
using System;

namespace FGUFW.Core
{
    public struct NativeTable<V>:IDisposable where V:struct
    {
        private const int MAX_NEW_EXPAND = 32;
        public const int DEF_CAPACITY = 8;
        public bool IsCreated{get;private set;}

        private NativeArray<int> _keys;
        private NativeArray<V> _vals;
        private int _capacity;
        private Allocator _allocator;

        public int Length{get;private set;}

        public V this[int k]
        {
            get
            {
                int index = indexOfKeys(k);
                if(index == -1)
                {
                    return default(V);
                }
                else
                {
                    return _vals[index];
                }
            }
            set
            {
                int index = indexOfKeys(k);
                if(index == -1)
                {
                    add(k,value);
                }
                else
                {
                    _vals[index] = value;
                }
            }
        }

        /// <summary>
        /// 不要使用无参的构造函数
        /// </summary>
        /// <param name="capacity"></param>
        public NativeTable(Allocator allocator,int capacity=DEF_CAPACITY)
        {
            _keys = new NativeArray<int>(capacity,allocator);
            _vals = new NativeArray<V>(capacity,allocator);
            _capacity = capacity;
            _allocator = allocator;
            Length = 0;
            IsCreated = true;
        }


        private void add(int k,V val)
        {
            if(_capacity==Length)
            {
                expansCapacity();
            }
            _keys[Length] = k;
            _vals[Length] = val;
            Length++;
        }


        private void expansCapacity()
        {
            int capacity = _capacity*2;
            if(capacity>MAX_NEW_EXPAND)capacity=MAX_NEW_EXPAND+_capacity;
            
            var keys = new NativeArray<int>(capacity,_allocator);
            var vals = new NativeArray<V>(capacity,_allocator);

            NativeArray<int>.Copy(_keys,keys,_capacity);
            NativeArray<V>.Copy(_vals,vals,_capacity);
            
            _keys.Dispose();
            _vals.Dispose();

            _keys = keys;
            _vals = vals;
            _capacity = capacity;

        }

        public void Remove(int k)
        {
            int index = indexOfKeys(k);
            if(index == -1)return;

            _keys[index] = _keys[Length-1];
            _vals[index] = _vals[Length-1];
            Length--;
        }

        public void Remove(Predicate<V> match)
        {
            int k;
            if(FindKey(match,out k))
            {
                Remove(k);
            }
        }

        private int indexOfKeys(int key)
        {
            int index = -1;
            for (int i = 0; i < Length; i++)
            {
                if(_keys[i]==key)
                {
                    index=i;
                    break;
                }
            }
            return index;
        }

        public void Foreach(Action<int,V> callback)
        {
            for (int i = 0; i < Length; i++)
            {
                callback(_keys[i],_vals[i]);
            }
        }

        public bool ContainsKey(int key)=>indexOfKeys(key) != -1;
        
        public bool ContainsValue(Predicate<V> match)
        {
            for (int i = 0; i < Length; i++)
            {
                if(match(_vals[i]))return true;
            }
            return false;
        }

        public bool FindKey(Predicate<V> match,out int key)
        {
            for (int i = 0; i < Length; i++)
            {
                if(match(_vals[i]))
                {
                    key = _keys[i];
                    return true;
                }
            }
            key = 0;
            return false;
        }


        public void Dispose()
        {
            _capacity=0;
            Length=0;
            _keys.Dispose();
            _vals.Dispose();
            IsCreated=false;
        }

        public void Clear()
        {
            Length=0;
        }

    }


}
