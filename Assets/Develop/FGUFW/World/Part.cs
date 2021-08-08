

namespace FGUFW.Play
{
    public abstract class Part<T> : PartBase where T : WorldBase
    {
        protected T _playManager;
        private bool _isInit;

        protected Part(WorldBase playManager) : base(playManager)
        {
            _playManager = playManager as T;
        }
    }

    public abstract class PartBase : IPart
    {
        public PartBase(WorldBase playManager)
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