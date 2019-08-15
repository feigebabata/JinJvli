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
    void Start()
    {
        var entityMng = World.Active.EntityManager;
        NativeArray<Entity> entitys = new NativeArray<Entity>(1,Allocator.Temp);
        var archetype = entityMng.CreateArchetype(typeof(Translation),typeof(RenderMesh),typeof(LocalToWorld),typeof(CubeLineGrow),typeof(CubeLine));
        entityMng.CreateEntity(archetype,entitys);
        Mesh cubeMesh = Instantiate(m_mesh);
        Vector3[] vertices = cubeMesh.vertices;
        for (int i = 0; i < cubeMesh.vertices.Length; i++)
        {
            if(cubeMesh.vertices[i].z==0.5f)
            {
                vertices[i].z +=2;
            }
        }
        cubeMesh.vertices=vertices;
        entityMng.SetSharedComponentData<RenderMesh>(entitys[0],new RenderMesh(){mesh=cubeMesh,material=m_mate});
        entityMng.SetComponentData<CubeLineGrow>(entitys[0],new CubeLineGrow(){Direction=CubeLineDirection.Forward,GrowSpeed=1});
        entitys.Dispose();
        Debug.Log("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
