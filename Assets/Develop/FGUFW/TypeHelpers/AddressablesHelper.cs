using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System;

namespace FGUFW.Core
{
    static public class AddressablesHelper
    {
        static public async void SetSpritePath(this Image self,string path,Action callback=null)
        {
            self.sprite = await Addressables.LoadAssetAsync<Sprite>(path).Task;
            callback?.Invoke();
        }

        static public async void SetAudioClipPath(this AudioSource self, string path)
        {
            self.clip = await Addressables.LoadAssetAsync<AudioClip>(path).Task;
        }

        static public async void SetSkeletonDataAssetPath(this SkeletonGraphic self, string path)
        {
            self.skeletonDataAsset = await Addressables.LoadAssetAsync<SkeletonDataAsset>(path).Task;
        }
    }
}
