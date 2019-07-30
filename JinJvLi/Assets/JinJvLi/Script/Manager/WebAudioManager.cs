using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AvatarProject
{
    public class WebAudioManager : IManager
    {
        public static string AUDIO_CACHE_PATH;
        Dictionary<string,Action<AudioClip>> waiting = new Dictionary<string,Action<AudioClip>>();
        Dictionary<string,AudioClip> cache = new Dictionary<string, AudioClip>();
        public void Clear()
        {
            waiting.Clear();
        }

        public void ClearTexCache()
        {
            string[] files = Directory.GetFiles(AUDIO_CACHE_PATH);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

        public void Init()
        {
            AUDIO_CACHE_PATH = Path.Combine(Application.persistentDataPath,"audioCache");
            AUDIO_CACHE_PATH = AUDIO_CACHE_PATH.Replace("\\","/");
            DebugHelper.LogWarning(AUDIO_CACHE_PATH);
            if(!Directory.Exists(AUDIO_CACHE_PATH))
            {
                Directory.CreateDirectory(AUDIO_CACHE_PATH);
            }
        }

        public void LatedUpdate()
        {
            
        }

        public void Reset()
        {
            
        }

        public void Update()
        {
            // throw new System.NotImplementedException();
        }

        public string GetLocalPath(string _url)
        {
            string localPath = _url;
            if(!File.Exists(_url))
            {
                string fileName = MD5Code.GetMD5HashFromData(Encoding.UTF8.GetBytes(_url))+Path.GetExtension(_url);
                localPath = $"{AUDIO_CACHE_PATH}/{fileName}";
            }
            else{
                //本地音乐 by vivian 如果本地音乐路径存在空格，那么将文件拷贝一份到audioCache并且去掉新文件名的空格
                int spaceIndex = localPath.IndexOf(' ');
                if (spaceIndex != -1)
                {
                    string newFile = localPath.Replace(' ', '0');//如果有空格就以0占位吧
                    if (File.Exists(localPath))
                    {
                        int endQuotation = newFile.LastIndexOf('/');
                        string fName = newFile.Substring(endQuotation + 1);
                        newFile = System.IO.Path.Combine(AUDIO_CACHE_PATH,fName);
                        if(File.Exists(newFile))
                            File.Delete(newFile);
                        File.Copy(localPath, newFile);
                        localPath = newFile;
                        DebugHelper.LogWarning($"-*-本地音乐文件包含空格，生成新文件代替:{newFile}");
                    }
                }
            }
            return localPath;
        }

        public void GetAudioClip(string _url,Action<AudioClip> _callback)
        {
            if(cache.ContainsKey(_url))
            {
                _callback(cache[_url]);
            }
            else
            {
                addWaiting(_url,_callback);
                if(File.Exists(_url))
                {
                    Coroutines.Run(loadAudioClip(new Uri(_url),_url));
                }
                else
                {
                    string savePath = GetLocalPath(_url);
                    if(File.Exists(savePath))
                    {
                        Coroutines.Run(loadAudioClip(new Uri(savePath),_url));
                    }
                    else
                    {
                        downloadAudioClip(_url,_url,_callback);
                    }
                }
            }
        }

        void addWaiting(string _url,Action<AudioClip> _callback)
        {
            if(waiting.ContainsKey(_url))
            {
                waiting[_url]+=_callback;
            }
            else
            {
                waiting.Add(_url,_callback);
            }
        }

        void downloadAudioClip(string _url,string _key,Action<AudioClip> _callback)
        {
            string savePath = GetLocalPath(_url);
            AvatarManager.Manager<AsynDownloadManager>().Download(_url,savePath,(isSucc)=>
            {
                if(isSucc)
                {
                    Coroutines.Run(loadAudioClip(new Uri(savePath),_key));
                }
                else
                {
                    error($"下载失败",_url);
                }
            });
        }

        IEnumerator loadAudioClip(Uri _url,string _key)
        {
            yield return null;
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(_url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.isHttpError || www.isNetworkError)
                {
                    error($"加载失败 {www.error}",_key);
                }
                else
                {
                    AudioClip clip=null;
                    try
                    {
                        clip=DownloadHandlerAudioClip.GetContent(www);
                    }
                    catch
                    {
                        DebugHelper.LogError($"[WebAudioManager.loadAudioClip]");
                    }
                    if(clip!=null && clip.length>0)
                    {
                        cache[_key]=clip;
                        waiting[_key](clip);
                    }
                    else
                    {
                        waiting[_key](null);
                    }
                    waiting.Remove(_key);
                }
            }
        }

        void error(string _info,string _key)
        {
            waiting[_key](null);
            waiting.Remove(_key);
            DebugHelper.LogError($"[WebAudioManager]{_info}\n{_key}");
        }

    }
}