// Author: Piotr Trzpil

namespace FileModule.Tasks
{
    #region Usings

    using System;

    #endregion

    #region Usings

    #endregion

    public class FileCreatedFirstSideTask : FileModuleTask
    {
        public readonly SingleFileMessage Message;


        public FileCreatedFirstSideTask(NewFileModule fileModule, SingleFileMessage message) : base(fileModule, false)
        {
            Message = message;
        }

        public override bool CheckCondition()
        {
            throw new NotImplementedException();
        }
    }
}