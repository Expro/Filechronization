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


        private readonly Dictionary<PieceID, ActivePiece> activePieces;

        
        private readonly Dictionary<string, TransferJob> jobList;
        

        private readonly Random random;
        private readonly Thread senderThread;
        private readonly byte[] dataBuffer;

        //private readonly HashSet<BlockID> requested;
        private bool running;


        public ExchangeSystem(NewFileModule fileModule)
        {
            this.fileModule = fileModule;
            jobList = new Dictionary<string, TransferJob>();
            activePieces = new Dictionary<PieceID, ActivePiece>();
            dataBuffer = new byte[ExchUtils.StandardPieceSize];
            //requested = new HashSet<BlockID>();

            random = new Random();
            senderThread = new Thread(RequestData);
            senderThread.Start();
        }

        private TransferJob SelectFile()
        {
            int index = random.Next(jobList.Count);
            return jobList.ElementAt(index).Value;
        }

        private ActivePiece SelectPiece()
        {
            int index = random.Next(activePieces.Count);
            return activePieces.ElementAt(index).Value;
        }
        private void CreateNewActivePiece()
        {
            var job = SelectFile();
            var pair = job.GetNonActivePiece();

            var newPiece = new ActivePiece(pair.Item1, pair.Item2);
     
            
            activePieces.Add(newPiece.PieceId, newPiece);
        }

        private void PieceComplete(ActivePiece piece)
        {
            PieceID pieceId = piece.PieceId;
            var job = jobList[pieceId.RelFilePath];
            if (CheckPieceData(job, pieceId))
            {
                var message = new PieceAvailable(job.File.RelativePath, pieceId.Index);

                job.PieceComplete(pieceId);
                if(job.Complete)
                {
                    FileComplete(job.File);
                }

            }
            else
            {
                piece.Reset();
            }
        }

        private void FileComplete(FileDescriptor file)
        {
            
        }

        private bool CheckPieceData(TransferJob job , PieceID piece)
        {
            var path = fileModule.Network.MainPath.CreateFullPath(piece.RelFilePath);
            using (var stream = File.Open(path, FileMode.Open))
            {
                try
                {
                    int count = stream.Read(dataBuffer, 0, ExchUtils.StandardPieceSize);
                    return job.CheckData(piece, dataBuffer, count);

                }
                catch (Exception)
                {

                    return false;
                }
                
                    
            }



        }

        private void RequestData()
        {
            while (running)
            {
                if (activePieces.Count < ExchUtils.MaxActivePieces)
                {
                    var file = SelectFile();
                  //  var pair = file.GetRareRandom();
                }

                var piece = SelectPiece();

                //todo wyjatki
                var job = jobList[piece.PieceId.RelFilePath];
                var user = job.SelectUser(piece.PieceId.Index);
                
                new BlockRequest(piece.RequestNextBlock());


                //requested.Add(new PieceMessage(file.name, pair.PieceIndex));
            }
        }

        public IList<PieceHash> HashAll(string path)
        {
            var list = new List<PieceHash>();
            using (var stream = File.Open(path, FileMode.Open))
            {
                while (true)
                {
                    int count = stream.Read(dataBuffer, 0, ExchUtils.StandardPieceSize);
                    if (count == 0)
                    {
                        break;
                    }

                    list.Add(new PieceHash(dataBuffer, count));
                }
            }
            return list;
        }

        /// <summary>
        /// Wywolywana gdy lokalnie zostanie wykryty nowy plik
        /// </summary>
        /// <param name="descr"></param>
        /// <param name="path"></param>
       
        public void NewFileVersion(FileDescriptor descr, string path)
        {
            try
            {
                var hashes = HashAll(path);

                var mess = new NewFilePublish(descr.RelativePath, descr, hashes);
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
        public void HandleNewFilePublish(Message mess)
        {
            var message = (NewFilePublish) mess;
            if(SecurityCheck(message))
            {

             //   var broad = new NewFileBroadcast();

            }

          
        }

        private bool SecurityCheck(NewFilePublish message)
        {
            if (message.Hashes.Count != message.Descriptor.Size)
            {
                Console.WriteLine("Message with wrong hash count");
                return false;
            }

            return true;

        }


        public void HandleNewFileBroadcast(Message mess)
        {
            var message = (NewFileBroadcast)mess;

            jobList.Add(message.FileName, new TransferJob(message.Descriptor, message.Hashes));
        }

        private FileDescriptor GetFile(string fileName)
        {
            var path = fileModule.Network.MainPath.CreateFullPath(fileName);
            return fileModule.tableOverseer.GetFile(path);
        }

        public void HandleBlockRequest(Message mess)
        {
            var message = (BlockRequest) mess;
      
            var descriptor = GetFile(message.BlockId.FileName);
            long position = ExchUtils.GetInFilePosition(message.BlockId);
            int currentBlockSize = (int) (descriptor.Size - position);
            if (currentBlockSize > ExchUtils.StandardBlockSize)
            {
                throw new Exception("blockSize == " + currentBlockSize);
            }

            byte[] data = new byte[currentBlockSize];
            var path = fileModule.Network.MainPath.CreateFullPath(descriptor.RelativePath);
            using (var stream = File.Open(path, FileMode.Open))
            {
                
                try
                {
                    stream.Position = position;
                    int read = stream.Read(data, 0, ExchUtils.StandardBlockSize);
                    if (read == 0)
                    {
                        throw new Exception("read == 0");
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e);
                }
            }

            //FIXME: Wykomentowane w celu kompilacji
            //var outMessage = new BlockTransfer(message, data);
        }

        public void HandleBlockTransfer(Message mess)
        {
            var message = (BlockTransfer) mess;


            var path = fileModule.Network.MainPath.CreateFullPath(message.BlockId.FileName);
            long position = ExchUtils.GetInFilePosition(message.BlockId);

            var activePiece = activePieces[message.BlockId.Piece];
            if (activePiece.WasRequested(message.BlockId))
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    try
                    {
                        stream.Position = position;
                        stream.Write(message.Data, 0, ExchUtils.StandardBlockSize);

                        activePiece.ReceivedBlock(message.BlockId);

                    }
                    catch (Exception e)
                    {
                        
                        Console.WriteLine(e);
                    }
                    
                    
                }

                if(activePiece.Complete)
                {
                    PieceComplete(activePiece);
                }


            }
            
        }

        public void HandlePieceAvailable(Message mess)
        {
            var message = (PieceAvailable)mess;

            var fileJob = jobList[message.FileName];

            var user = (User) message.UserSender;

            fileJob.UserHas(user.login, message.PieceIndex);
            
        }
        

    }
}