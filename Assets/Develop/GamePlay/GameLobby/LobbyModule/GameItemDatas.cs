using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.GameLobby
{
    [CreateAssetMenu()]
    public class GameItemDatas : ScriptableObject
    {
        public GameItemData[] Datas;
        
    }

    [System.Serializable]
    public struct GameItemData
    {
        public string TypeName;
        public Sprite Icon;
        public Vector3 Scale;
        public bool AndroidPlatform;
        public bool PCPlatform;
        public bool WebPlatform;
    }
}
