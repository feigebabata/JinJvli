using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.StepGrid
{
    public class DefaultGridComp : MonoBehaviour,IPointerDownHandler
    {
        public Vector2Int Data;

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }
        
        public void Init(DefaultView view,Vector2Int data)
        {

        }
    }
}
