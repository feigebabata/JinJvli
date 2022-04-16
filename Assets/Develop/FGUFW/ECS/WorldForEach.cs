
using System;
using System.Collections.Generic;
using Unity.Collections;

namespace FGUFW.ECS
{
    public sealed partial class World
    {
        public void ForEach<T0>(R<T0> callback)
        where T0:struct,IComponent
        {
            var t0_Type = ComponentHelper.GetType<T0>();
        }
    }

    public delegate void R<T0>(ref T0 t0);
    public delegate void V<T0>(T0 t0);
    public delegate void I<T0>(in T0 t0);
    public delegate void VV<T0,T1>(T0 t0,T1 t1);
    public delegate void VR<T0,T1>(T0 t0,ref T1 t1);
    public delegate void VI<T0,T1>(T0 t0,in T1 t1);
    public delegate void RV<T0,T1>(ref T0 t0,T1 t1);
    public delegate void RR<T0,T1>(ref T0 t0,ref T1 t1);
    public delegate void RI<T0,T1>(ref T0 t0,in T1 t1);
    public delegate void IV<T0,T1>(in T0 t0,T1 t1);
    public delegate void IR<T0,T1>(in T0 t0,ref T1 t1);
    public delegate void II<T0,T1>(in T0 t0,in T1 t1);

}