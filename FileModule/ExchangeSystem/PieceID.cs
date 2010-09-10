namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using Filechronization.Modularity.Messages;

    #endregion

    [Serializable]
    public struct PieceID 
    {
        private readonly string relFilePath;
        private readonly int index;


        public PieceID(string relFilePath, int index)
        {
            this.relFilePath = relFilePath;
            this.index = index;
        }

        public string RelFilePath
        {
            get { return relFilePath; }
        }
        /// <summary>
        /// Numer czesci w pliku
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        public bool Equals(PieceID other)
        {
   
            return Equals(other.relFilePath, relFilePath) && other.index == index;
        }

        public override bool Equals(object obj)
        {

            if (obj.GetType() != typeof (PieceID)) return false;
            return Equals((PieceID) obj);
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