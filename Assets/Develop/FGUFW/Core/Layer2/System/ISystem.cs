namespace FGUFW.Core
{
    public interface ISystem
    {
        void OnInit(object data);
        void OnRelease();
        void OnEnable();
        void OnDisable();
    }
}