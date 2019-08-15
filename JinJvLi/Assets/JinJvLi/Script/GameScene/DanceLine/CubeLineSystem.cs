using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace JinJvli.DanceLine
{
    public class CubeLineSystem : ComponentSystem
    {
        EntityQuery group;
        protected override void OnCreate()
        {
            group = GetEntityQuery(typeof(RenderMesh),typeof(CubeLine),typeof(CubeLineGrow));
        }
        protected override void OnUpdate()
        {
            var entitys = group.ToEntityArray(Allocator.TempJob);
            Debug.Log(entitys.Length);
            var lines = group.ToComponentDataArray<CubeLine>(Allocator.TempJob);
            var grows = group.ToComponentDataArray<CubeLineGrow>(Allocator.TempJob);
            for (int i = 0; i < entitys.Length; i++)
            {
                var render = EntityManager.GetSharedComponentData<RenderMesh>(entitys[i]);
                Debug.LogWarning(render.material);
            }
            entitys.Dispose();
            lines.Dispose();
            grows.Dispose();
        }
    }
}