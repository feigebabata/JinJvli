using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace JinJvli.DanceLine
{
    public class CubeLineSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            Entities.WithAll(typeof(CubeLine),typeof(RenderMesh),typeof(CubeLineGrow));
        }
        protected override void OnUpdate()
        {
            Debug.Log("--");
            Entities.ForEach((RenderMesh _render,ref CubeLine _line,ref CubeLineGrow _crow)=>
            {
                Debug.Log("**");
            });
        }
    }
}