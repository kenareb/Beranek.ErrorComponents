namespace Beranek.ErrorComponents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class DelayedFunc<TResult> : IDisposable
    {
        CancellationTokenSource _breaker = new CancellationTokenSource();

        protected Func<TResult> MyFunc { get; set; }
        private bool _goBackToOriginalThread = false;

        public DelayedFunc() { }

        public DelayedFunc(Func<TResult> fun)
        {
            MyFunc = fun;
        }

        public DelayedFunc(Func<TResult> fun, bool goBackToOriginalThread)
        {
            MyFunc = fun;
            _goBackToOriginalThread = goBackToOriginalThread;
        }

        public async Task<TResult> RunWithDelayAsync(int delay)
        {
            TResult result = default(TResult);
            await Task.Delay(delay, _breaker.Token)
                .ContinueWith(s => result = DoInvoke(), _breaker.Token)
                .ConfigureAwait(_goBackToOriginalThread);

            return result;
        }

        private TResult DoInvoke()
        {
            return MyFunc.Invoke();
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
