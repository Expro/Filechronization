// Author: Piotr Trzpil

namespace FileModule.Tasks
{
    #region Usings

    using System;
    using Filechronization.Modularity.Messages;

    #endregion

    #region Usings

    #endregion

    public class FileCreatedSecondSideSymTask : FileModuleSymTask
    {
        public FileCreatedSecondSideSymTask(NewFileModule fileModule, bool isUnique) : base(fileModule, isUnique)
        {
            AddHandler(typeof (SingleFileMessage), HandleFileCreated, 0);
        }

        private int HandleFileCreated(Message message)
        {
            SingleFileMessage fileCreated = (SingleFileMessage) message;

            //  var path = fileModule._context.Path.ToFull(fileCreated.RelativePath);


            return 0;
        }

        public override bool CheckCondition()
        {
            throw new NotImplementedException();
        }
    }
}