using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace JinJvli
{
    [PanelConfig("JJL_Panel/PathSelect",true)]
    public class PathSelect: PanelBase
    {
        public static class Config
        {
            public const string SELECT_PATH = "PathSelect.PATH";
            public const string ANY_FLIE_TYPE = ".*";
        }
        public class OpenData
        {
            public uint MaxSelectCount=1;
            public List<string> FileTypes=new List<string>(){Config.ANY_FLIE_TYPE};
            public Action<List<string>> Finsh;
        }
        List<string> m_pathRoots = new List<string>();
        string m_curDirPath=string.Empty;
        [SerializeField]
        UIList m_uiList;
        [SerializeField]
        Text m_dirPath,m_dirCount;
        string[] m_curDirs;
        string[] m_curFiles;
        OpenData m_openData;
        List<string> m_select = new List<string>();

        void Awake()
        {
            getPathRoots();
            m_uiList.m_ItemProvider += onItemProvider;
            m_uiList.m_ItemShow += onItemShow;
        }

        public override void OnOpen(object _openData = null)
        {
            m_openData = _openData as OpenData;
            m_curDirPath = PlayerPrefs.GetString(Config.SELECT_PATH);
            if(!Directory.Exists(m_curDirPath))
            {
                m_curDirPath=string.Empty;
            }
            getDirAndFile();
        }

        public override void OnClose()
        {
            PlayerPrefs.SetString(Config.SELECT_PATH,m_curDirPath);
        }

        void getDirAndFile()
        {
            m_select.Clear();
            if(string.IsNullOrEmpty(m_curDirPath))
            {
                m_curDirs = m_pathRoots.ToArray();
                m_curFiles = new string[0];
            }
            else
            {
                m_curDirs = Directory.GetDirectories(m_curDirPath);
                m_curFiles = Directory.GetFiles(m_curDirPath);
                if(!m_openData.FileTypes.Contains(Config.ANY_FLIE_TYPE))
                {
                    List<string> files = new List<string>();
                    for (int i = 0; i < m_curFiles.Length; i++)
                    {
                        if(m_openData.FileTypes.Contains(Path.GetExtension(m_curFiles[i])))
                        {
                            files.Add(m_curFiles[i]);
                        }
                    }
                    m_curFiles = files.ToArray();
                }
            }
            m_uiList.ItemNum=0;
            m_uiList.ItemNum=(uint)(m_curDirs.Length+m_curFiles.Length);
            m_dirPath.text = m_curDirPath;
            m_dirCount.text = (m_curDirs.Length+m_curFiles.Length).ToString();
        }

        void getPathRoots()
        {
            m_pathRoots.Clear();
            var drives = DriveInfo.GetDrives();
            #if ANDROID && !UNITY_EDITOR
            for (int i = 0; i < drives.Length; i++)
            {
                if(Application.persistentDataPath.StartsWith(drives[i].Name))
                {
                    m_pathRoots.Add(drives[i].Name);
                }
            }
            #else
            for (int i = 0; i < drives.Length; i++)
            {
                m_pathRoots.Add(drives[i].Name);
            }
            #endif
        }

        void nextDir(string _path)
        {
            try
            {
                Directory.GetDirectories(_path);
            }
            catch
            {
                return;
            }
            m_curDirPath = _path;
            getDirAndFile();
        }

        public void OnClickBackDir()
        {
            if(m_pathRoots.Contains(m_curDirPath))
            {
                m_curDirPath = null;
            }
            else
            {
                m_curDirPath = Path.GetDirectoryName(m_curDirPath);
            }
            getDirAndFile();
        }

        public void OnClickFinsh()
        {
            if(m_openData.Finsh!=null)
            {
                m_openData.Finsh(m_select);
            }
        }

        void onItemShow(int _index, RectTransform _item)
        {
            _item.GetComponent<Button>().onClick.RemoveAllListeners();
            if(_index<m_curDirs.Length)
            {
                if(string.IsNullOrEmpty(m_curDirPath))
                {
                    _item.GetChild(0).GetComponent<Text>().text = m_curDirs[_index];
                }
                else
                {
                    _item.GetChild(0).GetComponent<Text>().text = Path.GetFileName(m_curDirs[_index]);
                }
                _item.GetComponent<Button>().onClick.AddListener(()=>
                {
                    nextDir(m_curDirs[_index]);
                });
            }
            else
            {
                _item.GetChild(0).GetComponent<Text>().text = Path.GetFileName(m_curFiles[_index-m_curDirs.Length]);
                _item.GetChild(1).GetComponent<Text>().text = Path.GetExtension(m_curFiles[_index-m_curDirs.Length]);
                _item.GetChild(2).gameObject.SetActive(false);
                _item.GetComponent<Button>().onClick.AddListener(()=>
                {
                    selectItem(_index,_item.GetChild(2).gameObject);
                });
            }
        }

        void selectItem(int _index,GameObject _select)
        {
            if(_select.activeSelf)
            {
                m_select.Remove(m_curFiles[_index-m_curDirs.Length]);
                _select.SetActive(false);
            }
            else
            {
                if(m_select.Count>=m_openData.MaxSelectCount)
                {
                    return;
                }
                m_select.Add(m_curFiles[_index-m_curDirs.Length]);
                _select.SetActive(true);
            }
        }

        int onItemProvider(int _index)
        {
            if(_index<m_curDirs.Length)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}