
using System.Collections;
using FGUFW.Core;

namespace FGUFW.Play
{
    public interface IPlayManager
    {
        T Module<T>() where T : IPlayModule;
        void Create();
        void Destroy();
    }
}