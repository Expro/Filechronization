namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    #endregion

    public delegate void QueedJob();

    public class QueingThread
    {
        private readonly BlockingCollection<QueedJob> queue;
        private readonly Thread thread;
        private readonly CancellationTokenSource tokenSource;

        private bool running;

        public QueingThread()
        {
            queue = new BlockingCollection<QueedJob>(new ConcurrentQueue<QueedJob>());
            thread = new Thread(Run);
            tokenSource = new CancellationTokenSource();

            running = false;
        }

        public void Start()
        {
            //Monitor.Enter(queue);
            if (!running)
            {
                running = true;
                thread.Start();
            }
            //Monitor.Exit(queue);
        }

        public void Stop()
        {
            running = false;

            tokenSource.Cancel();
        }

        public void Add(QueedJob comm)
        {
            queue.Add(comm);
        }


        private void Run()
        {
            try
            {
                while (running)
                {
                    var job = queue.Take(tokenSource.Token);

                    try
                    {
                        job();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //
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