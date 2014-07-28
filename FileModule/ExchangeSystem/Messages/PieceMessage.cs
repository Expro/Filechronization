// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;
    using System.IO;
    using Filechronization.Modularity.Messages;

    #endregion

    #region Usings

    #endregion

    [Serializable]
    public class PieceMessage : UserMessage
    {
        private readonly PieceInfo pieceInfo;

        public PieceMessage(RelPath fileName, int pieceIndex)
        {
            pieceInfo = new PieceInfo(fileName, pieceIndex);
            
        }

        public RelPath FileName
        {
            get { return pieceInfo.RelFilePath; }
        }

        public int PieceIndex
        {
            get { return pieceInfo.Index; }
        }

        public PieceInfo Piece
        {
            get { return pieceInfo; }
        }
    }
}