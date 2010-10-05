// Author: Piotr Trzpil

namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using CodeExecutionTools.Logging;

    #endregion

    //public delegate void QueedJob();

    public class QueingThread : IDisposable
    {
        private readonly BlockingCollection<Action> _queue;
        private readonly Thread _thread;
        private bool _disposed;

        private bool _running;

        public QueingThread()
        {
            _queue = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
            _thread = new Thread(Run);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _running = false;
            _disposed = true;
            _queue.CompleteAdding();
        }

        #endregion

        public void Start()
        {
            if (!_running && !_disposed)
            {
                _running = true;
                _thread.Start();
            }
            else
            {
                throw new InvalidOperationException("Is running or disposed.");
            }
        }

        public void Add(Action action)
        {
            _queue.Add(action);
        }


        private void Run()
        {
            foreach (Action action in _queue.GetConsumingEnumerable())
            {
//#if !DEBUG
//                try
//                {
//#endif     
                action();
//#if !DEBUG
//                }
//                catch (Exception e)
//                {
//                    LoggingService.Trace.Error(e.toString());
                    // Kill the program anyway.
//                    throw;
//                }
//#endif
            }
        }


    }
}