using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using System;

namespace GamePlay.StepGrid
{
    public class PlayerInputModule : PlayModule<StepGridPlayManager>
    {

        public override void OnInit(PlayManager playManager)
        {
            base.OnInit(playManager);
            if(!IsInit)
            {
                MonoBehaviourEvent.I.UpdateListener += Update;
            }
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if(Physics.Raycast(ray,out raycastHit))
                {
                    var gridComp = raycastHit.transform.GetComponent<DefaultGridComp>();
                    // _playManager.Messenger.Broadcast(StepGridMsgID.ClickGrid,gridComp.Data.x);
                }
            }
        }

        public override void OnRelease()
        {
            if(IsInit)
            {
                MonoBehaviourEvent.I.UpdateListener -= Update;

            }
            base.OnRelease();
        }

        public override void OnShow()
        {
            if(IsInit)
            {

            }
        }

        public override void OnHide()
        {
            if(IsInit)
            {

            }
        }

    }
}
