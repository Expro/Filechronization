namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;

    #endregion

    public class ActivePiece
    {
        


        private readonly PieceInfo pieceInfo;
        private readonly int pieceByteSize;
        private readonly int blocksCount;
        private readonly List<BlockStatus> blocks;


        private short currentPosition;
        private int receivedCount;

//        private bool complete;

        

        public ActivePiece(RelPath fileName, int pieceIndex, int pieceByteSize)
            :this(new PieceInfo(fileName,pieceIndex),pieceByteSize )
        {
            
            
        }
        public ActivePiece(PieceInfo piece, int pieceByteSize)
        {
            this.pieceInfo = piece;
            this.pieceByteSize = pieceByteSize;
            blocksCount = ExchUtils.BlockCount(pieceByteSize);



            blocks = new List<BlockStatus>(blocksCount);

            Reset();
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


        public PieceInfo PieceInfo
        {
            get
            {
                return pieceInfo;
            }
        }
        public bool Complete
        {
            get
            {
                return receivedCount >= blocksCount;
            }
        }

        private enum BlockStatus
        {
            Available,
            Requested,
            Received,
        }
    }
}