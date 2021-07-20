using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using System;
using Google.Protobuf;
using System.Reflection;

namespace GamePlay.GameLobby
{
    public class  OnlineGameModule : PlayModule<GameLobbyPlayManager>
    {
        public Dictionary<float,PB_OnlineGame> OnlineGameDic = new Dictionary<float, PB_OnlineGame>();
        public object OnlineGameDicLock = new object();
        public const uint ONLINE_GAME_CMD=3;
        public PB_PlayerInfo SelfInfo;
        
        private  OnlineGameModuleInput _moduleInput;
        private  OnlineGameModuleOutput _moduleOutput;
        private long _selectGamePlayID;
        private Coroutine _broadcastGamePlay;
        private bool isEnterGame=false;


        public OnlineGameModule(PlayManager playManager) : base(playManager)
        {
            SelfInfo = new PB_PlayerInfo()
            {
                ID = SystemInfo.deviceUniqueIdentifier,
                Nickname = ConfigDatabase.GetConfig("nickname",SystemInfo.deviceName),
            };
            _moduleOutput = new  OnlineGameModuleOutput(_playManager);
            _moduleInput = new  OnlineGameModuleInput(_playManager);
            UdpBroadcastUtility.Init();
            UdpBroadcastUtility.OnReceive += onReceive;
            
        }

        public override void Dispose()
        {
            _broadcastGamePlay?.Stop();
            _broadcastGamePlay=null;
            UdpBroadcastUtility.OnReceive -= onReceive;
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
            _playManager.Messenger.Add(GameLobbyMsgID.OnJoinGame,onJoinGame);
            _playManager.Messenger.Add(GameLobbyMsgID.OnExitGame,onExitGame);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickStartBtn,onClickStartBtn);
        }

        public override void OnDisable()
        {
            _moduleInput.OnDisable();
            _moduleOutput.OnDisable();
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnCreateGame,onCreateGame);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnJoinGame,onJoinGame);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnExitGame,onExitGame);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickStartBtn,onClickStartBtn);
        }

        private void onExitGame(object obj)
        {
            _broadcastGamePlay?.Stop();
            _broadcastGamePlay = null;
        }

        private void onJoinGame(object obj)
        {
            var gameplay = obj as PB_OnlineGame;
            _selectGamePlayID = gameplay.GamePlayID;
            _broadcastGamePlay?.Stop();
            _broadcastGamePlay = broadcastOnlineGame(gameplay).Start();
        }

        private void onCreateGame(object obj)
        {
            var gameplay = obj as PB_OnlineGame;
            _selectGamePlayID = gameplay.GamePlayID;
            _broadcastGamePlay?.Stop();
            _broadcastGamePlay = broadcastOnlineGame(gameplay).Start();
        }

        private void onClickBack(object obj)
        {
            OnDisable();
            _playManager.Module<LobbyModule>().OnEnable();
        }

        IEnumerator broadcastOnlineGame(PB_OnlineGame data)
        {
            var sendData = NetworkUtility.Encode(NetworkUtility.APP_ID,NetworkUtility.GAMELOBBY_GPID,ONLINE_GAME_CMD,data.ToByteArray());
            while (true)
            {
                UdpBroadcastUtility.Send(sendData);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void onReceive(byte[] obj)
        {
            // Debug.Log(obj.Length);
            ushort appID=0,length=0;
            uint cmd=0;
            long gameplayID=0;
            if(obj.Length>=NetworkUtility.PACK_HEAD_LENGTH && NetworkUtility.Decode(obj,ref appID,ref length,ref gameplayID,ref cmd))
            {
                if(appID==NetworkUtility.APP_ID && length==obj.Length && gameplayID==NetworkUtility.GAMELOBBY_GPID)
                {
                    // Debug.LogWarning("cmd "+cmd);
                    if(cmd==ONLINE_GAME_CMD)
                    {
                        PB_OnlineGame onlineGame = PB_OnlineGame.Parser.ParseFrom(obj,NetworkUtility.PACK_HEAD_LENGTH,length-NetworkUtility.PACK_HEAD_LENGTH);
                        lock(OnlineGameDicLock)
                        {
                            addOnlineGame(Time.time,onlineGame);
                        }
                    }
                    else if(cmd==NetworkUtility.GAMESTART_CMD)
                    {
                        PB_GameStart gameStart = PB_GameStart.Parser.ParseFrom(obj,NetworkUtility.PACK_HEAD_LENGTH,length-NetworkUtility.PACK_HEAD_LENGTH);
                        enterGame(gameStart).Enqueue();
                    }
                }
            }
        }

        IEnumerator enterGame(PB_GameStart gameStart)
        {
            if(isEnterGame)
            {
                yield break;
            }
            isEnterGame=true;
            var gameData = _playManager.GameDatas[gameStart.GameID];
            
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            var playManager = assembly.CreateInstance(gameData.TypeName) as PlayManager; 

            _playManager.Destroy();
            playManager.Create(gameStart);
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

        private void onClickStartBtn(object obj)
        {
            PB_GameStart gameStart = new PB_GameStart();
            List<PB_OnlineGame> list = new List<PB_OnlineGame>();
            foreach (var item in OnlineGameDic)
            {
                if(item.Value.GamePlayID==_selectGamePlayID)
                {
                    list.Add(item.Value);
                }
            }
            list.Sort((l,r)=>{return (int)(l.JoinTime-r.JoinTime);});
            gameStart.GamePlayID=_selectGamePlayID;
            gameStart.GameID=list[0].GameID;
            for (int i = 0; i < list.Count; i++)
            {
                gameStart.Players.Add(new PB_Player()
                {
                    PlayerInfo = list[i].Player,
                    PlaceIndex = i,
                });
            }
            
            var sendData = NetworkUtility.Encode(NetworkUtility.APP_ID,NetworkUtility.GAMELOBBY_GPID,NetworkUtility.GAMESTART_CMD,gameStart.ToByteArray());
            UdpBroadcastUtility.Send(sendData);
            UdpBroadcastUtility.Send(sendData);
        }


    }
}
