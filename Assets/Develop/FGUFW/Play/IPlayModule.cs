
namespace FGUFW.Play
{
    public interface IPlayModule
    {

        void OnInit(PlayManager playManager);

        void OnRelease();

        void OnShow();

        void OnHide();

        bool IsInit { get;}
    }
}