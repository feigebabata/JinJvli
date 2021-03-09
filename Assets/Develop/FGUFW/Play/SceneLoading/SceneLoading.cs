using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;

namespace FGUFW.Play
{
    public class SceneLoading : MonoSingleton<SceneLoading>
    {
        const float MIN_SHOW_TIME=1;
        float showTime;
        public void Show()
        {
            gameObject.SetActive(true);
            showTime = Time.time;
        }
        public void Hide()
        {
            StopAllCoroutines();
            float time = Time.time-showTime;
            if(time<MIN_SHOW_TIME)
            {
                StartCoroutine(delayHide(MIN_SHOW_TIME-time));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        IEnumerator delayHide(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        }
        
    }    
}
