using Unity.Entities;
using Unity.Mathematics;

namespace JinJvli.DanceLine
{
    public struct CubeLine : IComponentData
    {
        public float3 StartPoint;
        public float3 EndPoint;
        public const float WIDTH=1;
    }

    public struct CubeLineGrow : IComponentData
    {
        public CubeDirection Direction;
        public float GrowSpeed;
    }

    public enum CubeDirection
    {
        Forward,Right
    }
}