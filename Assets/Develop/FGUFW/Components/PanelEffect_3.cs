using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGUFW.Core
{
    public sealed class PanelEffect_3 : MonoBehaviour
    {
        public GraphicRaycaster CanvasRaycaster;
        public Transform Panel;

        public Vector3 MoveOffset;
        public float AnimTime = 0.25f;
        private Vector3 _old_pos;
        private float ShowTime;
        // private const float DeltaTime = 0.015f;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _old_pos = Panel.position;
        }

        public void Show()
        {
            StopAllCoroutines();
            StartCoroutine(showAnim());
        }

        public void Hide()
        {
            if(!Panel.gameObject.activeSelf)return;
            StopAllCoroutines();
            StartCoroutine(hideAnim());
        }

        private IEnumerator showAnim()
        {
            ShowTime = 0;
            var pos = _old_pos - MoveOffset;
            var endTime = ShowTime + AnimTime;
            var speed = MoveOffset/AnimTime;
            Panel.position = pos;
            Panel.gameObject.SetActive(true);
            CanvasRaycaster.enabled=false;
            while (ShowTime< endTime)
            {
                yield return null;
                ShowTime += Time.unscaledDeltaTime;
                pos += speed* Time.unscaledDeltaTime;
                Panel.position = pos;
            }
            CanvasRaycaster.enabled=true;
            Panel.position = _old_pos;
        }

        private IEnumerator hideAnim()
        {
            ShowTime = 0;
            var pos = _old_pos;
            var endTime = ShowTime + AnimTime;
            var speed = -MoveOffset/AnimTime;
            Panel.position = pos;
            CanvasRaycaster.enabled=false;
            while (ShowTime < endTime)
            {
                yield return null;
                ShowTime += Time.unscaledDeltaTime;
                pos += speed* Time.unscaledDeltaTime;
                Panel.position = pos;
            }
            Panel.gameObject.SetActive(false);
            CanvasRaycaster.enabled=true;
            Panel.position = _old_pos;
        }

    }
}
