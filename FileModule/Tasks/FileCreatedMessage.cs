// Author: Piotr Trzpil
namespace FileModule.Tasks
{
    #region Usings

    using ExchangeSystem;

    #endregion

    #region Usings

    #endregion

    public class FileCreatedMessage
    {
        private FsFile<RelPath> descriptor;

        private PieceHash[] hashes;
        private string userLogin;
    }
}