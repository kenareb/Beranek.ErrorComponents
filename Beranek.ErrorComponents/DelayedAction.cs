namespace Beranek.ErrorComponents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class DelayedAction : IDisposable
    {
        CancellationTokenSource _breaker = new CancellationTokenSource();

        protected Action MyAction { get; set; }
        private bool _goBackToOriginalThread = false;

        public DelayedAction() { }

        public DelayedAction(Action act)
        {
            MyAction = act;
        }

        public DelayedAction(Action act, bool goBackToOriginalThread)
        {
            MyAction = act;
            _goBackToOriginalThread = goBackToOriginalThread;
        }

        public async Task<bool> RunWithDelayAsync(int delay)
        {            
            await Task.Delay(delay, _breaker.Token)
                .ContinueWith(s => DoAction(), _breaker.Token)
                .ConfigureAwait(_goBackToOriginalThread);

            return true;
        }

        private void DoAction()
        {
            MyAction?.Invoke();
        }

        public void Cancel()
        {
            _breaker.Cancel();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _breaker.Dispose();
                    _breaker = null;
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
