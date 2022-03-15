using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGUFW.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class PanelEffect_1 : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public GraphicRaycaster CanvasRaycaster;
        public Transform Panel;
        public float StartAlpha = 0;
        public float MoveOffset = -100;
        public float AnimTime = 0.25f;
        private float old_y;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            old_y = Panel.position.y;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(showAnim());
        }

        public void Hide()
        {
            if(gameObject.activeSelf)
            {
                StartCoroutine(hideAnim());
            }
        }

        private IEnumerator showAnim()
        {
            var pos = Panel.position - Vector3.up*MoveOffset;
            var endTime = Time.time+AnimTime;
            var alpha_w = (1-StartAlpha)/AnimTime;
            var speed = MoveOffset/AnimTime;
            Panel.position = pos;
            this.CanvasGroup.alpha = StartAlpha;
            CanvasRaycaster.enabled=false;
            while (Time.time<endTime)
            {
                yield return null;
                pos.y += speed*Time.deltaTime;
                Panel.position = pos;
                this.CanvasGroup.alpha += alpha_w*Time.deltaTime;
            }
            this.CanvasGroup.alpha=1;
            CanvasRaycaster.enabled=true;
            pos.y=old_y;
            Panel.position = pos;
        }

        private IEnumerator hideAnim()
        {
            var old_pos = Panel.position;
            var pos = Panel.position;
            var endTime = Time.time+AnimTime;
            var alpha_w = -(1-StartAlpha)/AnimTime;
            var speed = -MoveOffset/AnimTime;
            Panel.position = pos;
            this.CanvasGroup.alpha = 1;
            CanvasRaycaster.enabled=false;
            while (Time.time<endTime)
            {
                yield return null;
                pos.y += speed*Time.deltaTime;
                Panel.position = pos;
                this.CanvasGroup.alpha += alpha_w*Time.deltaTime;
            }
            gameObject.SetActive(false);
            this.CanvasGroup.alpha=StartAlpha;
            CanvasRaycaster.enabled=true;
            Panel.position = old_pos;
        }

    }
}
