using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace FGUFW.Core
{
    public class AndroidPlayerInput : IPlayerInput
    {
        public static class Config
        {
            public const string CAMERA_ROTATE_KEY = "AndroidPlayerInput.CameraRotate";
        }

        private Action<Vector2> _onMove;
        private Action _onClickA;
        private Action _onClickB;
        private Action _onClickBack;

        public Action OnClickBack { get => _onClickBack; set => _onClickBack = value; }
        public Action<Vector2> OnMove { get => _onMove; set => _onMove = value; }
        public Action OnClickA { get => _onClickA; set => _onClickA = value; }
        public Action OnClickB { get => _onClickB; set => _onClickB = value; }

        private GameObject _canvas;
        private AndroidPlayerInputMove _moveBehaviour;
        private Button _btnA;
        private Button _btnB;
        private Slider _cameraRotate;
        private AndroidCameraRotateCtrl _cameraRotateCtrl;
        private bool _enabled=true;

        public void LateUpdate()
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                OnClickBack?.Invoke();
            }
            
            
        }

        public void OnInit()
        {
            loadCanvas().Start();
        }

        public void OnRelease()
        {
            if(_canvas)
            {
                _moveBehaviour.OnDragListenter.RemoveAllListeners();
                _btnA.onClick.RemoveAllListeners();
                _btnB.onClick.RemoveAllListeners();
                _cameraRotate.onValueChanged.RemoveAllListeners();
                _moveBehaviour = null;
                _btnA = null;
                _btnB = null;
                GameObject.Destroy(_canvas);
            }
        }

        IEnumerator loadCanvas()
        {
            var loader = Addressables.InstantiateAsync("FGUFW.Core.AndroidPlayerInput",null,false);
            yield return loader;
            _canvas = loader.Result;
            _canvas.name = "AndroidPlayerInput";
            _moveBehaviour = _canvas.transform.Find("move").GetComponent<AndroidPlayerInputMove>();
            _btnA = _canvas.transform.Find("btnA").GetComponent<Button>();
            _btnB = _canvas.transform.Find("btnB").GetComponent<Button>();
            _cameraRotate = _canvas.transform.Find("cameraRotate").GetComponent<Slider>();

            _moveBehaviour.OnDragListenter.AddListener(onMove);
            _btnA.onClick.AddListener(onClickA);
            _btnB.onClick.AddListener(onClickB);

            _canvas.SetActive(_enabled);

            _cameraRotate.value = ConfigDatabase.GetConfig(Config.CAMERA_ROTATE_KEY,180f);
            _cameraRotate.onValueChanged.AddListener(_onSliderChanged);
            
        }

        private void _onSliderChanged(float arg0)
        {
            ConfigDatabase.SetConfig(Config.CAMERA_ROTATE_KEY,arg0);
        }

        public float CameraRotateY
        {
            get
            {
                return _cameraRotate? _cameraRotate.value:0;
            }
        }

        private void onClickB()
        {
            if(!_enabled)
            {
                return;
            }
            _onClickB?.Invoke();
        }

        private void onClickA()
        {
            Debug.Log("onClickA");
            if(!_enabled)
            {
                return;
            }
            _onClickA?.Invoke();
        }

        private void onMove(Vector2 arg0)
        {
            if(!_enabled)
            {
                return;
            }
            _onMove?.Invoke(arg0);
        }

        public void OnEnable()
        {
            _enabled = true;
            _canvas?.SetActive(true);
        }

        public void OnDisable()
        {
            _enabled = false;
            _canvas?.SetActive(false);
        }
    }
}
