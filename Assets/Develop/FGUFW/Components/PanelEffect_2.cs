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
        private float ShowTime;
        // private const float DeltaTime = 0.015f;

        public void Show()
        {
            // Debug.LogWarning("Show");
            StopAllCoroutines();
            StartCoroutine(showAnim());
        }

        public void Hide()
        {
            if(!Panel.gameObject.activeSelf)return;
            // Debug.LogWarning("Hide");
            StopAllCoroutines();
            StartCoroutine(hideAnim());
        }

        private IEnumerator showAnim()
        {
            ShowTime = 0;
            var endTime = AnimTime;
            var speed = (1-StartScale)/AnimTime;
            var scale = Vector3.one*StartScale;
            var alpha_w = (1-StartAlpha)/AnimTime;
            Panel.localScale = scale;
            Panel.gameObject.SetActive(true);
            CanvasRaycaster.enabled=false;
            this.CanvasGroup.alpha = StartAlpha;
            while (ShowTime<endTime)
            {
                yield return null;
                ShowTime+=Time.unscaledDeltaTime;
                scale += speed*Time.unscaledDeltaTime*Vector3.one;
                // Debug.Log(scale);
                Panel.localScale = scale;
                this.CanvasGroup.alpha += alpha_w*Time.unscaledDeltaTime;
            }
            CanvasRaycaster.enabled=true;
            Panel.localScale = Vector3.one;
        }

        private IEnumerator hideAnim()
        {
            // Debug.Log("hideAnim");
            ShowTime = 0;
            var endTime = AnimTime;
            var speed = -(1-StartScale)/AnimTime;
            var scale = Vector3.one;
            var alpha_w = -(1-StartAlpha)/AnimTime;
            Panel.localScale = scale;
            CanvasRaycaster.enabled=false;
            this.CanvasGroup.alpha = 1;
            while (ShowTime<endTime)
            {
            // Debug.Log("wait update");
                yield return null;
                ShowTime+=Time.unscaledDeltaTime;
                scale += speed*Time.unscaledDeltaTime*Vector3.one;
                Panel.localScale = scale;
                this.CanvasGroup.alpha += alpha_w*Time.unscaledDeltaTime;
            }
            Panel.gameObject.SetActive(false);
            this.CanvasGroup.alpha=StartAlpha;
            Panel.localScale = Vector3.one*StartScale;
        }

    }
}
