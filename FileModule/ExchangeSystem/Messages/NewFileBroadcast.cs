namespace FileModule.ExchangeSystem.Messages
{
    using System;
    using System.Collections.Generic;
    using Filechronization.Modularity.Messages;

    public class NewFileBroadcast : Message
    {

        private readonly FileDescriptor descriptor;
        private readonly string fileName;

        private readonly IList<PieceHash> hashes;
        private readonly string seedUser;

        public NewFileBroadcast(string fileName, FileDescriptor descriptor, string seedUser, IList<PieceHash> hashes)
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

        public FileDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public string SeedUser
        {
            get { return seedUser; }
        }

        public IList<PieceHash>  Hashes
        {
            get { return hashes; }
        }

    }
}