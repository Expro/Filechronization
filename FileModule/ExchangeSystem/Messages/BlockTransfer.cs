namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;

    #endregion

    [Serializable]
    public class BlockTransfer : BlockMessage
    {
        private readonly byte[] data;

        public BlockTransfer(string fileName, int pieceIndex, byte inPieceOffset, byte[] data)
            : base(fileName, pieceIndex, inPieceOffset)
        {
            this.data = data;
        }

        public BlockTransfer(BlockRequest message, byte[] bytes)
            : base(message.BlockId.FileName, message.BlockId.PieceIndex, message.BlockId.InPieceOffset)
        {
            data = bytes;
        }

        public byte[] Data
        {
            get { return data; }
        }
    }
}