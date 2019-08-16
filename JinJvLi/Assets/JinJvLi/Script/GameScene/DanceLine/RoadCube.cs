using Unity.Entities;
using Unity.Mathematics;

namespace JinJvli.DanceLine
{
    public struct RoadCube : IComponentData
    {
        public float4 Size;
    }

    public struct RoadData
    {
        public CubeDirection Direction;
        public float Length;
        public float Width;
        public float Height;
        public const float MIN_WIDTH=1.5f;
        public const float MAX_WIDTH=3;
        public const float MIN_HEIGHR=1;
        public const float MAX_HEIGHR=3;
    }
}