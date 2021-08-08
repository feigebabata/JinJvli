using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class LogicFrame 
    {
        public PB_Frame[] Frames;
        public bool Complete => !Array.Exists<PB_Frame>(Frames,f=>{return f==null;}); 
    }
}
