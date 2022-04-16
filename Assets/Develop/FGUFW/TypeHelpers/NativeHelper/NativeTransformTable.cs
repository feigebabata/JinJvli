using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;
using System;
using UnityEngine;

namespace FGUFW.Core
{
    public struct NativeTransformTable:IDisposable
    {
        private const int MAX_NEW_EXPAND = 32;
        public const int DEF_CAPACITY = 8;
        public bool IsCreated{get;private set;}

        private NativeArray<int> _keys;
        private TransformAccessArray _vals;
        private int _capacity;
        private Allocator _allocator;

        public int Length{get;private set;}

        public TransformAccessArray AccessArray => _vals;

        public Transform this[int k]
        {
            get
            {
                int index = indexOfKeys(k);
                if(index == -1)
                {
                    return null;
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

        public int GetKey(int index)
        {
            return _keys[index];
        }


        /// <summary>
        /// 不要使用无参的构造函数
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <param name="desiredJobCount">期望的Job数量</param>
        public NativeTransformTable(int capacity,Allocator allocator, int desiredJobCount = -1)
        {
            _keys = new NativeArray<int>(capacity,allocator);
            _vals = new TransformAccessArray(capacity);
            _capacity = capacity;
            _allocator = allocator;
            Length = 0;
            IsCreated = true;
        }


        private void add(int k,Transform val)
        {
            if(_capacity==Length)
            {
                expansCapacity();
            }
            _keys[Length] = k;
            _vals.Add(val);
            Length++;
        }


        private void expansCapacity()
        {
            int capacity = _capacity*2;
            if(capacity>MAX_NEW_EXPAND)capacity=MAX_NEW_EXPAND+_capacity;
            
            var keys = new NativeArray<int>(capacity,_allocator);

            NativeArray<int>.Copy(_keys,keys,_capacity);
            
            _keys.Dispose();

            _keys = keys;
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

        public void Foreach(Action<int,Transform> callback)
        {
            for (int i = 0; i < Length; i++)
            {
                callback(_keys[i],_vals[i]);
            }
        }

        public bool ContainsKey(int key)=>indexOfKeys(key) != -1;


        public void Dispose()
        {
            _capacity=0;
            Length=0;
            _keys.Dispose();
            _vals.Dispose();
            IsCreated=false;
        }

    }


}
