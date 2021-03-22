using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FGUFW.Core
{
    public class AndroidPlayerInputMove : MonoBehaviour
    {
        public ScrollRect.ScrollRectEvent OnDragListenter;
        
        private Vector3 _downPos = Vector3.back;
        private Image _image;

        void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void OnPointerUp(BaseEventData eventData)
        {
            _downPos=Vector3.back;
            _image.color = new Color32(255,255,255,5);
        }

        public void OnPointerDown(BaseEventData eventData)
        {
            _downPos=Input.mousePosition;
            _image.color = new Color32(0,0,0,5);
            
        }

        void Update()
        {
            if(_downPos!=Vector3.back)
            {
                OnDragListenter?.Invoke((Input.mousePosition-_downPos).normalized);
            }
        }

    }
}
