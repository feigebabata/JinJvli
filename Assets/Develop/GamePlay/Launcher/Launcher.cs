using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        new GamePlay.GameLobby.GameLobbyPlayManager().Create();  
    } 
}
