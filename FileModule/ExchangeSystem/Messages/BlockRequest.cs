namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;

    #endregion

    [Serializable]
    public class BlockRequest : BlockMessage
    {
        public BlockRequest(string fileName, int pieceIndex, short inPieceOffset)
            : base(fileName, pieceIndex, inPieceOffset)
        {
        }
        public BlockRequest(BlockID blockId) :base(blockId)
        {
            
        }
    }
}