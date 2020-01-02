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
        public const int SAMPLES_RATE=8;
        public const float MIN_ROAD_LENGTH=2;
        public const float MOVE_SPEED=3;
        public const float ROAD_SHOW_RADIUS=100;
    }

    RoadData[] m_roadDatas;
    int m_curIndex=0;
    Vector2Int m_curRoadIndexRange = Vector2Int.down;
    float3 m_roadEndPos=float3.zero;
    Entity m_curCubeLineEntity;

    [SerializeField]
    Mesh m_cubeMesh;
    [SerializeField]
    Material m_roadMate;
    [SerializeField]
    Material m_lineMate;
    [SerializeField]
    Transform m_cameraRoot;
    [SerializeField]
    AudioSamples m_audioSamples;
    [SerializeField]
    AudioSource m_audioSource;

    bool m_isStart=false;

    // Start is called before the first frame update
    void Start()
    {
        createCubeLine(false);

        createReodData(m_audioSamples.Samples);
    }

    void createCubeLine(bool _isGrow=true)
    {

        var entityMng = World.Active.EntityManager;

        float3 endPos=float3.zero,startPos = float3.zero;
        var direction = CubeDirection.Forward;
        if(m_curCubeLineEntity != Entity.Null)
        {
            startPos = entityMng.GetComponentData<CubeLine>(m_curCubeLineEntity).EndPoint;
            if(entityMng.GetComponentData<CubeLineGrow>(m_curCubeLineEntity).Direction== CubeDirection.Forward)
            {
                direction =CubeDirection.Right;
                //startPos.z -= CubeLine.WIDTH / 2;
            }
            else
            {
                direction =CubeDirection.Forward;
                //startPos.x -= CubeLine.WIDTH / 2;
            }
            endPos = startPos;
            entityMng.RemoveComponent<CubeLineGrow>(m_curCubeLineEntity);
        }

        var archetype = entityMng.CreateArchetype(typeof(Translation),typeof(RenderMesh),typeof(LocalToWorld),typeof(CubeLineGrow),typeof(CubeLine));
        m_curCubeLineEntity = entityMng.CreateEntity(archetype);
        
        Mesh cubeMesh = Instantiate(m_cubeMesh);
        var vertices = cubeMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y+=0.5f;
            if (direction == CubeDirection.Forward)
            {
                //vertices[i].z += 0.5f;
            }
            else
            {
                //vertices[i].x += 0.5f;
            }
        }

        //if(direction == CubeDirection.Forward)
        //{
        //    endPos.z = startPos.z+0.5f;
        //}
        //else
        //{
        //    endPos.x = startPos.x+0.5f;
        //}


        cubeMesh.vertices = vertices;
        entityMng.SetSharedComponentData<RenderMesh>(m_curCubeLineEntity,new RenderMesh(){mesh=cubeMesh,material=m_lineMate});
        entityMng.SetComponentData<Translation>(m_curCubeLineEntity,new Translation(){Value=startPos});
        entityMng.SetComponentData<CubeLine>(m_curCubeLineEntity,new CubeLine(){StartPoint=startPos,EndPoint=endPos});

        if(_isGrow)
        {
            entityMng.SetComponentData<CubeLineGrow>(m_curCubeLineEntity,new CubeLineGrow(){Direction=direction,GrowSpeed=Config.MOVE_SPEED});
        }
        else
        {
            entityMng.RemoveComponent<CubeLineGrow>(m_curCubeLineEntity);
        }
        
    }

    void createReodData(float[] _roadData)
    {
        List<RoadData> roadDaras = new List<RoadData>();
        int index=0;
        CubeDirection direction=CubeDirection.Forward;
        roadDaras.Add(new RoadData() { Direction = direction, Length = _roadData[0], Width = 1 * Config.MOVE_SPEED, Height = 2 });
        direction = CubeDirection.Right;
        for (int i = 1; i < _roadData.Length; i++)
        {
            if(i>1 && i<_roadData.Length-1)
            {
                float length = _roadData[i] - _roadData[i-1];
                if(length>Config.MIN_ROAD_LENGTH)
                {
                    roadDaras.Add(new RoadData(){Direction=direction,Length=length,Width=1*Config.MOVE_SPEED,Height=2});
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
        if(Input.GetMouseButtonDown(0))
        {
            if(m_isStart)
            {
                createCubeLine();
            }
            else
            {
                m_audioSource.Play();
                m_isStart =true;
                World.Active.EntityManager.AddComponentData<CubeLineGrow>(m_curCubeLineEntity,new CubeLineGrow(){Direction=CubeDirection.Forward,GrowSpeed=Config.MOVE_SPEED});
            }
        }
    }

    void LateUpdate()
    {
        if(m_curCubeLineEntity!= Entity.Null)
        {
            float3 pos = World.Active.EntityManager.GetComponentData<CubeLine>(m_curCubeLineEntity).EndPoint;
            m_cameraRoot.position = pos;
        }
    }

    void createRoadUpdate()
    {
        if(m_roadDatas!= null)
        {
            if (m_curRoadIndexRange.y<m_curIndex+Config.ROAD_SHOW_RADIUS && m_curRoadIndexRange.y<m_roadDatas.Length-1)
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
                    m_roadEndPos.x-=data.Width/2;
                }
                else
                {
                    roadCube.Size.z = m_roadEndPos.x;
                    roadCube.Size.x = m_roadEndPos.x+data.Length*DanceLineScene.Config.MOVE_SPEED;
                    roadCube.Size.y = m_roadEndPos.z+data.Width/2;
                    roadCube.Size.w = m_roadEndPos.z-data.Width/2;
                    m_roadEndPos.x+=data.Length*DanceLineScene.Config.MOVE_SPEED;
                    m_roadEndPos.z-=data.Width/2;
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
                        if(vertices[i].z>0)
                        {
                            vertices[i].z += data.Length * DanceLineScene.Config.MOVE_SPEED;
                        }
                        vertices[i].x*=data.Width;
                    }
                    else
                    {
                        if (vertices[i].x>0)
                        {
                            vertices[i].x += data.Length * DanceLineScene.Config.MOVE_SPEED;
                        }
                        vertices[i].z*=data.Width;
                    }
                }
                mesh.vertices= vertices;
                World.Active.EntityManager.SetSharedComponentData<RenderMesh>(entity,new RenderMesh(){mesh=mesh,material=m_roadMate});
            }
        }
    }
}
