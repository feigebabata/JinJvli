using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

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
        var archetype = entityMng.CreateArchetype(typeof(Translation),typeof(RenderMesh),typeof(LocalToWorld));
        entityMng.CreateEntity(archetype,entitys);
        entityMng.SetComponentData<Translation>(entitys[0],new Translation(){Value=new float3(3,0,0)});
        Vector3[] vertices = new Vector3[m_mesh.vertexCount];
        for (int i = 0; i < m_mesh.vertices.Length; i++)
        {
            if(m_mesh.vertices[i].x==0.5f)
            {
                vertices[i] = new Vector3(2,m_mesh.vertices[i].y,m_mesh.vertices[i].z);
            }
            else
            {
                vertices[i]=m_mesh.vertices[i];
            }
        }
        m_mesh.vertices=vertices;
        entityMng.SetSharedComponentData<RenderMesh>(entitys[0],new RenderMesh(){mesh=m_mesh,material=m_mate});
        Debug.Log("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
