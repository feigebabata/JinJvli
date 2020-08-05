

namespace FGUFW.Play
{
    public interface IPlayManager
    {
        T Model<T>() where T : IPlayModel;
        void Release();
    }
}