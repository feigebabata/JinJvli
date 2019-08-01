using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using JinJvli;
using TMPro;
using System.Collections.Generic;
using System;
using Google.Protobuf;

namespace JinJvLi
{
    [PanelConfig("JJL_Panel/LoginPanel")]
    public class LoginPanel : PanelBase
    {
        public static class Config
        {
            public const string SELF_INFO = "SELF_INFO";
            public const byte MAX_USER_NAME_LENGTH=4;
            public const byte MIN_USER_NAME_LENGTH=2;
        }
        public TMP_InputField m_sendIF;
        UserInfo m_selfInfo;

        public override void OnOpen(object _openData = null)
        {
            string user_json = PlayerPrefs.GetString(Config.SELF_INFO);
            if(string.IsNullOrEmpty(user_json))
            {
                m_selfInfo = new UserInfo();
                m_selfInfo.UID = SystemInfo.deviceUniqueIdentifier;
                string dev_name = "";
                for(int i = 0; i < SystemInfo.deviceName.Length; i++)
                {
                    if(SystemInfo.deviceName[i].IsZH_CN())
                    {
                        dev_name+=SystemInfo.deviceName[i];
                    }
                    if(dev_name.Length>=Config.MAX_USER_NAME_LENGTH)
                    {
                        break;
                    }
                }
                if(dev_name.Length<Config.MIN_USER_NAME_LENGTH)
                {
                    dev_name = "未知";
                }
                m_selfInfo.Name = dev_name;
                PlayerPrefs.SetString(Config.SELF_INFO,m_selfInfo.ToString());
            }
            else
            {
                m_selfInfo = UserInfo.Descriptor.Parser.ParseJson(user_json) as UserInfo;
            }
            m_sendIF.text = m_selfInfo.Name;
        }

        public void OnClick_Send()
        {
            PathSelect.OpenData openData = new PathSelect.OpenData();
            openData.FileTypes = new List<string>(){".mp3"};
            openData.Finsh = selectAudioResult;
            Main.Manager<PanelManager>().Open<PathSelect>(openData);
            // Main.Manager<NetworkManager>().SendBroadcast(new SendData());
        }

        public void OnEndEdit(string _text)
        {
            if(_text!= null && _text.Length>=Config.MIN_USER_NAME_LENGTH && _text.Length<=Config.MAX_USER_NAME_LENGTH && _text.IsZH_CN())
            {
                m_selfInfo.Name = _text;
            }
            else
            {
                m_sendIF.text = m_selfInfo.Name;
            }
        }

        private void selectAudioResult(List<string> obj)
        {
            if(obj.Count>0)
            {
                Debug.Log(obj[0]);
            }
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