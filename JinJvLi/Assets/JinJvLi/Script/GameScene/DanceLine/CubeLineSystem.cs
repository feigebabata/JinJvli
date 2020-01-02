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
            float offset=0;
            for (int i = 0; i < entitys.Length; i++)
            {
                offset=grows[i].GrowSpeed*Time.DeltaTime;

                if(grows[i].Direction==CubeDirection.Forward)
                {
                    CubeLine tempLine = lines[i];
                    tempLine.EndPoint.z+=offset;
                    EntityManager.SetComponentData<CubeLine>(entitys[i],tempLine);
                }
                else
                {
                    CubeLine tempLine = lines[i];
                    tempLine.EndPoint.x+=offset;
                    EntityManager.SetComponentData<CubeLine>(entitys[i],tempLine);
                }

                var render = EntityManager.GetSharedComponentData<RenderMesh>(entitys[i]);
                if(grows[i].Direction==CubeDirection.Forward)
                {
                    var vertices = render.mesh.vertices;
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        if(vertices[j].z>0)
                        {
                            vertices[j].z +=offset;
                        }
                    }
                    render.mesh.vertices = vertices;
                }
                else
                {
                    var vertices = render.mesh.vertices;
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        if(vertices[j].x>0)
                        {
                            vertices[j].x +=offset;
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