namespace FileModule.ExchangeSystem.Messages
{
    #region Usings

    using System.Collections.Generic;
    using Filechronization.Modularity.Messages;

    #endregion

    public class NewFilePublish : Message
    {
        private readonly FileDescriptor descriptor;
        private readonly string fileName;

        private readonly IList<PieceHash> hashes;
     

        public NewFilePublish(string fileName, FileDescriptor descriptor, IList<PieceHash> hashes)
        {
            this.fileName = fileName;
            this.descriptor = descriptor;
    
            this.hashes = hashes;
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        public FileDescriptor Descriptor
        {
            get
            {
                return descriptor;
            }
        }

    
        public IList<PieceHash> Hashes
        {
            get
            {
                return hashes;
            }
        }
    }
}