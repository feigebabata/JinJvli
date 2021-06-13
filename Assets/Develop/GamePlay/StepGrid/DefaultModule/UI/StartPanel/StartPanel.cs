using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.StepGrid
{
    public class StartPanel : IUIPanel
    {
        private StepGridPlayManager _playManager;
        private StartPanelComps _startPanelComps;

        public string GetPanelAssetPath()
        {//返回UI预制件地址
            return "GamePlay.StepGrid.DefaultModule.StartPanel";
        }

        public void OnInit(GameObject panelGO, PlayManager playManager)
        {
            _playManager = playManager as StepGridPlayManager;
            _startPanelComps = panelGO.GetComponent<StartPanelComps>();
            _startPanelComps.StartBtn.onClick.AddListener(onClickStart);
        }

        public void OnRelease()
        {
            _startPanelComps.StartBtn.onClick.AddListener(onClickStart);
            _startPanelComps=null;
            _playManager=null;
        }

        public void OnShow()
        {
            
        }

        public void OnHide()
        {
            
        }

        private void onClickStart()
        {
            _playManager.Messenger.Broadcast(StepGridMsgID.Start,null);
        }

    }

}
