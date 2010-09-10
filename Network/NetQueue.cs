/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network
{
    #region Usings

    using global::System;
    using global::System.Threading;
    using Modularity;
    using Modularity.Messages;

    #endregion

//    public delegate void ActionTask();
//
//    public class NetQueue
//    {
//        private readonly BlockingCollection<ActionTask> _queue;
//        private readonly Thread _thread;
//
//        private bool _running;
//
//        public NetQueue()
//        {
//            _queue = new BlockingCollection<ActionTask>();
//            _thread = new Thread(Run);
//            _running = false;
//        }
//
//        public void Start()
//        {
//            Monitor.Enter(_queue);
//            if (!_running)
//            {
//                _running = true;
//                _thread.Start();
//            }
//            Monitor.Exit(_queue);
//        }
//
//        public void Stop()
//        {
//            _running = false;
//        }
//
//        public void Add(ActionTask comm)
//        {
//            _queue.Enqueue(comm);
//        }
//
//        public void Clear()
//        {
//            _queue.Clear();
//        }
//
//        private void Run()
//        {
//            while (_running)
//            {
//                var task = _queue.Dequeue();
//                task();
//            }
//        }
//    }

    /// <summary>
    /// Klasa opakowujaca funkcjonalnosc Service zwiazana z wykonywaniem kodu prze odpowiedni Procesor
    /// </summary>
    public class NetQueue
    {
        private readonly Service _service;
        private readonly Processor _processor;
        private readonly Thread _thread;
        private bool _running;

        private static int idd;
        private int id;

        public NetQueue(Service service, Processor processor)
        {
            _service = service;
            _processor = processor;
            _thread = new Thread(run);
            _running = false;


            id = idd++;
        }

        public void Start()
        {
            if (!_running)
            {
                _running = true;
                _thread.Start();
            }
        }

        public void Add(ServiceTask comm)
        {
            _service.EnqueueTask(comm, _processor);
        }

        public void Add(Message comm)
        {
            _service.EnqueueMessage(comm);
        }

        public void Register(Type msg, MessageEvent ev)
        {
            _service.Register(msg, ev);
        }

        public void Unregister(Type msg, MessageEvent ev)
        {
            _service.Unregister(msg, ev);
        }

        public void Stop()
        {
            _running = false;
            _service.DestroyProcessor(_processor);
        }

        private void run()
        {
            while (_running)
            {
                //Console.WriteLine("processor " + id);
                _processor();
            }
        }
    }
}