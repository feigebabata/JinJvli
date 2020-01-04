using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Addressables.InstantiateAsync("Assets/JinJvLi/Res/JJL_Panel/LoginPanel.prefab").Completed += (handle)=>
        {
            Debug.Log(handle.Result);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
