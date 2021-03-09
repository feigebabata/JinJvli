using System.Threading.Tasks;
using UnityEngine;
namespace FGUFW.Core.System
{
    public interface IAudioSystem :ISystem
    {
        void Play(string assetPath,AudioAssetMode playMode=AudioAssetMode.Asset);
    }

    public enum AudioAssetMode
    {
        Asset,
        File,
    }
}