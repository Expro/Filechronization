namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;
    using Filechronization.Modularity.Messages;

    #endregion

    [Serializable]
    public class BlockMessage : UserMessage
    {
        private readonly BlockInfo blockInfo;

        public BlockMessage(RelPath fileName, int pieceIndex, short inPieceOffset)
            
        {
            blockInfo = new BlockInfo(fileName, pieceIndex, inPieceOffset);
        }

        public BlockMessage(BlockInfo blockInfo)
        {
            this.blockInfo = blockInfo;
        }

        public BlockInfo BlockInfo
        {
            get { return blockInfo; }
        }
    }
}