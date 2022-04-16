
namespace FGUFW.ECS
{
    /// <summary>
    /// 消息组件
    /// </summary>
    public interface IMessageComponent:IComponent
    {
        //在那一帧激活 一般都设为下一帧
        int ActiveFrameIndex{get;set;}
    }
}