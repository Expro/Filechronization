namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;

    #endregion

    [Serializable]
    public class PieceAvailable : PieceMessage
    {
        public PieceAvailable(RelPath fileName, int pieceIndex)
            : base(fileName, pieceIndex)
        {
        }
    }
}