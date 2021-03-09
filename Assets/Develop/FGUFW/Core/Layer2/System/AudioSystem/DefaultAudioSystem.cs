using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Diagnostics;

namespace FGUFW.Core.System
{
    public class DefaultAudioSystem : IAudioSystem
    {
        private IDictionary<string,AudioClip> _audioClipCache = new Dictionary<string,AudioClip>();
        private GameObject _gameObject;
        private List<AudioSource> _audioSourceCache = new List<AudioSource>();

        public void OnInit()
        {
            _gameObject = new GameObject("DefaultAudioSystem");
            _gameObject.AddComponent<AudioListener>();
            GameObject.DontDestroyOnLoad(_gameObject);
        }

        public void OnRelease()
        {
            foreach (var item in _audioSourceCache)
            {
                item.clip=null;
                GameObject.Destroy(item);
            }
            _audioSourceCache.Clear();
            _audioClipCache.Clear();
            GameObject.Destroy(_gameObject);
        }

        public void Play(string assetPath, AudioAssetMode playMode=AudioAssetMode.Asset)
        {
            #if !UNITY_WEBGL
            play(assetPath,playMode).Start();
            #endif
        }

        IEnumerator play(string assetPath, AudioAssetMode playMode=AudioAssetMode.Asset)
        {
            AudioSource audioSource = _audioSourceCache.Find((source)=>{return !source.isPlaying;});
            if(!audioSource)
            {
                audioSource = _gameObject.AddComponent<AudioSource>();
                _audioSourceCache.Add(audioSource);
            }
            AudioClip audioClip = null;
            if(_audioClipCache.ContainsKey(assetPath))
            {
                audioClip = _audioClipCache[assetPath];
            }
            else
            {
                if(playMode==AudioAssetMode.Asset)
                {
                    var loader = Addressables.LoadAssetAsync<AudioClip>(assetPath);
                    yield return loader;
                    audioClip = loader.Result;
                }
                else if(playMode==AudioAssetMode.File)
                {

                }
                _audioClipCache.Add(assetPath,audioClip);
            }
            audioSource.clip = audioClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        
    }
}