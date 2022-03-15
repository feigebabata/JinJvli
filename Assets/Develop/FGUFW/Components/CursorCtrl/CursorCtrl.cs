using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class CursorCtrl : MonoSingleton<CursorCtrl>
    {
        [Header("系统鼠标")]
        public Texture2D Def_Up_Tex;
        public Texture2D[] Def_Down_Texs;
        public float Def_Down_Time=0.25f;
        public Vector2 Hotspot;
        private int _def_Index=-1;
        private float _def_DownTime;

        [Header("UI鼠标")]
        public List<CtrlUnit> Units;
        private Vector2 _viewSize;
        private Queue<CursorCtrlMode> _queue = new Queue<CursorCtrlMode>();
        private float _ui_playTime;
        private Vector2Int _currentTexIndex = Vector2Int.down;

        protected override bool IsDontDestroyOnLoad()
        {
            return true;
        }

        protected override void Init()
        {
            base.Init();
            // Cursor.visible=false;
            _viewSize = transform.AsRT().sizeDelta;
            Cursor.SetCursor(Def_Up_Tex,Hotspot,CursorMode.ForceSoftware);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            updateSystemCursor();
            updateUICursor();
            if(_queue.Count==0 && !Cursor.visible)
            {
                Cursor.visible = true;
            }
            else if(_queue.Count>0 && Cursor.visible)
            {
                Cursor.visible = false;
            }


        }

        private void updateUICursor()
        {
            if(_queue.Count==0)return;

            var unit = Units.Find(u=>u.ID==_queue.Peek());
            unit.Cursor.SetActive(true);
            
            unit.Cursor.transform.position = Input.mousePosition;
            if(!unit.Loop && _ui_playTime >= unit.Time)
            {
                unit.Cursor.SetActive(false);
                _queue.Dequeue();
                _ui_playTime=0;
                return;
            }
            _ui_playTime += Time.deltaTime;
        }

        private void updateSystemCursor()
        {
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                _def_DownTime=0;
            }
            else if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                if(_def_Index != -1)
                {
                    Cursor.SetCursor(Def_Up_Tex,Hotspot,CursorMode.ForceSoftware);
                    _def_Index = -1;
                }
            }
            else if(Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                int idx = MathfHelper.IndexOf(Def_Down_Texs.Length,Def_Down_Time,_def_DownTime);
                if(idx!=_def_Index)
                {
                    Cursor.SetCursor(Def_Down_Texs[idx],Hotspot,CursorMode.ForceSoftware);
                    _def_Index = idx;
                }
            }
            _def_DownTime += Time.deltaTime;
        }


        private bool cursorInScreen()
        {
            var pos = Input.mousePosition;
            if(pos.x<0||pos.y<0)return false;
            if(pos.x>Screen.width||pos.y>Screen.height)return false;
            return true;
        }

        public void SetPlayID(CursorCtrlMode id)
        {
            // Debug.Log($"Set {id}");
            Clear();
            _queue.Enqueue(id);
            _ui_playTime=0;
        }

        public void AddPlayID(CursorCtrlMode id)
        {
            // Debug.Log($"Add {id}");
            _queue.Enqueue(id);
        }

        public void Clear()
        {
            // Debug.Log($"Clear");
            if(_queue.Count==0)return;

            var unit = Units.Find(u=>u.ID==_queue.Peek());
            unit.Cursor.SetActive(false);
            _ui_playTime=0;
            _queue.Clear();
        }
    }

    [System.Serializable]
    public class CtrlUnit
    {
        public CursorCtrlMode ID;
        public GameObject Cursor;
        public float Time;
        public bool Loop;
    }

    

    public enum CursorCtrlMode
    {
        Move_1,
        Move_Err,
        Skill_1,
        Skill_2,
        DrumUp,
        DrumDown,
    }

}
