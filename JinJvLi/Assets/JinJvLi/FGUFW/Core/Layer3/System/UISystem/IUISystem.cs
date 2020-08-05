namespace FGUFW.Core.System
{
    public interface IUISystem
    {
        void Init();
        void Open(int panelID);
        void Back();
        void Clear();
    }

    public enum UIOpenMode
    {
        Overlay,
        Back,
    } 
        
    public enum UICycleStatus
    {
        None=0,
        Create=3,//创建视图之外
        Show=2,//移至视图内
        Focus=1,//有焦点
        UnFocus=-1,//无焦点
        Hide=-2,//移至视图外
        Destroy=-3,//销毁或添加至回收池
    }

    public interface IUICycle
    {
        void onCreate();
        void onShow(object _data);
        void onFocus();
        void onUnfocus();
        void onHide();
        void onDestroy();
    }    
    
}