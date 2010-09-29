// Author: Piotr Trzpil
namespace FileModule.Tasks
{
    #region Usings

    using Filechronization.Modularity.Messages;

    #endregion

    #region Usings

    #endregion

    public class SingleFileMessage : Message
    {
        public readonly SinglePathFileEvent EventType;
        public readonly FileOrFolder ObjectType;
        public readonly string RelativePath;


        public SingleFileMessage(string relativePath, FileOrFolder objectType, SinglePathFileEvent eventType)
        {
            RelativePath = relativePath;
            ObjectType = objectType;
            EventType = eventType;
        }
    }
}