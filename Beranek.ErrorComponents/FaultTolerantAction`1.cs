namespace Beranek.ErrorComponents
{
    using System;

    public class FaultTolerantAction<T> : FaultTolerantAction
    {
        T _arg;
        Action<T> _act;

        public FaultTolerantAction(Action<T> act)
        {
            _act = act;
        }

        public FaultTolerantAction(Action<T> act, int maxTries)
        {
            _act = act;
            MaxTries = maxTries;
        }

        public FaultTolerantAction(Action<T> act, int maxTries, IRetryStrategy strategy)
        {
            _act = act;
            MaxTries = maxTries;
            Strategy = strategy;
        }

        public FaultTolerantAction(Action<T> act, int maxTries, IRetryStrategy strategy, T arg)
        {
            _act = act;
            MaxTries = maxTries;
            Strategy = strategy;
            _arg = arg;
            MyAction = () => DoInvoke(_arg);
        }

        public void SetParameter(T arg)
        {
            _arg = arg;
            MyAction = () => DoInvoke(_arg);
        }

        private void DoInvoke(T arg)
        {
            _act?.Invoke(arg);
        }
    }
}
