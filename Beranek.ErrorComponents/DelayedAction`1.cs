﻿namespace Beranek.ErrorComponents
{
    using System;
    public class DelayedAction<T> : DelayedAction
    {
        private Action<T> _act;
        private T _arg;

        public DelayedAction(Action<T> act, T arg) : base()
        {
            _act = act;
            _arg = arg;
            MyAction = () => DoInvoke(_arg);
        }

        public DelayedAction(Action<T> act) : base()
        {
            _act = act;
        }

        public void SetParameter(T arg)
        {
            _arg = arg;
            MyAction = () => DoInvoke(_arg);
        }

        private void DoInvoke(T arg)
        {
            _act.Invoke(arg);
        }
    }
}
