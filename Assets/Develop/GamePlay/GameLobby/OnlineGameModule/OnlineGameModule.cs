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
    public class  OnlineGameModule : Part<GameLobbyPlayManager>
    {
        public Dictionary<long,PB_OnlineGame> OnlineGameDic = new Dictionary<long, PB_OnlineGame>();
        public object OnlineGameDicLock = new object();
        public PB_PlayerInfo SelfInfo;
        
        private  OnlineGameModuleInput _moduleInput;
        private  OnlineGameModuleOutput _moduleOutput;
        public long SelectGamePlayID;
        private bool _isEnterGame=false;
        private SyncClient _syncClient;


        public OnlineGameModule(WorldBase playManager) : base(playManager)
        {
            SelfInfo = new PB_PlayerInfo()
            {
                ID = SystemInfo.deviceUniqueIdentifier,
                Nickname = ConfigDatabase.GetConfig("nickname",SystemInfo.deviceName),
            };
            _moduleOutput = new  OnlineGameModuleOutput(_playManager);
            _moduleInput = new  OnlineGameModuleInput(_playManager);
            UdpBroadcastUtility.Init();
            UdpBroadcastUtility.OnReceive += onReceiveBroadcast;
            
        }

        public override void Dispose()
        {
            UdpBroadcastUtility.OnReceive -= onReceiveBroadcast;

            
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
        }

        private void onJoinGame(object obj)
        {
            var gameplay = obj as PB_OnlineGame;
            SelectGamePlayID = gameplay.GamePlayID;
        }

        private void onCreateGame(object obj)
        {
            GameItemData gameItemData = obj as GameItemData;

            var gameplayServer = new SyncServer(gameItemData.PlayerMaxCount);

            PB_OnlineGame gameplay = new PB_OnlineGame();
            gameplay.GameID = gameItemData.ID;
            gameplay.GamePlayID = DateTime.Now.UnixMilliseconds();
            gameplay.IP = gameplayServer.LocalIPEndPoint.Address.ToString();
            gameplay.Port = gameplayServer.LocalIPEndPoint.Port;

            gameplayServer.Data = gameplay;

            SelectGamePlayID = gameplay.GamePlayID;
            
            createSyncClient(gameplay);
        }

        private void onClickBack(object obj)
        {
            OnDisable();
            _playManager.Part<LobbyModule>().OnEnable();
        }

        private void onReceiveBroadcast(byte[] obj)
        {
            ushort appID=0,length=0;
            uint cmd=0;
            long gameplayID=0;
            if(obj.Length>=NetworkUtility.PACK_HEAD_LENGTH && NetworkUtility.DecodeU(ref appID,ref length,ref gameplayID,ref cmd,obj,0,obj.Length))
            {
                if(appID==NetworkUtility.APP_ID && length==obj.Length && gameplayID==NetworkUtility.GAMELOBBY_GPID)
                {
                    switch(cmd)
                    {
                        case SyncServer.ONLINE_GAME_CMD:
                            PB_OnlineGame onlineGame = PB_OnlineGame.Parser.ParseFrom(obj,NetworkUtility.PACK_HEAD_LENGTH,length-NetworkUtility.PACK_HEAD_LENGTH);
                            lock(OnlineGameDicLock)
                            {
                                addOnlineGame(onlineGame);
                            }
                        break;
                    }
                }
            }
        }

        private void createSyncClient(PB_OnlineGame onlineGame)
        {
            _syncClient = new SyncClient(onlineGame.IP,onlineGame.Port);
            _syncClient.OnConnect+=onConnectServer;
            _syncClient.OnReceive+=onReceiveServer;
        }

        private int onReceiveServer(byte[] buffer, int bufLen)
        {
            ushort length=0;
            uint cmd=0;
            int index=0;
            if(bufLen>=NetworkUtility.PACK_HEAD_LENGTH)
            {
                while (true)
                {
                    NetworkUtility.DecodeT(ref length,ref cmd,buffer,index,bufLen);
                    if(length<=bufLen-index)
                    {
                        if(cmd==NetworkUtility.GAMESTART_CMD)
                        {

                        }
                    }
                }
            }
            // if(buffer.Length>=NetworkUtility.PACK_HEAD_LENGTH && NetworkUtility.DecodeU(ref appID,ref length,ref gameplayID,ref cmd,buffer,0,bufLen))
            // {
            //     if(appID==NetworkUtility.APP_ID && length==buffer.Length && gameplayID==NetworkUtility.GAMELOBBY_GPID)
            //     {
            //         // Debug.LogWarning("cmd "+cmd);
            //         if(cmd==ONLINE_GAME_CMD)
            //         {
            //             PB_OnlineGame onlineGame = PB_OnlineGame.Parser.ParseFrom(buffer,NetworkUtility.PACK_HEAD_LENGTH,length-NetworkUtility.PACK_HEAD_LENGTH);
            //             lock(OnlineGameDicLock)
            //             {
            //                 addOnlineGame(Time.time,onlineGame);
            //             }
            //             if(onlineGame.Player.ID == SelfInfo.ID && _syncClient==null)
            //             {
            //                 createSyncClient(onlineGame);
            //             }
            //         }
            //         // else if(cmd==NetworkUtility.GAMESTART_CMD)
            //         // {
            //         //     PB_GameStart gameStart = PB_GameStart.Parser.ParseFrom(obj,NetworkUtility.PACK_HEAD_LENGTH,length-NetworkUtility.PACK_HEAD_LENGTH);
            //         //     enterGame(gameStart).Enqueue();
            //         // }
            //     }
            // }
            return 0;
        }

        private void onConnectServer(string obj)
        {
            Debug.Log("服务器连接成功"+obj);
            PB_JoinGame joinGame = new PB_JoinGame
            {
                PlayerInfo = SelfInfo,
            };
            _syncClient.Send(joinGame.ToByteArray());
        }

        private 

        IEnumerator enterGame(PB_OnlineGame onlineGame)
        {
            if(_isEnterGame)
            {
                yield break;
            }
            _isEnterGame=true;
            var gameData = _playManager.GameDatas[onlineGame.GameID];
            
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            var playManager = assembly.CreateInstance(gameData.TypeName) as WorldBase; 

            _playManager.Destroy();
            playManager.Create(onlineGame,SelfInfo);
        }

        private void addOnlineGame(PB_OnlineGame onlineGame)
        {
            if(OnlineGameDic.ContainsKey(onlineGame.GamePlayID))
            {
                OnlineGameDic[onlineGame.GamePlayID]=onlineGame;
            }
            else
            {
                OnlineGameDic.Add(onlineGame.GamePlayID,onlineGame);
            }
        }

        private void onClickStartBtn(object obj)
        {
            PB_GameStart gameStart = new PB_GameStart();
            // List<PB_OnlineGame> list = new List<PB_OnlineGame>();
            // foreach (var item in OnlineGameDic)
            // {
            //     if(item.Value.GamePlayID==_selectGamePlayID)
            //     {
            //         list.Add(item.Value);
            //     }
            // }
            // list.Sort((l,r)=>{return (int)(l.JoinTime-r.JoinTime);});
            // gameStart.GamePlayID=_selectGamePlayID;
            // gameStart.GameID=list[0].GameID;
            // for (int i = 0; i < list.Count; i++)
            // {
            //     gameStart.Players.Add(new PB_Player()
            //     {
            //         PlayerInfo = list[i].Player,
            //         PlaceIndex = i,
            //     });
            // }
            
            var sendData = NetworkUtility.EncodeU(NetworkUtility.APP_ID,NetworkUtility.GAMELOBBY_GPID,NetworkUtility.GAMESTART_CMD,gameStart.ToByteArray());
            UdpBroadcastUtility.Send(sendData);
            UdpBroadcastUtility.Send(sendData);
        }


    }
}
