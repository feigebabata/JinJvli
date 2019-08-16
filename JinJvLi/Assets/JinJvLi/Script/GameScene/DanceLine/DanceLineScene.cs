using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using JinJvli.DanceLine;
using JinJvli;

public class DanceLineScene : MonoBehaviour
{
    public static class Config
    {
        public const int SAMPLES_RATE=10;
        public const float MIN_ROAD_LENGTH=2;
        public const float MOVE_SPEED=2;
        public const float ROAD_SHOW_RADIUS=10;
    }

    RoadData[] m_roadDatas;
    int m_curIndex=0;
    Vector2Int m_curRoadIndexRange = Vector2Int.down;
    float3 m_roadEndPos=float3.zero;


    [SerializeField]
    Mesh m_cubeMesh;
    [SerializeField]
    Material m_roadMate;
    [SerializeField]
    Material m_lineMate;

    // Start is called before the first frame update
    void Start()
    {
        // var entityMng = World.Active.EntityManager;
        // NativeArray<Entity> entitys = new NativeArray<Entity>(1,Allocator.Temp);
        // var archetype = entityMng.CreateArchetype(typeof(Translation),typeof(RenderMesh),typeof(LocalToWorld),typeof(CubeLineGrow),typeof(CubeLine));
        // entityMng.CreateEntity(archetype,entitys);
        // Mesh cubeMesh = Instantiate(m_mesh);
        // entityMng.SetSharedComponentData<RenderMesh>(entitys[0],new RenderMesh(){mesh=cubeMesh,material=m_mate});
        // entityMng.SetComponentData<CubeLineGrow>(entitys[0],new CubeLineGrow(){Direction=CubeLineDirection.Forward,GrowSpeed=1});
        // Entity entity = entitys[0];
        // entitys.Dispose();
        // yield return new WaitForSeconds(3);
        // entityMng.RemoveComponent<CubeLineGrow>(entity);
        var music = Resources.Load<AudioClip>("DanceLine/深眠");
        music.GetSamples((int)music.length*Config.SAMPLES_RATE,createReodData).Start();
    }

    void createReodData(float[] _roadData)
    {
        List<RoadData> roadDaras = new List<RoadData>();
        int index=0;
        CubeDirection direction=CubeDirection.Forward;
        for (int i = 0; i < _roadData.Length; i++)
        {
            if(i>1 && i<_roadData.Length-1)
            {
                float length = ((float)i-(float)index)/Config.SAMPLES_RATE;
                if(_roadData[i]>_roadData[i-1] && _roadData[i]>_roadData[i+1] && length>Config.MIN_ROAD_LENGTH)
                {
                    roadDaras.Add(new RoadData(){Direction=direction,Length=length,Width=UnityEngine.Random.Range(RoadData.MIN_WIDTH,RoadData.MAX_WIDTH),Height=UnityEngine.Random.Range(RoadData.MIN_HEIGHR,RoadData.MAX_HEIGHR)});
                    if(direction==CubeDirection.Forward)
                    {
                        direction=CubeDirection.Right;
                    }
                    else
                    {
                        direction=CubeDirection.Forward;
                    }
                    index=i;
                }

            }
        }
        m_roadDatas = roadDaras.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        createRoadUpdate();
    }

    void createRoadUpdate()
    {
        if(m_roadDatas!= null)
        {
            while (m_curRoadIndexRange.y<m_curIndex+Config.ROAD_SHOW_RADIUS)
            {
                m_curRoadIndexRange.y++;
                RoadData data = m_roadDatas[m_curRoadIndexRange.y];
                var entity = World.Active.EntityManager.CreateEntity(typeof(RenderMesh),typeof(LocalToWorld),typeof(Translation),typeof(RoadCube));

                World.Active.EntityManager.SetComponentData<Translation>(entity,new Translation(){Value=m_roadEndPos});

                RoadCube roadCube = new RoadCube();
                if(data.Direction == CubeDirection.Forward)
                {
                    roadCube.Size.w = m_roadEndPos.z;
                    roadCube.Size.y = m_roadEndPos.z+data.Length*DanceLineScene.Config.MOVE_SPEED;
                    roadCube.Size.z = m_roadEndPos.x-data.Width/2;
                    roadCube.Size.x = m_roadEndPos.x+data.Width/2;
                    m_roadEndPos.z+=data.Length*DanceLineScene.Config.MOVE_SPEED;
                }
                else
                {
                    roadCube.Size.z = m_roadEndPos.x;
                    roadCube.Size.x = m_roadEndPos.x+data.Length*DanceLineScene.Config.MOVE_SPEED;
                    roadCube.Size.y = m_roadEndPos.z+data.Width/2;
                    roadCube.Size.w = m_roadEndPos.z-data.Width/2;
                    m_roadEndPos.x+=data.Length*DanceLineScene.Config.MOVE_SPEED;
                }

                World.Active.EntityManager.SetComponentData<RoadCube>(entity,roadCube);

                var mesh = GameObject.Instantiate(m_cubeMesh);
                var vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].y-=0.5f;
                    vertices[i].y*=data.Height;
                    if(data.Direction == CubeDirection.Forward)
                    {
                        vertices[i].z+=0.5f;
                        vertices[i].z*=data.Length*DanceLineScene.Config.MOVE_SPEED;
                        vertices[i].x*=data.Width;
                    }
                    else
                    {
                        vertices[i].x+=0.5f;
                        vertices[i].x*=data.Length*DanceLineScene.Config.MOVE_SPEED;
                        vertices[i].z*=data.Width;
                    }
                }
                mesh.vertices= vertices;
                World.Active.EntityManager.SetSharedComponentData<RenderMesh>(entity,new RenderMesh(){mesh=mesh,material=m_roadMate});
            }
        }
    }
}
