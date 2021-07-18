using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.GameLobby
{
    [CreateAssetMenu()]
    public class GameItemDatas : ScriptableObject,IEnumerable<GameItemData>
    {
        [SerializeField]
        private GameItemData[] _datas;
        
        public GameItemData this[int id]
        {
            get
            {
                return Array.Find<GameItemData>(_datas,gd=>{return gd.ID==id;});
            }
        }
        
        public GameItemData this[string pmTypeName]
        {
            get
            {
                return Array.Find<GameItemData>(_datas,gd=>{return gd.TypeName==pmTypeName;});
            }
        }

        public IEnumerator<GameItemData> GetEnumerator()
        {
            for (int i = 0; i < _datas.Length; i++)
            {
                yield return _datas[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _datas.Length; i++)
            {
                yield return _datas[i];
            }
        }

        public int Count => _datas.Length;
    }

    [System.Serializable]
    public class GameItemData
    {
        public int ID;
        public int PlayerMaxCount=1;
        public string Name;
        public string TypeName;
        public Sprite Icon;
        public Vector3 Scale;
        public RuntimePlatform[] Platform;
    }
}
