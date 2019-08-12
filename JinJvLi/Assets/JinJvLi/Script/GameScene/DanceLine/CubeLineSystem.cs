using Unity.Entities;
using Unity.Rendering;

namespace JinJvli.DanceLine
{
    public class CubeLineSystem : ComponentSystem
    {
        struct Group
        {
            public CubeLine Line;
            public RenderMesh Render;
            public CubeLineGrow Grow;
        }
        protected override void OnUpdate()
        {
            
        }
    }
}