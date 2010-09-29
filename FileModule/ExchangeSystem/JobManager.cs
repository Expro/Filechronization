// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Filechronization.UserManagement;
    using Messages;

    #endregion

    #region Usings

    #endregion

    public class JobManager
    {
        private readonly Dictionary<PieceInfo, ActivePiece> activePieces;


        private readonly NewFileModule fileModule;
        private readonly Dictionary<string, TransferJob> jobList;


        private readonly Random random;
        private NetworkContext _currContext;
        private DataManager _dataManager;

        public JobManager()
        {
            jobList = new Dictionary<string, TransferJob>();
            activePieces = new Dictionary<PieceInfo, ActivePiece>();

            //requested = new HashSet<BlockInfo>();

            random = new Random();
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
            TransferJob job = SelectFile();
            ActivePiece newPiece = job.GetNonActivePiece();


            activePieces.Add(newPiece.PieceInfo, newPiece);
        }

        private void PieceComplete(ActivePiece piece)
        {
            PieceInfo pieceInfo = piece.PieceInfo;
            TransferJob job = jobList[pieceInfo.RelFilePath];
            if (CheckPieceData(job, pieceInfo))
            {
                //TODO:PieceAvailable message = new PieceAvailable(job.File.Path, pieceInfo.Index);

                job.PieceComplete(pieceInfo);
                if (job.Complete)
                {
                    JobComplete(job);
                }
            }
            else
            {
                piece.Reset();
            }
        }

        private bool CheckPieceData(TransferJob job, PieceInfo piece)
        {
            string path = fileModule._context.Path.ToFull(piece.RelFilePath);
            // TODO: to jest zle na razie (stala wielkosc)
            byte[] data = _dataManager.ReadPiece(path, ExchUtils.GetInFilePosition(piece),
                                                 ExchUtils.StandardPieceSize);
            return job.CheckData(piece, data, ExchUtils.StandardPieceSize);
        }

        private void JobComplete(TransferJob job)
        {
            jobList.Remove(job.File.Path);
        }

        public void RequestBlock()
        {
            if (activePieces.Count < ExchUtils.MaxActivePieces)
            {
                ActivePiece newPiece = SelectFile().GetNonActivePiece();

                activePieces.Add(newPiece.PieceInfo, newPiece);

                //  var pair = file.GetRareRandom();
            }

            ActivePiece piece = SelectPiece();

            //todo wyjatki
            TransferJob job = jobList[piece.PieceInfo.RelFilePath];
            UserID user = job.SelectUser(piece.PieceInfo.Index);

            new BlockRequest(piece.RequestNextBlock());


            //requested.Add(new PieceMessage(file.name, pair.PieceIndex));
        }


        public void ReceiveBlock(BlockInfo block, byte[] data)
        {
            ActivePiece activePiece = activePieces[block.Piece];
            if (activePiece.WasRequested(block))
            {
                _dataManager.WriteBlock(_currContext.ToLocal(block), data);
                activePiece.ReceivedBlock(block);

                if (activePiece.Complete)
                {
                    PieceComplete(activePiece);
                }
            }
        }

        public void AddJob(NewFileBroadcast message)
        {
            jobList.Add(message.FileName, new TransferJob(message.Descriptor, message.Hashes));
        }

        public void PieceAvailable(PieceAvailable message)
        {
            TransferJob fileJob = jobList[message.FileName];

            User user = (User) message.UserSender;

            fileJob.UserHas(UserID.Of(user), message.PieceIndex);
        }

        public bool CheckValidRequest(BlockRequest message)
        {
            throw new NotImplementedException();
        }
    }
}