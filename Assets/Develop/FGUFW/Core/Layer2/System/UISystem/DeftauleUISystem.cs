using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FGUFW.Play;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace FGUFW.Core.System
{
    public class DeftauleUISystem : IUISystem
    {
        public class UIPanelData
        {
            public IUIPanel UIPanel;
            public GameObject PanelGO;
            public int RenderSortOrder;
        }

        Transform _uiPanelRoot;
        List<UIPanelData> _uiPanelDatas = new List<UIPanelData>();
        public void OnInit()
        {
            _uiPanelRoot = new GameObject("UIPanelRoot").transform;
            GameObject.DontDestroyOnLoad(_uiPanelRoot);
        }

        public void OnRelease()
        {
            Logger.d("[DefaultUISystem.OnRelease]");
            GameObject.Destroy(_uiPanelRoot.gameObject);
            _uiPanelDatas.Clear();
        }
        public void HideView(IUIPanel uiPanel)
        {
            UIPanelData panelData = _uiPanelDatas.Find((uiPanelDataItem)=>{return uiPanelDataItem.UIPanel==uiPanel;});
            panelData.RenderSortOrder=-1;
            panelData.PanelGO.GetComponent<Canvas>().sortingOrder = panelData.RenderSortOrder;
            panelData.PanelGO.GetComponent<Canvas>().enabled=false;
            panelData.PanelGO.GetComponent<GraphicRaycaster>().enabled=false;
            uiPanel.OnHide();
        }

        public IEnumerator CreateView(IUIPanel uiPanel)
        {
            string assetPath = uiPanel.GetPanelAssetPath();
            Logger.d($"[DeftauleUISystem.Create] UI预制体加载 assetPath={assetPath}");
            var loader = Addressables.InstantiateAsync(assetPath,_uiPanelRoot,false);
            yield return loader;
            if(loader.Status == AsyncOperationStatus.Succeeded)
            {
                UIPanelData panelData = new UIPanelData();
                _uiPanelDatas.Add(panelData);
                panelData.PanelGO = loader.Result;
                panelData.UIPanel = uiPanel;

                panelData.PanelGO.GetComponent<Canvas>().enabled=false;
                panelData.PanelGO.GetComponent<GraphicRaycaster>().enabled=false;
                uiPanel.OnInit(loader.Result);
                
            }
            else
            {
                Logger.e($"[DeftauleUISystem.Create] UI预制体加载失败 assetPath={assetPath}");
            }
        }

        public void ReleaseView(IUIPanel uiPanel)
        {
            Logger.d($"[DeftauleUISystem.ReleaseView] {uiPanel.GetPanelAssetPath()}");
            UIPanelData panelData = _uiPanelDatas.Find((uiPanelDataItem)=>{return uiPanelDataItem.UIPanel==uiPanel;});
            uiPanel.OnRelease();
            GameObject.Destroy(panelData.PanelGO);
            panelData.PanelGO=null;
            panelData.UIPanel=null;
            _uiPanelDatas.Remove(panelData);
        }

        public void ShowView(IUIPanel uiPanel, IPlayModule module)
        {
            UIPanelData panelData = _uiPanelDatas.Find((uiPanelDataItem)=>{return uiPanelDataItem.UIPanel==uiPanel;});
            panelData.RenderSortOrder=_uiPanelDatas.Count;
            panelData.PanelGO.GetComponent<Canvas>().sortingOrder = panelData.RenderSortOrder;
            panelData.PanelGO.GetComponent<Canvas>().enabled=true;
            panelData.PanelGO.GetComponent<GraphicRaycaster>().enabled=true;
            uiPanel.OnShow(module);
        }
    }
}
