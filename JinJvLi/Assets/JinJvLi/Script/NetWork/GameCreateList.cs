using System;
using UnityEngine;

namespace JinJvli
{
    [Serializable]
    public class CreateGameInfo
    {
        public string Name;
        public UInt32 ID;
    } 

    [CreateAssetMenu(menuName="GameCreateList")]
    public class GameCreateList : ScriptableObject
    {
        public CreateGameInfo[] List;
    }
}