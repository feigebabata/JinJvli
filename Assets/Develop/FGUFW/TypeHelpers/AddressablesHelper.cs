using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace FGUFW.Core
{
    static public class AddressablesHelper
    {
        static public async void SetSpritePath(this Image self,string path)
        {
            self.sprite = await Addressables.LoadAssetAsync<Sprite>(path).Task;
        }
    }
}
