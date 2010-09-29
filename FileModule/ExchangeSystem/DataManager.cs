// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;

    #endregion

    public class DataManager
    {
        private readonly byte[] dataBuffer;

        public DataManager()
        {
            dataBuffer = new byte[ExchUtils.StandardPieceSize];
        }


        public byte[] ReadPiece(string path, long position, int length)
        {
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                try
                {
                    int count = stream.Read(dataBuffer, 0, length);
                    return dataBuffer;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "path"></param>
        /// <param name = "position"></param>
        /// <param name = "length"></param>
        /// <exception cref = "System.IO.IOException"></exception>
        public byte[] ReadBlock(string path, long position, int length)
        {
            byte[] data = new byte[length];
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                try
                {
                    stream.Position = position;
                    int read = stream.Read(data, 0, ExchUtils.StandardBlockSize);
                    if (read == 0)
                    {
                        throw new Exception("read == 0");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return data;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "data"></param>
        /// <param name = "path"></param>
        /// <param name = "position"></param>
        /// <exception cref = "System.IO.IOException"></exception>
        public void WriteBlock(byte[] data, string path, long position)
        {
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                try
                {
                    stream.Position = position;
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public IList<PieceHash> HashAll(string path)
        {
            List<PieceHash> list = new List<PieceHash>();
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (true)
                {
                    byte[] bytes = reader.ReadBytes(ExchUtils.StandardPieceSize);
                    //int count = reader.Read(dataBuffer, 0, ExchUtils.StandardPieceSize);
                    if (bytes.Length == 0)
                    {
                        break;
                    }

                    list.Add(new PieceHash(dataBuffer, bytes.Length));
                }
            }

            return list;
        }

        public void WriteBlock(LocalBlockInfo blockLocal, byte[] data)
        {
            using (FileStream stream = File.Open(blockLocal.AbsFilePath, FileMode.Open))
            {
                try
                {
                    stream.Position = blockLocal.Position;
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public byte[] ReadBlock(LocalBlockInfo blockLocal)
        {
            byte[] data = new byte[blockLocal.Size];
            using (FileStream stream = File.Open(blockLocal.AbsFilePath, FileMode.Open))
            {
                try
                {
                    stream.Position = blockLocal.Position;
                    int read = stream.Read(data, 0, ExchUtils.StandardBlockSize);
                    if (read == 0)
                    {
                        throw new Exception("read == 0");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return data;
            }
        }
    }
}