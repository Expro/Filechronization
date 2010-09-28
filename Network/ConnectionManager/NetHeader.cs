namespace ConsoleApplication1
{
    using System;
    using System.IO;
    using System.Text;
    /// <summary>
    /// Header is combined from 8 ASCII letters 
    /// and 4 bytes indicating length of object.
    /// </summary>
    public class NetHeader
    {

        

        public const string Signature = "XCEAHEIM";
        public const int HeaderLength = 8 + 4;

        public static int ReadLengthFromHeader(MemoryStream stream, int position)
        {
            long tmpPos = stream.Position;
            stream.Position = position;
            byte[] tmp = new byte[Signature.Length];
            stream.Read(tmp, 0, tmp.Length);
            byte[] num = new byte[sizeof (Int32)];
            stream.Read(num, 0, num.Length);
            stream.Position = tmpPos;

            if (Encoding.ASCII.GetString(tmp).Equals(Signature))
            {
                return BitConverter.ToInt32(num, 0);
            }

            throw new BadHeaderException();
        }

        public static bool IsFullObjectInStream(MemoryStream stream, int position, out int len)
        {
            if (stream.Length - position >= HeaderLength)
            {
                int length;
                try
                {
                    length = ReadLengthFromHeader(stream, position);
                }
                catch (BadHeaderException )
                {
                    // prawdopodobnie powstanie wyjatek przy kolejnym 
                    // Deserialize, wiec ok.
                    len = 0;
                    return true;
                }

                if (stream.Length - position - HeaderLength >= length)
                {
                    len = length;
                    return true;
                }
            }
            len = -1;
            return false;
        }

        public static byte[] CreateHeader(long length)
        {
            byte[] s = Encoding.ASCII.GetBytes(NetHeader.Signature);
            byte[] len = BitConverter.GetBytes((int)length);
            byte[] header = new byte[s.Length + len.Length];
            Array.Copy(s, 0, header, 0, s.Length);
            Array.Copy(len, 0, header, s.Length, len.Length);
            return header;
        }
    }
}