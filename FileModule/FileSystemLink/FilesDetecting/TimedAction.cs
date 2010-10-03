namespace FileModule
{
    using System;
    using System.Threading;

    public class TimedAction
    {
        private readonly FsObject<RelPath> _descriptor;
        protected readonly TimerCallback _callback;
        protected readonly long _dueTime;
        private readonly long _period;
        private readonly Timer _tickTimer;
        private bool _timerStopped;
       
        public bool IsTimerStopped
        {
            get { return _timerStopped; }
        }

        public TimedAction(FsObject<RelPath> descriptor, TimerCallback callback, long dueTime, long period)
        {
            _descriptor = descriptor;
            _callback = callback;
            _dueTime = dueTime;
            _period = period;
            _tickTimer = new Timer(callback, this, dueTime, period);
        }

        public FsObject<RelPath> Descriptor
        {
            get { return _descriptor; }
        }
        public RelPath Path
        {
            get
            {
                return _descriptor.Path;
            }
        }
        public void Stop()
        {
            if (_timerStopped)throw new InvalidOperationException();
            _timerStopped = true;
            _tickTimer.Dispose();
        }
            
        public virtual TimedAction Clone()
        {
            return new TimedAction(_descriptor,  _callback,_dueTime, _period);
        }
    }
}