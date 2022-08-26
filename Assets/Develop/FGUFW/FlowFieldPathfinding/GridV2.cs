
using UnityEngine;

namespace FGUFW.FlowFieldPathfinding
{
    public struct GridV2
    {
        /// <summary>
        /// 距起点间距
        /// -2:障碍
        /// -1:未知
        /// </summary>
        public int Distance;
        public Vector2Int Position;
    }
}