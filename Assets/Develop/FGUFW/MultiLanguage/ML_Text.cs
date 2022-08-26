using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGUFW.Core
{
    public class ML_Text : Text
    {
        private string _ml_Id;
        public string ML_Id
        {
            get
            {
                return _ml_Id;
            }
            set
            {
                _ml_Id = value;

            }
        }
    }
}
