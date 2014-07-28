// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;

    #endregion

    /// <summary>
    ///   Wlasciwosci liczbowe protokolu wymiany plikow
    /// </summary>
    public static class ExchUtils
    {
        /// <summary>
        ///   Rozmiar bloku: 16 kB = 2^14
        /// </summary>
        public const int StandardBlockSize = 16384;

        public const int StandardBlockSizeLog2 = 14;

        /// <summary>
        ///   Rozmiar czesci: 512 kB = 2^19
        /// </summary>
        public const int StandardPieceSize = 524288;

        public const int StandardPieceSizeLog2 = 19;

        public const int BlocksInPiece = StandardPieceSize/StandardBlockSize;
        public const int MaxActivePieces = 10;

        public static long GetInFilePosition(BlockInfo block)
        {
            return ((long) block.PieceIndex << StandardPieceSize) + block.InPieceOffset;
        }

        public static long GetInFilePosition(PieceInfo piece)
        {
            return (long) piece.Index << StandardPieceSize;
        }

        public static int BlockCount(int sizeInBytes)
        {
            if (sizeInBytes < 0)
                throw new ArgumentException("Size is negative: " + sizeInBytes);


            return (int) Math.Ceiling((double) sizeInBytes/StandardBlockSize);
        }

        public static int PieceCount(long sizeInBytes)
        {
            if (sizeInBytes < 0)
                throw new ArgumentException("Size is negative: " + sizeInBytes);


            //return (int)Math.Ceiling((double)sizeInBytes / ExchUtils.StandardPieceSize);
            return (int) Math.Ceiling((double) sizeInBytes/StandardPieceSize);
        }

        public static int PieceSize(long fileSize, int pieceIndex)
        {
            if (fileSize < 0)
                throw new ArgumentException("File size is negative: " + fileSize);

            if (pieceIndex < 0)
                throw new ArgumentException("Piece index is negative: " + pieceIndex);

            int count = PieceCount(fileSize);

            if (pieceIndex >= count)
                throw new ArgumentException("Piece index too big: " + pieceIndex + " in file: " + fileSize);


            return
                (int) (pieceIndex == count - 1 ? fileSize - ((count - 1) << StandardPieceSizeLog2) : StandardPieceSize);
        }
    }
}