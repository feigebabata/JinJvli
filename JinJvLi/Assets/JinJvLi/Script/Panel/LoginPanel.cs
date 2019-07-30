using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using JinJvli;
using TMPro;
using System.Collections.Generic;

namespace JinJvLi
{
    [PanelConfig("JJL_Panel/LoginPanel")]
    public class LoginPanel : PanelBase
    {
        public TMP_InputField m_sendIF;

        public override void Open(object _openData = null)
        {
        }
        public override void Show()
        {
            base.Show();
        }

        public void OnClick_Send()
        {
            PathSelect.OpenData openData = new PathSelect.OpenData();
            Main.Manager<PanelManager>().Open<PathSelect>(openData);
            // Main.Manager<NetworkManager>().SendBroadcast(new SendData());
        }

        struct SendData : ISendData
        {
            public byte[] Pack()
            {
                return new byte[45535];
            }
        }
    }
}