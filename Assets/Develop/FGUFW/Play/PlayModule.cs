

namespace FGUFW.Play
{
    public abstract class PlayModule<T> : IPlayModule where T : PlayManager
    {
        protected T _playManager;
        private bool _isInit;
        public bool IsInit => _isInit;

        public virtual void OnInit(PlayManager playManager)
        {
            if(!_isInit)
            {
                _playManager = playManager as T;
                _isInit = true;
            }
        }

        public virtual void OnRelease()
        {
            _isInit = false;
        }

        public virtual void OnShow(){}

        public virtual void OnHide(){}

        protected void log(string d)
        {

        }
    }
}