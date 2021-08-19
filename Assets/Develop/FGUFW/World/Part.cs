

namespace FGUFW.Play
{
    public abstract class Part<T> : PartBase where T : WorldBase
    {
        protected T _world;
        private bool _isInit;

        protected Part(WorldBase world) : base(world)
        {
            _world = world as T;
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