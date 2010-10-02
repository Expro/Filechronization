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
        private readonly RelPath fileName;

        private readonly IList<PieceHash> hashes;
        private readonly string seedUser;

        public NewFileBroadcast(RelPath fileName, FsFile<RelPath> descriptor, string seedUser, IList<PieceHash> hashes)
        {
            this.fileName = fileName;
            this.descriptor = descriptor;
            this.seedUser = seedUser;
            this.hashes = hashes;
        }

        public RelPath FileName
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