using Unity.Entities;
using Unity.Mathematics;

namespace JinJvli.DanceLine
{
    public struct CubeLine : IComponentData
    {
        public float3 StartPoint;
        public float3 EndPoint;
    }

    public struct CubeLineGrow : IComponentData
    {
        public CubeLineDirection Direction;
        public float GrowSpeed;
    }

    public enum CubeLineDirection
    {
        Forward,Right
    }
}