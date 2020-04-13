namespace Beranek.ErrorComponents
{
    using System;
    public class TimedAction<T1, T2> : TimedAction
    {
        Action<T1, T2> _act;
        T1 _arg1;
        T2 _arg2;

        public TimedAction(Action<T1, T2> act, T1 arg1, T2 arg2) : base()
        {
            _act = act;
            _arg1 = arg1;
            _arg2 = arg2;
            MyAction = () => Invoke(_arg1, _arg2);

        }
        public TimedAction(Action<T1, T2> act) : base()
        {
            _act = act;
        }

        public void SetParameter(T1 arg1, T2 arg2)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            MyAction = () => Invoke(_arg1, _arg2);
        }

        private void Invoke(T1 arg1, T2 arg2)
        {
            _act.Invoke(arg1, arg2);
        }
    }
}
