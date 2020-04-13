namespace Beranek.ErrorComponents
{
    using System;
    using System.Threading;

    public class TimedAction : IDisposable
    {
        protected Action MyAction { get; set; }
        private Timer _timer;
        private object _state;      

        public TimedAction() { }

        public TimedAction(Action act)
        {
            MyAction = act;
        }

        public void RunWithDelay(int delay)
        {
            _timer = new Timer(DoAction, _state, delay, Timeout.Infinite);
        }

        private void DoAction(object state)
        {
            MyAction?.Invoke();
        }

        public void Cancel()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                Dispose();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);            
        }
        #endregion
    }
}
