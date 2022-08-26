using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace FGUFW.Core
{
    [RequireComponent(typeof(Text))]
    public class MultiLanguageComp : MultiLanguageCompBase
    {
        private Text _comp;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            this._comp = this.GetComponent<Text>();
            MultiLanguage.OnLanguageChanged += this.onLanguageChanged;
            onLanguageChanged();
        }

        private void onLanguageChanged()
        {
            SetMLText(TextID.ToCL());
        }

        public void SetTextId(string textId)
        {
            TextID = textId;
            SetMLText(TextID.ToCL());
        }

        public void SetText(Text comp, string text)
        {
            var spaceIndex = 0;
            foreach (var item in text)
            {
                if(item!=' ')break;
                spaceIndex++;
            }
            if(spaceIndex>0)
            {
                // Debug.Log($"{transform.FullPath()}  spaceIndex");
                var spaceText = string.Empty;
                for (int i = 0; i < spaceIndex; i++)
                {
                    spaceText += "\u3000";
                }
                text = spaceText + text.Substring(spaceIndex,text.Length-spaceIndex);
            }
            comp.text = text;
            comp.font = MultiLanguage.GetMultiLanguangeFont();
            comp.CalculateLayoutInputHorizontal();
            comp.CalculateLayoutInputVertical();
            GetComponent<LanguageContentParentSize>()?.OnLanguageChanged();
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
            if(_comp==null)_comp = GetComponent<Text>();
            SetText(_comp,text);
        }
    }

    public abstract class MultiLanguageCompBase:MonoBehaviour
    {
        public string TextID;
        public abstract void SetMLText(string text);
    }

}
