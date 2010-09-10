namespace FileModule.Tasks
{
    #region Usings

    using System;
    using Filechronization.Modularity.Messages;

    #endregion

    public class FileCreatedSecondSideTask : FileModuleTask
    {
        public FileCreatedSecondSideTask(NewFileModule fileModule, bool isUnique) : base(fileModule, isUnique)
        {
            AddHandler(typeof (SingleFileMessage), HandleFileCreated, 0);
        }

        private int HandleFileCreated(Message message)
        {
            var fileCreated = (SingleFileMessage) message;

            var path = fileModule.Network.MainPath.CreateFullPath(fileCreated.RelativePath);


            return 0;
        }

        public override bool CheckCondition()
        {
            throw new NotImplementedException();
        }
    }
}