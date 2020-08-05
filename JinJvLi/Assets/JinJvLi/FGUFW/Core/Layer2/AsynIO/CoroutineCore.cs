using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FGUFW.Core
{
	public class CoroutineCore:MonoSingleton<CoroutineCore> 
	{
		public enum IOStatus
		{
			None,
			Wait,
			Runing,
		}

		static public class Config
		{
			public const int MAX_IO_COUNT=10;
		}


		Dictionary<int,IEnumerator> _waits = new Dictionary<int, IEnumerator>();
		Dictionary<int,Coroutine> _runing = new Dictionary<int, Coroutine>();
		int _lastID=int.MinValue;

		public int StartIO(IEnumerator ie)
		{
			int id = getNewID();
			if(_runing.Count>Config.MAX_IO_COUNT)
			{
				_waits.Add(id,ie);
			}
			else
			{
				var cor = StartCoroutine(ioCoroutine(ie,id));
				_runing.Add(id,cor);
			}
			return id;
		}

		public void StopIO(int id)
		{
			if(_waits.ContainsKey(id))
			{
				_waits.Remove(id);
			}
			else if(_runing.ContainsKey(id))
			{
				var cor = _runing[id];
				StopCoroutine(cor);
				_runing.Remove(id);
				resetIOWait();
			}
		}

		public IOStatus GetIOState(int id)
		{
			IOStatus state=IOStatus.None;
			if(_waits.ContainsKey(id))
			{
				state=IOStatus.Wait;
			}
			else if(_runing.ContainsKey(id))
			{
				state=IOStatus.Runing;
			}
			return state;
		}

		public void Clear()
		{
			StopAllCoroutines();
			_waits.Clear();
			_runing.Clear();
		}

		IEnumerator ioCoroutine(IEnumerator ie,int id)
		{
			yield return ie;
			yield return null;
			_runing.Remove(id);
			resetIOWait();
		}

		void resetIOWait()
		{
			if(_waits.Count>0 && _runing.Count<Config.MAX_IO_COUNT)
			{
				var keySort = from kv in _waits orderby kv.Key select kv;
				int id = keySort.First().Key;
				
				var ie = _waits[id];
				_waits.Remove(id);
			}
		}

		int getNewID()
		{
			if(_lastID==int.MaxValue)
			{
				_lastID=int.MinValue;
			}
			_lastID++;
			return _lastID;
		}
		
	}
}
