namespace FGUFW.Core
{
    public interface ISystem
    {
        void OnInit();
        void OnRelease();
        void OnEnable();
        void OnDisable();
    }
}