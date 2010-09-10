namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;

    #endregion

    public class ActivePiece
    {
        


        private readonly PieceID pieceId;
        private readonly int pieceByteSize;
        private readonly int blocksCount;
        private readonly List<BlockStatus> blocks;


        private short currentPosition;
        private int receivedCount;

//        private bool complete;

        

        public ActivePiece(string fileName, int pieceIndex, int pieceByteSize)
            :this(new PieceID(fileName,pieceIndex),pieceByteSize )
        {
            
            
        }
        public ActivePiece(PieceID piece, int pieceByteSize)
        {
            this.pieceId = piece;
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


        public BlockID RequestNextBlock()
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
            
            return new BlockID(pieceId, ret);
        }

        public bool WasRequested(BlockID blockId)
        {

            return blocks[blockId.InPieceOffset] == BlockStatus.Requested;
        }
        public void ReceivedBlock(BlockID blockId)
        {
            blocks[blockId.InPieceOffset] = BlockStatus.Received;

            receivedCount++;

//            if(receivedCount==blocksCount)
//            {
//                complete = true;
//
//        
//            }
            

        }


        public PieceID PieceId
        {
            get
            {
                return pieceId;
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