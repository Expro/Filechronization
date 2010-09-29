// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System.Collections.Generic;
    using Filechronization.Modularity.Messages;

    #endregion

    #region Usings

    #endregion

    public class NewFileSignal : Message
    {
        private readonly FsFile<RelPath> descriptor;
        private readonly string fileName;

        private readonly IList<PieceHash> hashes;


        public NewFileSignal(string fileName, FsFile<RelPath> descriptor, IList<PieceHash> hashes)
        {
            this.fileName = fileName;
            this.descriptor = descriptor;

            this.hashes = hashes;
        }

        public string FileName
        {
            get { return fileName; }
        }

        public FsFile<RelPath> Descriptor
        {
            get { return descriptor; }
        }


        public IList<PieceHash> Hashes
        {
            get { return hashes; }
        }
    }
}