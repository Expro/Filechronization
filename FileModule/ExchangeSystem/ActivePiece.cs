// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    #region Usings

    #endregion

    public class ActivePiece
    {
        private readonly List<BlockStatus> blocks;
        private readonly int blocksCount;
        private readonly int pieceByteSize;
        private readonly PieceInfo pieceInfo;


        private short currentPosition;
        private int receivedCount;

//        private bool complete;


        public ActivePiece(RelPath fileName, int pieceIndex, int pieceByteSize)
            : this(new PieceInfo(fileName, pieceIndex), pieceByteSize)
        {
        }

        public ActivePiece(PieceInfo piece, int pieceByteSize)
        {
            pieceInfo = piece;
            this.pieceByteSize = pieceByteSize;
            blocksCount = ExchUtils.BlockCount(pieceByteSize);


            blocks = new List<BlockStatus>(blocksCount);

            Reset();
        }

        public PieceInfo PieceInfo
        {
            get { return pieceInfo; }
        }

        public bool Complete
        {
            get { return receivedCount >= blocksCount; }
        }

        public void Reset()
        {
//            complete = false;
            currentPosition = 0;
            receivedCount = 0;
            for (int i = 0; i < blocksCount; i++)
            {
                blocks[i] = BlockStatus.Available;
            }
        }


        public BlockInfo RequestNextBlock()
        {
            short ret = currentPosition;
            blocks[currentPosition] = BlockStatus.Requested;

            if (currentPosition + ExchUtils.StandardBlockSize >= pieceByteSize)
            {
                currentPosition = -1;
            }
            else
            {
                currentPosition += ExchUtils.StandardBlockSize;
            }

            return new BlockInfo(pieceInfo, ret);
        }

        public bool WasRequested(BlockInfo blockInfo)
        {
            return blocks[blockInfo.InPieceOffset] == BlockStatus.Requested;
        }

        public void ReceivedBlock(BlockInfo blockInfo)
        {
            blocks[blockInfo.InPieceOffset] = BlockStatus.Received;

            receivedCount++;

//            if(receivedCount==blocksCount)
//            {
//                complete = true;
//
//        
//            }
        }

        #region Nested type: BlockStatus

        private enum BlockStatus
        {
            Available,
            Requested,
            Received,
        }

        #endregion
    }
}