// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System;

    #endregion

    #region Usings

    #endregion

    [Serializable]
    public class BlockTransfer : BlockMessage
    {
        private readonly byte[] data;

        public BlockTransfer(RelPath fileName, int pieceIndex, byte inPieceOffset, byte[] data)
            : base(fileName, pieceIndex, inPieceOffset)
        {
            this.data = data;
        }

        public BlockTransfer(BlockRequest message, byte[] bytes)
            : base(message.BlockInfo.RelFilePath, message.BlockInfo.PieceIndex, message.BlockInfo.InPieceOffset)
        {
            data = bytes;
        }

        public byte[] Data
        {
            get { return data; }
        }
    }
}