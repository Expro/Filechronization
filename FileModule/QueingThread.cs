// Author: Piotr Trzpil

namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    #endregion

    //public delegate void QueedJob();

    public class QueingThread : IDisposable
    {
        private readonly BlockingCollection<Action> queue;
        private readonly Thread thread;


        private bool running;

        public QueingThread()
        {
            queue = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
            thread = new Thread(Run);


            running = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            running = false;
            queue.CompleteAdding();
        }

        #endregion

        public void Start()
        {
            if (!running)
            {
                running = true;
                thread.Start();
            }
        }

        public void Add(Action comm)
        {
            queue.Add(comm);
        }


        private void Run()
        {
            foreach (Action action in queue.GetConsumingEnumerable())
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
//                    Console.Out.WriteLine(e);
//                }
//#endif
            }
        }


//        private readonly BlockingQueue<QueedJob> queue;
//        private readonly Thread thread;
//
//        private bool running;
//
//        public QueingThread()
//        {
//            queue = new BlockingQueue<QueedJob>();
//            thread = new Thread(Run);
//            running = false;
//        }
//
//        public void Start()
//        {
//            Monitor.Enter(queue);
//            if (!running)
//            {
//                running = true;
//                thread.Start();
//            }
//            Monitor.Exit(queue);
//        }
//
//        public void Stop()
//        {
//   
//            running = false;
//            queue.Unblock();
//
//
//        }
//
//        public void Add(QueedJob comm)
//        {
//            queue.Enqueue(comm);
//        }
//
//        public void Clear()
//        {
//            queue.Clear();
//        }
//
//        private void Run()
//        {
//            while (running)
//            {
//                var task = queue.Dequeue();
//                if(task!= null)
//                {
//                    task();
//                }
//                
//            }
//        }
    }
}