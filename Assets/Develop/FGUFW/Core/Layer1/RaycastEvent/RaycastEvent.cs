using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class RaycastEvent : MonoSingleton<RaycastEvent>
    {
        public const float CLICK_MAX_TIME = 0.25f;
        public static float CLICK_MAX_OFFSET;

        private Queue<Vector3> _clickPoss=new Queue<Vector3>();
        private bool _cast=false;
        private float _castTime;
        private List<Listener> _listeners = new List<Listener>();
        private Dictionary<string,RaycastHit> _selectDic = new Dictionary<string, RaycastHit>();

        protected override void Init()
        {
            base.Init();
            CLICK_MAX_OFFSET = Screen.width * 1f/100f;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(!_cast)
                {
                    castStart();
                }
                _clickPoss.Enqueue(Input.mousePosition);
            }
            if(Input.GetMouseButtonUp(0))
            {
                _clickPoss.Enqueue(Input.mousePosition);
            }
            if(_cast && Time.time-_castTime>=CLICK_MAX_TIME)
            {
                if(_clickPoss.Count==1)
                {
                    var pos = _clickPoss.Dequeue();
                    if( (Input.mousePosition-pos).magnitude <= CLICK_MAX_OFFSET )
                    {
                        castEvent(RaycastEventType.Long);
                    }
                }
                else if(_clickPoss.Count==2)
                {
                    castEvent(RaycastEventType.Click);
                }
                else
                {
                    Vector3 down_1 = _clickPoss.Dequeue();
                    _clickPoss.Dequeue();
                    Vector3 down_2 = _clickPoss.Dequeue();
                    if( (down_1-down_2).magnitude <= CLICK_MAX_OFFSET )
                    {
                        castEvent(RaycastEventType.Double);
                    }
                }

                castEnd();
            }
        }

        void castEvent(RaycastEventType type)
        {
            var list = _listeners.ToArray();
            foreach (var listener in list)
            {
                if(type==listener.Type)
                {
                    foreach (var select in _selectDic)
                    {
                        if(listener.LayerMask==select.Key)
                        {
                            listener.Callback(select.Value);
                        }
                    }
                }    
            }
        }

        void castStart()
        {
            _castTime = Time.time;
            _cast=true;
            _clickPoss.Clear();
            raycast();
        }

        void castEnd()
        {
            _clickPoss.Clear();
            _cast=false;
        }

        void raycast()
        {
            _selectDic.Clear();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (var item in _listeners)
            {
                int layer = -1;
                if(string.IsNullOrEmpty(item.LayerMask))
                {
                    layer = LayerMask.NameToLayer(item.LayerMask);
                }
                
                RaycastHit raycastHit;
                if(layer==-1)
                {
                    if(Physics.Raycast(ray,out raycastHit))
                    {
                        if(!_selectDic.ContainsKey(string.Empty))
                        {
                            _selectDic.Add(string.Empty,raycastHit);
                        }
                        else
                        {
                            _selectDic[string.Empty] = raycastHit;
                        }
                    }
                }
                else
                {
                    if(Physics.Raycast(ray,out raycastHit,1000,layer))
                    {
                        if(!_selectDic.ContainsKey(string.Empty))
                        {
                            _selectDic.Add(string.Empty,raycastHit);
                        }
                        else
                        {
                            _selectDic[string.Empty] = raycastHit;
                        }
                    }
                }
            }
        }

        public void AddListener(RaycastEventType type,Action<RaycastHit> callback,string layerMask="")
        {
            if(callback==null)
            {
                return;
            }
            _listeners.Add(new Listener(){Type=type,Callback=callback,LayerMask=layerMask});
        }

        public void RemoveListener(RaycastEventType type,Action<RaycastHit> callback,string layerMask="")
        {
            _listeners.RemoveAll( l=>{return l.Callback==callback && l.Type==type && l.LayerMask==layerMask;} );
        }

        public class Listener
        {
            public RaycastEventType Type;
            public Action<RaycastHit> Callback;
            public string LayerMask;
        }

        
        
    }

    public enum RaycastEventType
    {
        Click,
        Long,
        Double,
    }

}
