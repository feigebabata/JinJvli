using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePlay.GameLobby;

public class Test : MonoBehaviour
{
    DefaultModuleCtrl _defaultModuleCtrl;

    // Start is called before the first frame update
    void Awake()
    {
        _defaultModuleCtrl=new DefaultModuleCtrl();
        _defaultModuleCtrl.Enable();
        _defaultModuleCtrl.Move.PC.performed+=(val)=>
        {
            Debug.Log(val.ReadValue<Vector2>());
        };
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_defaultModuleCtrl.Move.PC.ReadValue<Vector2>());
    }
}
