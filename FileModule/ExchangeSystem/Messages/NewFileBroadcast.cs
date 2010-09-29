// Author: Piotr Trzpil
namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System.Collections.Generic;
    using Filechronization.Modularity.Messages;

    #endregion

    public class NewFileBroadcast : Message
    {
        private readonly FsFile<RelPath> descriptor;
        private readonly string fileName;

        private readonly IList<PieceHash> hashes;
        private readonly string seedUser;

        public NewFileBroadcast(string fileName, FsFile<RelPath> descriptor, string seedUser, IList<PieceHash> hashes)
        {
            this.fileName = fileName;
            this.descriptor = descriptor;
            this.seedUser = seedUser;
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

        public string SeedUser
        {
            get { return seedUser; }
        }

        public IList<PieceHash> Hashes
        {
            get { return hashes; }
        }
    }
}