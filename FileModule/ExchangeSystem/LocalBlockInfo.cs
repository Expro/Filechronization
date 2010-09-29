// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem
{
    public class LocalBlockInfo
    {
        private readonly string _absFilePath;
        private readonly long _position;
        private readonly int _size;

        public LocalBlockInfo(string absFilePath, long position, int size)
        {
            _absFilePath = absFilePath;
            _position = position;
            _size = size;
        }

        public string AbsFilePath
        {
            get { return _absFilePath; }
        }

        public long Position
        {
            get { return _position; }
        }

        public int Size
        {
            get { return _size; }
        }
    }
}