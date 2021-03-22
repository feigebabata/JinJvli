
using System;
using UnityEngine;

namespace FGUFW.Core
{
    public interface IPlayerInput : ISystem
    {
        Action<Vector2> OnMove{get;set;}
        Action OnClickA{get;set;}
        Action OnClickB{get;set;}
        Action OnClickBack{get;set;}
        void LateUpdate();
    }
}