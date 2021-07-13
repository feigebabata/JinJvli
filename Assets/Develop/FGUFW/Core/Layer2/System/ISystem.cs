namespace FGUFW.Core
{
    public interface ISystem
    {
        void OnInit(params object[] datas);
        void OnRelease();
        void OnEnable();
        void OnDisable();
    }
}