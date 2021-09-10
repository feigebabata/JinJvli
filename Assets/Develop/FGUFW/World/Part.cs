

namespace FGUFW.Play
{
    public abstract class Part<T> : PartBase where T : WorldBase
    {
        protected T _world;

        protected Part(WorldBase world) : base(world)
        {
            _world = world as T;
        }
    }

    public abstract class PartBase : IPart
    {
        public bool Enabled{get;private set;}
        public PartBase(WorldBase playManager)
        {

        }

        public virtual void Dispose()
        {
            
        }

        public virtual void OnDisable()
        {
            Enabled=false;
        }

        public virtual void OnEnable()
        {
            Enabled=true;
        }
    }
}