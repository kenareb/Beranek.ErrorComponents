namespace Beranek.ErrorComponents
{
    using System;
    using System.Threading;
    public class DelayedFunc<T, TResult> : DelayedFunc<TResult>
    {
        private T _arg;
        private Func<T, TResult> _fun;

        public DelayedFunc() { }

        public DelayedFunc(Func<T, TResult> fun)
        {
            _fun = fun;
            MyFunc = () => fun(_arg);
        }
        public DelayedFunc(Func<T, TResult> fun, T arg)
        {
            _fun = fun;
            _arg = arg;
            MyFunc = () => DoInvoke(_arg);
        }

        private TResult DoInvoke(T arg)
        {
            return _fun.Invoke(arg);
        }

        public void SetParameter(T arg)
        {
            _arg = arg;
            MyFunc = () => DoInvoke(_arg);
        }
    }
}
