using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FGUFW.Core
{
    public class PCPlayerInput : IPlayerInput
    {
        private Action<Vector2> _onMove;
        private Action _onClickA;
        private Action _onClickB;
        private Action _onClickBack;
        private bool _enabled=true;

        public Action OnClickBack { get => _onClickBack; set => _onClickBack = value; }
        public Action<Vector2> OnMove { get => _onMove; set => _onMove = value; }
        public Action OnClickA { get => _onClickA; set => _onClickA = value; }
        public Action OnClickB { get => _onClickB; set => _onClickB = value; }

        public void LateUpdate()
        {
            if(!_enabled)
            {
                return;
            }
            if(Input.GetMouseButtonDown(0) && EventSystem.current && !EventSystem.current.firstSelectedGameObject)
            {
                _onClickA?.Invoke();
            }
            if(Input.GetMouseButtonDown(1) && EventSystem.current && !EventSystem.current.firstSelectedGameObject)
            {
                _onClickB?.Invoke();
            }
            if(Input.GetKey(KeyCode.Escape))
            {
                OnClickBack?.Invoke();
            }
            Vector2 dir = Vector2.zero;
            if(Input.GetKey(KeyCode.W))
            {
                dir+=Vector2.up;
            }
            if(Input.GetKey(KeyCode.S))
            {
                dir+=Vector2.down;
            }
            if(Input.GetKey(KeyCode.A))
            {
                dir+=Vector2.left;
            }
            if(Input.GetKey(KeyCode.D))
            {
                dir+=Vector2.right;
            }
            if(dir!=Vector2.zero)
            {
                _onMove?.Invoke(dir.normalized);
            }
        }

        public void OnDisable()
        {
            _enabled = false;
        }

        public void OnEnable()
        {
            _enabled = true;
        }

        public void OnInit()
        {
            
        }

        public void OnRelease()
        {
            
        }
    }
}