using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
using System.IO;
using FGUFW.Core;
using Worlds.Battle;

public class MultiLanguageSelect : EditorWindow
{
    string filePath = "/Develop/CSV/MultiLanguageConfig.csv";
    List<string> _ml_ls = new List<string>();
    List<string> _search_ls = new List<string>();
    string[][] _config;
    MultiLanguageCompBase _comp;
    private ListView _ml_lsView;
    private ListView _search_lsView;
    private string _textId;

    [MenuItem("多语言/选择翻译文本")]
    public static void ShowExample()
    {
        MultiLanguageSelect wnd = GetWindow<MultiLanguageSelect>();
        wnd.titleContent = new GUIContent("MultiLanguageSelect");
    }

    // private void OnGUI()
    // {
    //     if(Event.current.rawType==EventType.KeyDown)
    //     {
    //         EventCallBack(Event.current);
    //     }
    // }

    // private void EventCallBack(Event e)
    // {
    //     bool eventDown = (e.modifiers & EventModifiers.Control) != 0;
 
    //     if (!eventDown) return;
 
    //     e.Use();        //使用这个事件
 
    //     switch(e.keyCode)
    //     {
    //         case KeyCode.S:
    //         {
    //             if(_ml_lsView.selectedIndex>-1 && _ml_lsView.selectedIndex<_ml_ls.Count)
    //             {
    //                 GUIUtility.systemCopyBuffer = _ml_ls[_ml_lsView.selectedIndex];
    //                 Debug.Log("复制:"+_ml_ls[_ml_lsView.selectedIndex]);
    //             }
    //         }
    //         break;
    //     }
        
    // }

    private void loadConfig()
    {
        var lines = File.ReadAllText(Application.dataPath+filePath).ToCsvLines();
        var lineCount = lines.Length;
        var itemCount = lines[0].Split(',').Length;
        _config = new string[lineCount][];
        for (int i = 0; i < lineCount; i++)
        {
            var items = lines[i].Split(',');
            _config[i] = new string[itemCount];
            for (int j = 0; j < items.Length; j++)
            {
                try
                {
                    var item = items[j];
                    if(item.Contains("\n"))item = item.Substring(1,item.Length-2);
                    _config[i][j]=item;
                }
                catch (System.Exception)
                {
                    Debug.LogError($"{i}:{j}");
                }
            }
        }
    }

    public void CreateGUI()
    {
        loadConfig();

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;


        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // VisualElement label = new Label("Hello World! From C#");
        // root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Develop/FGUFW/MultiLanguage/Editor/MultiLanguageSelect/MultiLanguageSelect.uxml");

        visualTree.CloneTree(root);

        _ml_lsView = root.Q<ListView>("ML_LS");
        _ml_lsView.makeItem = onMS_LSMakeItem;
        _ml_lsView.bindItem = onMS_LSBindItem;
        _ml_lsView.itemsSource = _ml_ls;
        _ml_lsView.onSelectionChange += onML_SelectionChange;

        _search_lsView = root.Q<ListView>("Search_LS");
        _search_lsView.makeItem = onSearch_LSMakeItem;
        _search_lsView.bindItem = onSearch_LSBindItem;
        _search_lsView.itemsSource = _search_ls;
        _search_lsView.onSelectionChange += onSearch_SelectionChange;

        root.Q<TextField>("SearchInput").RegisterValueChangedCallback(onSearchValueChanged);
        rootVisualElement.Q<Button>("Confirm").clicked += onClickConfirm;
        rootVisualElement.Q<Button>("Copy").clicked += onClickCopy;


        checkSelection();
    }

    private void onClickCopy()
    {
        if(_ml_lsView.selectedIndex>-1 && _ml_lsView.selectedIndex<_ml_ls.Count)
        {
            GUIUtility.systemCopyBuffer = _ml_ls[_ml_lsView.selectedIndex];
            Debug.Log("复制:"+_ml_ls[_ml_lsView.selectedIndex]);
        }
    }

    private void onClickConfirm()
    {
        _comp.TextID = _textId;
        EditorUtility.SetDirty(_comp);
        resetTop();
    }

    private void onSearch_SelectionChange(IEnumerable<object> obj)
    {
        if(_search_lsView.selectedIndex==-1)
        {
            _textId = null;
            clearML_LS();
        }
        else
        {
            _textId = _search_ls[_search_lsView.selectedIndex].Split(':')[0];
            resetMLListView(_textId);
        }
    }

    private void clearML_LS()
    {
        _ml_ls.Clear();
        _ml_lsView.itemsSource = _ml_ls;
        rootVisualElement.Q<Button>("Copy").SetEnabled(_ml_lsView.selectedIndex!=-1);
    }

    private void onML_SelectionChange(IEnumerable<object> obj)
    {
        rootVisualElement.Q<Button>("Copy").SetEnabled(_ml_lsView.selectedIndex!=-1);
        var index = _ml_lsView.selectedIndex;
        if(index==-1)return;
        
        if(_comp==null)return;
        MultiLanguage.SetLanguage(index);
        _comp.SetMLText(_ml_ls[index]);
        EditorUtility.SetDirty(_comp);
    }

    private void onSearchValueChanged(ChangeEvent<string> evt)
    {
        getSearchData(_config,evt.newValue,_search_ls);
        _search_lsView.itemsSource = _search_ls;
        _search_lsView.SetSelection(-1);
    }

    private void onSearch_LSBindItem(VisualElement arg1, int arg2)
    {
        var label = arg1 as Label;
        label.text = _search_ls[arg2];
    }

    private VisualElement onSearch_LSMakeItem()
    {
        var label = new Label();
        label.style.unityTextAlign = TextAnchor.MiddleLeft;
        label.style.marginLeft = 20;
        label.style.borderBottomWidth = 1;
        label.style.borderBottomColor = Color.gray;
        return label;
    }

    private void onMS_LSBindItem(VisualElement arg1, int arg2)
    {
        var label = arg1 as Label;
        label.text = _ml_ls[arg2];
    }

    private VisualElement onMS_LSMakeItem()
    {
        var label = new Label();
        label.style.unityTextAlign = TextAnchor.MiddleLeft;
        label.style.marginLeft = 20;
        label.style.borderBottomWidth = 1;
        label.style.borderBottomColor = Color.gray;
        return label;
    }

    void OnSelectionChange()
    {
        checkSelection();
    }

    void OnFocus()
    {
        checkSelection();
    }

    void checkSelection()
    {
        _comp = null;
        _comp = Selection.activeGameObject?.GetComponent<MultiLanguageCompBase>();
        resetTop();
        if(_comp)
        {
            resetMLListView(_comp.TextID);
        }
    }

    private void resetMLListView(string textId)
    {
        getMLData(_config,textId,_ml_ls);
        _ml_lsView.itemsSource = _ml_ls;
        _ml_lsView.SetSelection(-1);
        rootVisualElement.Q<Button>("Copy").SetEnabled(_ml_lsView.selectedIndex!=-1);
    }

    void resetTop()
    {
        if(_comp==null)
        {
            rootVisualElement.Q<Label>("Tip").text = "未选中";
        }
        else
        {
            rootVisualElement.Q<Label>("Tip").text = $"{_comp.name}:{_comp.TextID}";
        }
        rootVisualElement.Q<Button>("Confirm").SetEnabled(_comp!=null);
    }

    static void getMLData(string[][] config,string textId,List<string> ml_ls)
    {
        ml_ls.Clear();
        foreach (var item in config)
        {
            if(item[0]==textId)
            {
                var length = item.Length;
                for (int i = 1; i < length; i++)
                {
                    ml_ls.Add(item[i]);
                }
            }
        }
    }

    static void getSearchData(string[][] config,string text,List<string> search_ls)
    {
        search_ls.Clear();
        if(string.IsNullOrEmpty(text))return;
        foreach (var item in config)
        {
            if(item[1].Contains(text))
            {
                search_ls.Add($"{item[0]}:{item[1]}");
            }
        }
    }

}