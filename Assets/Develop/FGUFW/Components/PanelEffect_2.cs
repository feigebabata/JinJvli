using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGUFW.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class PanelEffect_2 : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;

        public GraphicRaycaster CanvasRaycaster;
        public Transform Panel;
        public float StartAlpha = 0.7f;
        public float AnimTime = 0.1f;
        public float StartScale = 0.8f;

        public void Show()
        {
            // Debug.LogWarning("Show");
            StopAllCoroutines();
            StartCoroutine(showAnim());
        }

        public void Hide()
        {
            // Debug.LogWarning("Hide");
            StopAllCoroutines();
            StartCoroutine(hideAnim());
        }

        private IEnumerator showAnim()
        {
            var endTime = Time.time+AnimTime;
            var speed = (1-StartScale)/AnimTime;
            var scale = Vector3.one*StartScale;
            var alpha_w = (1-StartAlpha)/AnimTime;
            Panel.localScale = scale;
            Panel.gameObject.SetActive(true);
            CanvasRaycaster.enabled=false;
            this.CanvasGroup.alpha = StartAlpha;
            while (Time.time<endTime)
            {
                yield return null;
                scale += speed*Time.deltaTime*Vector3.one;
                // Debug.Log(scale);
                Panel.localScale = scale;
                this.CanvasGroup.alpha += alpha_w*Time.deltaTime;
            }
            CanvasRaycaster.enabled=true;
            Panel.localScale = Vector3.one;
        }

        private IEnumerator hideAnim()
        {
            var endTime = Time.time+AnimTime;
            var speed = -(1-StartScale)/AnimTime;
            var scale = Vector3.one;
            var alpha_w = -(1-StartAlpha)/AnimTime;
            Panel.localScale = scale;
            CanvasRaycaster.enabled=false;
            this.CanvasGroup.alpha = 1;
            while (Time.time<endTime)
            {
                yield return null;
                scale += speed*Time.deltaTime*Vector3.one;
                Panel.localScale = scale;
                this.CanvasGroup.alpha += alpha_w*Time.deltaTime;
            }
            Panel.gameObject.SetActive(false);
            this.CanvasGroup.alpha=StartAlpha;
            Panel.localScale = Vector3.one*StartScale;
        }

    }
}
