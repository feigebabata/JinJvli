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
    public class MultiLanguageComp : MonoBehaviour
    {
        public string TextID;
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
            _comp.text = MultiLanguage.GetLanguageText(TextID);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            MultiLanguage.OnLanguageChanged -= this.onLanguageChanged;
        }


    }

}
