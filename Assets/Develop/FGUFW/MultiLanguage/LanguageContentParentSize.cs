using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FGUFW.Core
{
    public class LanguageContentParentSize : MonoBehaviour
    {
        public Vector2 Padding;
        public bool Horizontal=true,Vertical=true;

        public void OnLanguageChanged()
        {
            var rect = transform.AsRT();
            var layout = GetComponent<ILayoutElement>();
            layout.CalculateLayoutInputHorizontal();
            layout.CalculateLayoutInputVertical();
            
            rect.anchorMin = new Vector2(0.5f,0.5f);
            rect.anchorMax = new Vector2(0.5f,0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.pivot = new Vector2(0.5f,0.5f);

            float width = layout.preferredWidth ;
            float height = layout.preferredHeight;
            rect.sizeDelta = new Vector2(width,height);
            
            width *= rect.localScale.x;
            height *= rect.localScale.y;

            rect = transform.parent.AsRT();
            var sizeDelta = rect.sizeDelta;
            if(Horizontal)
            {
                sizeDelta.x = width+Padding.x*2;
            }
            if(Vertical)
            {
                sizeDelta.y = height+Padding.y*2;
            }
            rect.sizeDelta = sizeDelta;
        }
        
    }
}
