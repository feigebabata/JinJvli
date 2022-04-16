using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;
using System;

namespace FGUFW.Core
{
    public struct NativeQueue<V>:IDisposable where V:struct
    {
        private const int MAX_NEW_EXPAND = 32;
        public const int DEF_CAPACITY = 8;
        public bool IsCreated{get;private set;}
        private NativeArray<V> _queue;
        private int _capacity;
        private int _firstIndex,_lastIndex;
        private Allocator _allocator;

        public int Length{get;private set;}

        /// <summary>
        /// 不要使用无参的构造函数
        /// </summary>
        /// <param name="capacity"></param>
        public NativeQueue(int capacity,Allocator allocator)
        {
            _queue = new NativeArray<V>(capacity,allocator);
            _capacity = capacity;
            _firstIndex = -1;
            _lastIndex = -1;
            _allocator = allocator;
            Length = 0;
            IsCreated = true;
        }

        public void Enqueue(V val)
        {
            if(_capacity==Length)
            {
                expansCapacity();
            }
            if(Length==0)
            {
                _firstIndex=0;
                _lastIndex=0;
            }
            else
            {
                _lastIndex++;
                if(_lastIndex==_capacity)_lastIndex=0;
            }
            _queue[_lastIndex] = val;
            Length++;
        }

        public V Dequeue()
        {
            if(Length==0)
            {
                throw new IndexOutOfRangeException("队列已空");
            }
            V val = _queue[_firstIndex];
            _firstIndex++;
            if(_firstIndex==_capacity)_firstIndex=0;
            Length--;
            return val;
        }

        public V Peek()
        {
            if(Length==0)
            {
                throw new IndexOutOfRangeException("队列已空");
            }
            V val = _queue[_firstIndex];
            return val;
        }

        private void expansCapacity()
        {
            int capacity = _capacity*2;
            if(capacity>MAX_NEW_EXPAND)capacity=MAX_NEW_EXPAND+_capacity;
            
            var queue = new NativeArray<V>(capacity,_allocator);

            if(_lastIndex>_firstIndex)
            {
                NativeArray<V>.Copy(_queue,queue,_capacity);
            }
            else
            {
                int length = _capacity - _firstIndex;
                NativeArray<V>.Copy(_queue,_firstIndex,queue,0,length);
                NativeArray<V>.Copy(_queue,0,queue,length,_lastIndex+1);
            }
            
            _queue.Dispose();

            _firstIndex = 0;
            _lastIndex = _capacity-1;
            _queue = queue;
            _capacity = capacity;
        }


        public void Dispose()
        {
            _capacity=0;
            Length=0;
            _queue.Dispose();
            IsCreated=false;
        }

        public void Clear()
        {
            Length=0;
            _firstIndex = -1;
            _lastIndex = -1;
        }

    }


}
