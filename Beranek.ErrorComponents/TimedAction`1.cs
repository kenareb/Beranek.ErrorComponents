namespace Beranek.ErrorComponents
{
    using System;
    public class TimedAction<T> : TimedAction
    {
        Action<T> _act;
        T _arg;

        public TimedAction(Action<T> act, T arg) : base()
        {
            _act = act;
            _arg = arg;
            MyAction = () => Invoke(_arg);
        }

        public TimedAction(Action<T> act) : base()
        {
            _act = act;
        }

        public void SetParameter(T arg)
        {
            _arg = arg;
            MyAction = () => Invoke(_arg);
        }

        private void Invoke(T arg)
        {
            _act.Invoke(arg);
        }

    }
}
