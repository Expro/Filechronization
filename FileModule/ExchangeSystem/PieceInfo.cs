// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;

    #endregion

    #region Usings

    #endregion

    [Serializable]
    public struct PieceInfo
    {
        private readonly int index;
        private readonly RelPath relFilePath;


        public PieceInfo(RelPath relFilePath, int index)
        {
            this.relFilePath = relFilePath;
            this.index = index;
        }

        public RelPath RelFilePath
        {
            get { return relFilePath; }
        }

        /// <summary>
        ///   Numer czesci w pliku
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        public bool Equals(PieceInfo other)
        {
            return Equals(other.relFilePath, relFilePath) && other.index == index;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (PieceInfo)) return false;
            return Equals((PieceInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (relFilePath.GetHashCode()*397) ^ index;
            }
        }
    }
}