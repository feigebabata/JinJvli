using System;

namespace FGUFW.Core.AsynIO
{
    /// <summary>
    /// 异步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsynLoad<T>
    {
        /// <summary>
        /// resID==null/Empty 则取消加载
        /// </summary>
        /// <param name="resID"></param>
        /// <param name="callback"></param>
        void Load(string resID,Action<T> callback);
    }
}