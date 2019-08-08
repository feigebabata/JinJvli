using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JinJvLi.Lobby;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FGBBT : MonoBehaviour
{
    UIList m_uiList;
    // Start is called before the first frame update
    void Start()
    {
        m_uiList = GetComponent<UIList>();
        m_uiList.m_ItemProvider+=itemProvider;
        m_uiList.m_ItemShow+=itemRenderer;
        m_uiList.m_ItemHide+=itemHide;
        m_uiList.ItemNum=50;
    }

    private void itemHide(int arg1, RectTransform arg2)
    {
        Debug.Log("hide index "+arg1);
    }

    private void itemRenderer(int arg1, RectTransform arg2)
    {
        Debug.Log("show index "+arg1);
    }

    private int itemProvider(int arg)
    {
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [MenuItem("FGBBT/Run")]
    public static void Run()
    {
        PB_GameRoom gameRoom = new PB_GameRoom();
        Debug.Log(gameRoom.ToString());
    }
}
