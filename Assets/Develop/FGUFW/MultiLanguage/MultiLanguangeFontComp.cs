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
    public class MultiLanguangeFontComp : MonoBehaviour
    {
        private Text _comp;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            this._comp = this.GetComponent<Text>();
            MultiLanguage.OnLanguageChanged += this.OnLanguageChanged;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            OnLanguageChanged();
        }

        public void OnLanguageChanged()
        {
            if(_comp==null)this._comp = this.GetComponent<Text>();
            _comp.font = MultiLanguage.GetMultiLanguangeFont();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            MultiLanguage.OnLanguageChanged -= this.OnLanguageChanged;
        }
    }

}
