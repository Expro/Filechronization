namespace FileModule.ExchangeSystem
{
    using System;
    using System.Collections.Generic;

    public struct BlockID
    {
        private readonly PieceID pieceId;
        private readonly short inPieceOffset;


        public BlockID(string fileName, int pieceIndex, short inPieceOffset)
        {
            this.pieceId = new PieceID(fileName, pieceIndex);
            this.inPieceOffset = inPieceOffset;
        }
        public BlockID(PieceID piece, short inPieceOffset)
        {
            this.pieceId = piece;
            this.inPieceOffset = inPieceOffset;
        }

        public string FileName
        {
            get
            {
                return pieceId.RelFilePath;
            }
        }

        public int PieceIndex
        {
            get
            {
                return pieceId.Index;
            }
        }
        public short InPieceOffset
        {
            get { return inPieceOffset; }
        }

        public PieceID Piece
        {
            get { return pieceId; }
        }

        public bool Equals(BlockID other)
        {
     
            return Equals(other.pieceId, pieceId) && other.inPieceOffset == inPieceOffset;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (BlockID)) return false;
            return Equals((BlockID) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (pieceId.GetHashCode()*397) ^ inPieceOffset.GetHashCode();
            }
        }
    }
}