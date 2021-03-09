using System;
using System.Collections;
using System.Threading.Tasks;
using FGUFW.Play;
using UnityEngine;

namespace FGUFW.Core.System
{
    public interface IUISystem : ISystem
    {
        IEnumerator CreateView(IUIPanel uiPanel);
        void ReleaseView(IUIPanel uiPanel);
        void ShowView(IUIPanel uiPanel,IPlayModule module);
        void HideView(IUIPanel uiPanel);
    }

    public interface IUIPanel
    {
        string GetPanelAssetPath();
        
        void OnInit(GameObject panelGO);
        void OnRelease();
        void OnShow(IPlayModule module);
        void OnHide();
    }

    // public enum UIOpenMode
    // {
    //     Overlay,
    //     Back,
    // } 
        
    // public enum UIPanelCycleStatus
    // {
    //     None=0,
    //     Create=3,//创建视图之外
    //     Show=2,//移至视图内
    //     Focus=1,//有焦点
    //     UnFocus=-1,//无焦点
    //     Hide=-2,//移至视图外
    //     Destroy=-3,//销毁或添加至回收池
    // }

    // public interface IUIPanelCycle
    // {
    //     UIPanelCycleStatus uiCycleStatus { get; set; }

    //     void onCreate();
    //     void onShow(object _data);
    //     void onFocus();
    //     void onUnfocus();
    //     void onHide();
    //     void onDestroy();
    // }
    
}