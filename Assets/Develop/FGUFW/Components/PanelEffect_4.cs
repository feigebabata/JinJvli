using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class PanelEffect_4 : MonoBehaviour
    {
        public Item[] ShowAnims,HideAnims;
        // private const float DeltaTime = 0.015f;

        public void Show()
        {
            playAnim(ShowAnims);
        }

        public void Hide()
        {
            playAnim(HideAnims);
        }

        private void playAnim(Item[] items)
        {
            StopAllCoroutines();
            foreach (var item in items)
            {
                StartCoroutine(moveAnim(item));
            }
        }

        private IEnumerator moveAnim(Item item)
        {
            float speed =  Time.unscaledDeltaTime*item.Offset.magnitude/item.Time;
            var endPoint = item.Target.anchoredPosition+item.Offset;
            float t=0;
            while (t<item.Time)
            {
                item.Target.anchoredPosition = Vector2.MoveTowards(item.Target.anchoredPosition,endPoint,speed);
                yield return null;
                t+=Time.unscaledDeltaTime;
            }
            item.Target.anchoredPosition = endPoint;
        }

        [Serializable]
        public struct Item
        {
            public RectTransform Target;
            public Vector2 Offset;
            public float Time;
        }
    }
}
