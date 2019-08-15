using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace JinJvli.DanceLine
{
    public class CubeLineSystem : ComponentSystem
    {
        EntityQuery query;
        protected override void OnCreate()
        {
            query = GetEntityQuery(typeof(CubeLine),typeof(RenderMesh),typeof(CubeLineGrow));
        }
        protected override void OnUpdate()
        {
            var entitys = query.ToEntityArray(Allocator.TempJob);
            var lines = query.ToComponentDataArray<CubeLine>(Allocator.TempJob);
            var grows = query.ToComponentDataArray<CubeLineGrow>(Allocator.TempJob);
            for (int i = 0; i < entitys.Length; i++)
            {
                var render = EntityManager.GetSharedComponentData<RenderMesh>(entitys[i]);
                if(grows[i].Direction==CubeLineDirection.Forward)
                {
                    Debug.Log("*");
                    var vertices = render.mesh.vertices;
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        if(vertices[j].z>0)
                        {
                            vertices[j].z +=grows[i].GrowSpeed*Time.deltaTime;
                        }
                    }
                    render.mesh.vertices = vertices;
                }
            }
            entitys.Dispose();
            lines.Dispose();
            grows.Dispose();
        }
    }
}