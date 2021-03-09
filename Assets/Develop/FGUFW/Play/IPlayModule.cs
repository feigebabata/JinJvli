

namespace FGUFW.Play
{
    public interface IPlayModule
    {
        void OnInit(IPlayManager playManager);
        void OnRelease();
        void OnShow();
        void OnHide();
        bool IsInit();
    }
}