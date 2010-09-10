namespace FileModule.Tasks
{
    #region Usings

    using ExchangeSystem;

    #endregion

    public class FileCreatedMessage
    {
        private FileDescriptor descriptor;

        private PieceHash[] hashes;
        private string userLogin;
    }
}