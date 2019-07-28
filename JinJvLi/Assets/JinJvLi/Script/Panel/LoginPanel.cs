using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using JinJvli;
using TMPro;

namespace JinJvLi
{
    [PanelConfig("JJL_Panel/LoginPanel")]
    public class LoginPanel : PanelBase
    {
        public TMP_InputField m_sendIF;

        public void OnClick_Send()
        {
            Main.Manager<NetworkManager>().SendBroadcast(new SendData());
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