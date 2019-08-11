using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public class DanceLineScene : MonoBehaviour
{
    [SerializeField]
    GameObject m_cube;

    // Start is called before the first frame update
    void Start()
    {
        var entityMng = World.Active.EntityManager;
        NativeArray<Entity> entitys = new NativeArray<Entity>(1,Allocator.Temp);
        var archetype = entityMng.CreateArchetype(typeof(Translation));
        entityMng.Instantiate(m_cube,entitys);
        entityMng.SetComponentData<Translation>(entitys[0],new Translation(){Value=new float3(0,0,0)});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
