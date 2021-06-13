using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public static class CanvasHelper
    {
        static List<Canvas> panelCanvas = new List<Canvas>();

        public static void SetPanelSortOrder(this Canvas canvas)
        {
            panelCanvas.Add(canvas);
            resetPanelCanvasSortOrder();
        }

        public static void ClearSortOrder(this Canvas canvas)
        {
            panelCanvas.Remove(canvas);
            resetPanelCanvasSortOrder();
        }

        private static void resetPanelCanvasSortOrder()
        {
            for (int i = 0; i < panelCanvas.Count; i++)
            {
                panelCanvas[i].sortingOrder = i;
            }
        }
    }
}
