// Author: Piotr Trzpil

namespace FileModule.Tasks
{
    #region Usings

    using System;

    #endregion

    #region Usings

    #endregion

    public class FileCreatedFirstSideSymTask : FileModuleSymTask
    {
        public readonly SingleFileMessage Message;


        public FileCreatedFirstSideSymTask(NewFileModule fileModule, SingleFileMessage message) : base(fileModule, false)
        {
            Message = message;
        }

        public override bool CheckCondition()
        {
            throw new NotImplementedException();
        }
    }
}