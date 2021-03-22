using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.Playables;
using System;
using System.Reflection;
using UnityEngine.AddressableAssets;

namespace GamePlay.GameLobby
{
    public class DefaultModule : IPlayModule
    {
        private bool _isInit;
        private GameLobbyPlayManager _playManager;
        private Camera _mainCamera;
        private CharacterController _characterController;
        private float Speed=3;
        private GameItem _currectSelect;
        private Transform _gameItemsParent;

        public Color Select = new Color32(255,0,255,255);
        public Color UnSelect = new Color32(0,255,255,255);

        public bool IsInit()
        {
            return _isInit;
        }

        public void OnInit(IPlayManager playManager)
        {
            if(!_isInit)
            {
                _isInit = true;
                _playManager = playManager as GameLobbyPlayManager;
                GameObject.Find("cameraAnim").GetComponent<PlayableDirector>().stopped += onStartAniStop;
                _playManager.PlayerInput.OnMove += onMove;
                _playManager.PlayerInput.OnClickA += onClickA;
                _playManager.PlayerInput.OnDisable();
                _mainCamera = GameObject.Find("character/Main Camera").GetComponent<Camera>();
                _gameItemsParent = GameObject.Find("gamelist").transform;
                _characterController = GameObject.Find("character").GetComponent<CharacterController>();
                MonoBehaviourEvent.I.UpdateListener += Update;
                _playManager.PlayerInput.OnClickBack += onClickBack;
                Cursor.lockState = CursorLockMode.Locked;
                
                showItemList().Start();
                OnShow();
            }
        }

        private void onClickBack()
        {
            Application.Quit();
        }

        private void onClickA()
        {
            if(_currectSelect)
            {
                enterGame(_currectSelect.TypeName);
            }
        }

        private void onMove(Vector2 obj)
        {
            
            var selfDirUp = new Vector3(_mainCamera.transform.forward.x,0,_mainCamera.transform.forward.z).normalized*obj.y*Speed; 
            var selfDirRight = new Vector3(_mainCamera.transform.right.x,0,_mainCamera.transform.right.z).normalized*obj.x*Speed;
            _characterController.SimpleMove(selfDirRight+selfDirUp);
        }

        public void OnRelease()
        {
            if(_isInit)
            {
                _playManager.PlayerInput.OnMove -= onMove;
                _playManager.PlayerInput.OnClickA -= onClickA;
                _playManager.PlayerInput.OnClickBack -= onClickBack;
                MonoBehaviourEvent.I.UpdateListener -= Update;
                _isInit = false;
                _playManager = null;
                _mainCamera = null;
                _characterController = null;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        public void OnShow()
        {
            if(_isInit)
            {

            }
        }

        public void OnHide()
        {
            if(_isInit)
            {

            }
        }

        private void onStartAniStop(PlayableDirector obj)
        {
            _playManager.PlayerInput.OnEnable();
            #if UNITY_ANDROID && !UNITY_EDITOR
            _mainCamera.GetComponent<AndroidCameraRotateCtrl>().enabled=true;
            _mainCamera.GetComponent<AndroidCameraRotateCtrl>().PlayerInput = _playManager.PlayerInput as AndroidPlayerInput;
            #else
            _mainCamera.GetComponent<PCCameraRotateCtrl>().enabled=true;
            #endif
            
        }

        void Update()
        {
            rayUpdate();
        }

        void rayUpdate()
        {
            Ray ray = new Ray(_mainCamera.transform.position,_mainCamera.transform.forward);
            RaycastHit raycastHit;
            if(Physics.Raycast(ray,out raycastHit))
            {
                var item = raycastHit.transform.GetComponent<GameItem>();

                if(item)
                {
                    
                    if(_currectSelect && _currectSelect!=item)
                    {
                        _currectSelect.GetComponent<MeshRenderer>().material.SetColor("MainColor",UnSelect);
                    }

                    if(_currectSelect==null || _currectSelect!=item)
                    {
                        _currectSelect = item;
                        _currectSelect.GetComponent<MeshRenderer>().material.SetColor("MainColor",Select);
                    }
                }
                else if(_currectSelect)
                {
                    _currectSelect.GetComponent<MeshRenderer>().material.SetColor("MainColor",UnSelect);
                    _currectSelect=null;
                }
                
            }
        }

        void enterGame(string typeName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            var playManager = assembly.CreateInstance(typeName) as IPlayManager; 

            _playManager.Destroy();
            playManager.Create();
        }

        IEnumerator showItemList()
        {
            var loader = Addressables.LoadAssetAsync<GameItemDatas>("GamePlay.GameLobby.GameDatas");
            yield return loader;
            var datas = loader.Result.Datas;
            bool ignore = false;
            for (int i = 0; i < datas.Length; i++)
            {
                #if UNITY_ANDROID && !UNITY_EDITOR
                    ignore = !datas[i].AndroidPlatform;
                #elif UNITY_WEBGL && !UNITY_EDITOR
                    ignore = !datas[i].WebPlatform;
                #else
                    ignore = !datas[i].PCPlatform;
                #endif

                if(!ignore)
                {
                    Transform item = GameObject.Instantiate(_gameItemsParent.GetChild(0),_gameItemsParent,false);
                    item.GetComponent<GameItem>().TypeName = datas[i].TypeName;
                    item.GetChild(0).GetComponent<SpriteRenderer>().sprite = datas[i].Icon;
                    item.GetChild(0).localScale = datas[i].Scale;
                    var space = item.localPosition*(1+i*0.02f);
                    item.localPosition = Quaternion.Euler(0,23*i,0) * space;
                    item.gameObject.SetActive(true);
                    item.name = datas[i].TypeName;
                }
            }
        }

    }
}
