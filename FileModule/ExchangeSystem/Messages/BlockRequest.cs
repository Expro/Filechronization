namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;

    #endregion

    [Serializable]
    public class BlockRequest : BlockMessage
    {
        public BlockRequest(RelPath fileName, int pieceIndex, short inPieceOffset)
            : base(fileName, pieceIndex, inPieceOffset)
        {
        }
        public BlockRequest(BlockInfo blockInfo) :base(blockInfo)
        {
            
        }
    }
}