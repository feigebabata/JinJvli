using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using System;
using Google.Protobuf;

namespace GamePlay.GameLobby
{
    public class  OnlineGameModule : PlayModule<GameLobbyPlayManager>
    {
        public Dictionary<float,PB_OnlineGame> OnlineGameDic = new Dictionary<float, PB_OnlineGame>();
        public object OnlineGameDicLock = new object();
        
        private  OnlineGameModuleInput _moduleInput;
        private  OnlineGameModuleOutput _moduleOutput;
        private PB_OnlineGame _selectGame;
        private Coroutine _broadcastGamePlay;


        public OnlineGameModule(PlayManager playManager) : base(playManager)
        {
            _moduleOutput = new  OnlineGameModuleOutput(_playManager);
            _moduleInput = new  OnlineGameModuleInput(_playManager);
            UdpBroadcastUtility.Init();
            UdpBroadcastUtility.OnReceive += onReceive;
            
        }

        public override void Dispose()
        {
            UdpBroadcastUtility.OnReceive += onReceive;
            UdpBroadcastUtility.Release();
            _moduleInput.Dispose();
            _moduleOutput.Dispose();
        }

        public override void OnEnable()
        {
            _moduleInput.OnEnable();
            _moduleOutput.OnEnable();
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
            _playManager.Messenger.Add(GameLobbyMsgID.OnCreateGame,onCreateGame);
        }

        public override void OnDisable()
        {
            _moduleInput.OnDisable();
            _moduleOutput.OnDisable();
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnCreateGame,onCreateGame);
        }

        private void onCreateGame(object obj)
        {
            var gameplay = obj as PB_OnlineGame;
            _broadcastGamePlay = broadcastOnlineGame(gameplay).Start();
        }

        private void onClickBack(object obj)
        {
            OnDisable();
            _playManager.Module<LobbyModule>().OnEnable();
        }

        IEnumerator broadcastOnlineGame(PB_OnlineGame data)
        {
            var sendData = NetworkUtility.Encode(NetworkUtility.APP_ID,NetworkUtility.GAMELOBBY_GPID,data.ToByteArray());
            while (true)
            {
                UdpBroadcastUtility.Send(sendData);
                yield return new WaitForSeconds(1);
            }
        }

        private void onReceive(byte[] obj)
        {
            // Debug.Log(obj.Length);
            ushort appID=0,length=0,gameplayID=0;
            if(obj.Length>=NetworkUtility.PACK_HEAD_LENGTH && NetworkUtility.Decode(obj,ref appID,ref length,ref gameplayID))
            {
                if(appID==NetworkUtility.APP_ID && length==obj.Length && gameplayID==NetworkUtility.GAMELOBBY_GPID)
                {
                    PB_OnlineGame onlineGame = PB_OnlineGame.Parser.ParseFrom(obj,NetworkUtility.PACK_HEAD_LENGTH,length-NetworkUtility.PACK_HEAD_LENGTH);
                    lock(OnlineGameDicLock)
                    {
                        addOnlineGame(Time.time,onlineGame);
                    }
                }
            }
        }

        private void addOnlineGame(float time,PB_OnlineGame onlineGame)
        {
            float key = -1;
            foreach (var item in OnlineGameDic)
            {
                if(item.Value.Player.ID==onlineGame.Player.ID && item.Value.GamePlayID==onlineGame.GamePlayID )
                {
                    key = item.Key;
                }
            }
            OnlineGameDic.Add(time,onlineGame);
            if(key!=-1)
            {
                OnlineGameDic.Remove(key);
            }
        }


    }
}
