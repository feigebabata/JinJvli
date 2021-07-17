

namespace FGUFW.Play
{
    public abstract class PlayModule<T> : PlayModule where T : PlayManager
    {
        protected T _playManager;
        private bool _isInit;

        protected PlayModule(PlayManager playManager) : base(playManager)
        {
            _playManager = playManager as T;
        }
    }

    public abstract class PlayModule : IPlayModule
    {
        public PlayModule(PlayManager playManager)
        {

        }

        public virtual void Dispose()
        {
            
        }

        public virtual void OnDisable()
        {
            
        }

        public virtual void OnEnable()
        {
            
        }
    }
}