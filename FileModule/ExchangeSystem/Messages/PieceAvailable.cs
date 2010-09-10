namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;

    #endregion

    [Serializable]
    public class PieceAvailable : PieceMessage
    {
        public PieceAvailable(string fileName, int pieceIndex)
            : base(fileName, pieceIndex)
        {
        }
    }
}