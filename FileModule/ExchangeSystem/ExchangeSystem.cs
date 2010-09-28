namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Filechronization.Modularity.Messages;
    using Filechronization.UserManagement;
    using Messages;

    #endregion
    
    public class ExchangeSystem
    {
        
        private readonly NewFileModule fileModule;

        private JobManager _jobManager;
        private DataManager _dataManager;

        private NetworkContext _currContext;

        public ExchangeSystem(NewFileModule fileModule)
        {
            this.fileModule = fileModule;
            
//            senderThread = new Thread(RequestBlock);
//            senderThread.Start();
        }


        /// <summary>
        /// Wywolywana gdy lokalnie zostanie wykryty nowy plik
        /// </summary>
        /// <param name="descr"></param>
        /// <param name="path"></param>
        public void NewFileVersion(FsFile<RelPath> descr, string path)
        {
            try
            {
                var hashes = _dataManager.HashAll(path);

                var mess = new NewFileSignal(descr.Path, descr, hashes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void StopTransfer(string filePath)
        {
        }

        /// <summary>
        /// Wykonywana przez arbitra
        /// </summary>
        /// <param name="mess"></param>
        public void HandleNewFileSignal(Message mess)
        {
            var message = (NewFileSignal) mess;
            if(SecurityCheck(message))
            {

             //   var broad = new NewFileBroadcast();

            }
        }

        private bool SecurityCheck(NewFileSignal message)
        {
            try
            {
                int pieceCount = ExchUtils.PieceCount(message.Descriptor.Size);
                if (message.Hashes.Count != pieceCount)
                {
                    //Console.WriteLine("Message with wrong hash count");
                    return false;
                }
               
            }
            catch (Exception )
            {
                return false;
            }
            

            return true;

        }


        public void HandleNewFileBroadcast(Message mess)
        {
            var message = (NewFileBroadcast)mess;

            _jobManager.AddJob(message);

            
        }


        public void HandleBlockRequest(Message mess)
        {
            var message = (BlockRequest) mess;

            _jobManager.CheckValidRequest(message);
            byte[] data = _dataManager.ReadBlock(_currContext.ToLocal(message.BlockInfo));
            //FIXME: Wykomentowane w celu kompilacji
            //var outMessage = new BlockTransfer(message, data);
            

            
        }

        public void HandleBlockTransfer(Message mess)
        {
            var message = (BlockTransfer) mess;
            
            _jobManager.ReceiveBlock(message.BlockInfo, message.Data);


        }

        

        public void HandlePieceAvailable(Message mess)
        {
            var message = (PieceAvailable)mess;

            _jobManager.PieceAvailable(message);


            
            
        }
    }
}