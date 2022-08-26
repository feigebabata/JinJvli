using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class DestroyDoDestroy : MonoBehaviour
    {
        public GameObject[] GObjs;
        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            foreach (var item in GObjs)
            {
                Destroy(item);
            }
        }
    }
}
