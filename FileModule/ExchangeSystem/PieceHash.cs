namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    public class PieceHash
    {
        private readonly string hashString;

        public PieceHash(byte[] dataPiece, int count)
        {
            MD5 hasher = MD5.Create();

            byte[] hash = hasher.ComputeHash(dataPiece, 0, count);

            var sBuilder = new StringBuilder();

            foreach (byte b in hash)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            hashString = sBuilder.ToString();
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            if (obj is PieceHash)
            {
                var other = (PieceHash) obj;
                var comparer = StringComparer.OrdinalIgnoreCase;
                return comparer.Compare(other.hashString, hashString) == 0;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return hashString.GetHashCode();
        }
    }
}