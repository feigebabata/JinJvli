using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    [RequireComponent(typeof(Canvas))]
    public class WorldCanvasSizeByCameraView : MonoBehaviour
    {
        void OnEnable()
        {
            StartCoroutine(initCanvasSize());
        }

        IEnumerator initCanvasSize()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            while (canvas.worldCamera==null)yield return null;
            var width = Screen.width;
            var height = Screen.height;
            var rect = transform.AsRT();
            var size = new Vector2(0,canvas.worldCamera.orthographicSize*2);
            size.x = (width*size.y)/height;
            rect.sizeDelta = size;
        }
    }
}
