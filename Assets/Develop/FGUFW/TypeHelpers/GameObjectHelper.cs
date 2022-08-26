using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    static public class GameObjectHelper
    {
        static public GameObject Copy(this GameObject self,Transform parent)
        {
            return GameObject.Instantiate(self,parent);
        }
    }
}