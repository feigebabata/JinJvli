using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    static public class NumberHelper
    {
        static public int Ceil(this Single self)
        {
            return Mathf.CeilToInt(self);
        }

        static public int ToInt32(this Single self)
        {
            return (int)self;
        }
    }
}