namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;
    using Filechronization.Modularity.Messages;

    #endregion

    [Serializable]
    public class PieceMessage : UserMessage
    {
        private readonly PieceID pieceId;

        public PieceMessage(string fileName, int pieceIndex)
        {
            pieceId = new PieceID(fileName, pieceIndex);
        }

        public string FileName
        {
            get { return pieceId.RelFilePath; }
        }

        public int PieceIndex
        {
            get { return pieceId.Index; }
        }

        public PieceID Piece
        {
            get { return pieceId; }
        }
    }
}