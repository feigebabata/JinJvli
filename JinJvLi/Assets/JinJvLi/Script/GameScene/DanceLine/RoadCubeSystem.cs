using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace JinJvli.DanceLine
{
    public class RoadCubeSystem : ComponentSystem
    {
        private EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(typeof(RoadCube),typeof(RenderMesh));
        }
        protected override void OnUpdate()
        {
            deleteRoadUpdate();
        }

        void deleteRoadUpdate()
        {
            var entitys = m_query.ToEntityArray(Allocator.TempJob);
            var roads = m_query.ToComponentDataArray<RoadCube>(Allocator.TempJob);
            for (int i = 0; i < entitys.Length; i++)
            {
                var render = EntityManager.GetSharedComponentData<RenderMesh>(entitys[i]);
                
            }
            entitys.Dispose();
            roads.Dispose();
        }
    }
}