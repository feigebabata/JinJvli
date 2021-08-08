using UnityEngine;

namespace FGUFW.Core
{
    static public class ScreenHelper
    {
        static public void Landscape()
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
        static public void Portrait()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}