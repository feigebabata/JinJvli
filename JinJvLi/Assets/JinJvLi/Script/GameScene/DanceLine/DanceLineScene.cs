using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using JinJvli.DanceLine;

public class DanceLineScene : MonoBehaviour
{
    [SerializeField]
    Mesh m_mesh;
    [SerializeField]
    Material m_mate;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        var entityMng = World.Active.EntityManager;
        NativeArray<Entity> entitys = new NativeArray<Entity>(1,Allocator.Temp);
        var archetype = entityMng.CreateArchetype(typeof(Translation),typeof(RenderMesh),typeof(LocalToWorld),typeof(CubeLineGrow),typeof(CubeLine));
        entityMng.CreateEntity(archetype,entitys);
        Mesh cubeMesh = Instantiate(m_mesh);
        entityMng.SetSharedComponentData<RenderMesh>(entitys[0],new RenderMesh(){mesh=cubeMesh,material=m_mate});
        entityMng.SetComponentData<CubeLineGrow>(entitys[0],new CubeLineGrow(){Direction=CubeLineDirection.Forward,GrowSpeed=1});
        Entity entity = entitys[0];
        entitys.Dispose();
        yield return new WaitForSeconds(3);
        entityMng.RemoveComponent<CubeLineGrow>(entity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
