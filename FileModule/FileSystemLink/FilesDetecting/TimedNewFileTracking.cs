namespace FileModule
{
    using System.Threading;

    internal class TimedNewFileTracking : TimedAction
    {
        private bool _wasCheckedByTimer;

        public bool WasCheckedByTimer
        {
            get
            {
                return _wasCheckedByTimer;
            }
            set
            {
                _wasCheckedByTimer = value;
            }
        }

        public TimedNewFileTracking(FsObject<RelPath> descriptor, TimerCallback callback, long dueTile, long period)
            : base(descriptor, callback, dueTile, period)
        {
        }

    }
}