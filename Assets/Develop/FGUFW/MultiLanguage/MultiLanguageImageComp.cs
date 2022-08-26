using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace FGUFW.Core
{
    [RequireComponent(typeof(Image))]
    public class MultiLanguageImageComp  : MultiLanguageCompBase
    {
        private Image _comp;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            this._comp = this.GetComponent<Image>();
            MultiLanguage.OnLanguageChanged += this.onLanguageChanged;
            onLanguageChanged();
        }

        private void onLanguageChanged()
        {
            SetMLText(TextID.ToCL());
        }

        public void SetSprite(Image comp, string text)
        {
            Sprite sprite = null;
            #if UNITY_EDITOR
                if(!UnityEditor.EditorApplication.isPlaying)
                {
                    sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Develop/ArtResources/MultiLanguageSprite/{text}.png");
                }
                else
                {
                    sprite = Addressables.LoadAssetAsync<Sprite>($"ArtResources/MultiLanguageSprite/{text}.png").WaitForCompletion();
                }
            #else
                sprite = Addressables.LoadAssetAsync<Sprite>($"ArtResources/MultiLanguageSprite/{text}.png").WaitForCompletion();
            #endif
            if(sprite!=null)
            {
                comp.sprite = sprite;
                comp.SetNativeSize();
                GetComponent<LanguageContentParentSize>()?.OnLanguageChanged();
            }
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            MultiLanguage.OnLanguageChanged -= this.onLanguageChanged;
        }

        public override void SetMLText(string text)
        {
            if(_comp==null)_comp = GetComponent<Image>();
            SetSprite(_comp,text);
        }
    }

}
