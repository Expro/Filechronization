namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;
    using Filechronization.Modularity.Messages;

    #endregion

    [Serializable]
    public class BlockMessage : UserMessage
    {
        private readonly BlockID blockId;

        public BlockMessage(string fileName, int pieceIndex, short inPieceOffset)
            
        {
            blockId = new BlockID(fileName, pieceIndex, inPieceOffset);
        }

        public BlockMessage(BlockID blockId)
        {
            this.blockId = blockId;
        }

        public BlockID BlockId
        {
            get { return blockId; }
        }
    }
}