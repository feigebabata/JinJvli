using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.GameLobby
{
    public class OnlineGameModuleOutput : IDisposable
    {
        GameLobbyPlayManager _playManager;
        public OnlineGameModuleOutput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
        }

        public void Dispose()
        {
            _playManager.Messenger.Remove(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager = null;
        }

        private void onEnterOnlineGame(object obj)
        {
            var endPos = new Vector3(-3.8f,1,-6);
            var endAngle = new Vector3(-10,-90,0);
            float aniTime = 2;
            var character = GameObject.Find("character").transform;
            character.MoveLocal(endPos,aniTime).Start();
            Camera.main.transform.RotateLocal(endAngle,aniTime).Start();
            waitEnterOnlineGameUI(aniTime).Start();
            
            Cursor.lockState = CursorLockMode.None;
        }

        IEnumerator waitEnterOnlineGameUI(float time)
        {
            yield return new WaitForSeconds(time);
        }

    }
}
