// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem
{
    public struct BlockInfo
    {
        private readonly short inPieceOffset;
        private readonly PieceInfo pieceInfo;


        public BlockInfo(RelPath filePath, int pieceIndex, short inPieceOffset)
        {
            pieceInfo = new PieceInfo(filePath, pieceIndex);
            this.inPieceOffset = inPieceOffset;
        }

        public BlockInfo(PieceInfo piece, short inPieceOffset)
        {
            pieceInfo = piece;
            this.inPieceOffset = inPieceOffset;
        }

        public RelPath RelFilePath
        {
            get { return pieceInfo.RelFilePath; }
        }

        public int PieceIndex
        {
            get { return pieceInfo.Index; }
        }

        public short InPieceOffset
        {
            get { return inPieceOffset; }
        }

        public PieceInfo Piece
        {
            get { return pieceInfo; }
        }

        public bool Equals(BlockInfo other)
        {
            return Equals(other.pieceInfo, pieceInfo) && other.inPieceOffset == inPieceOffset;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (BlockInfo)) return false;
            return Equals((BlockInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (pieceInfo.GetHashCode()*397) ^ inPieceOffset.GetHashCode();
            }
        }
    }
}