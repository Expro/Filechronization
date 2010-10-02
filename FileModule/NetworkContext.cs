// Author: Piotr Trzpil


namespace FileModule
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using ExchangeSystem;

    #endregion

    public class NetworkContext
    {
        private readonly NewFileModule _fileModule;
        private readonly ChangeWatcher _changeWatcher;
        private readonly NetworkModel _netModel;
        private readonly MainStoragePath _mainPath;
        private readonly MainFileIndex _mainFileIndex;


        private ChangeMaster _dispatcher;

        public NetworkContext(NewFileModule module, NetworkModel model, MainStoragePath mainPath)
        {
            _fileModule = module;
            _mainPath = mainPath;
            _netModel = model;
            _mainFileIndex = new MainFileIndex(this);
            _changeWatcher = new ChangeWatcher(this);
            _dispatcher = new ChangeMaster(this);
        }

        public List<GroupModel> GroupList
        {
            get { return _netModel.GroupList; }
        }


        public NewFileModule FileModule
        {
            get { return _fileModule; }
        }

        public MainStoragePath MainPath
        {
            get { return _mainPath; }
        }

        public MainFileIndex MainFileIndex
        {
            get { return _mainFileIndex; }
        }

        public ChangeWatcher ChangeWatcher
        {
            get { return _changeWatcher; }
        }

        public FsObject<AbsPath> GetFile(RelPath fileName)
        {
            AbsPath path = fileName.AbsoluteIn(MainPath);
            return MainFileIndex.GetObjectAbs(path);
        }

        public LocalBlockInfo ToLocal(BlockInfo block)
        {
            FsFile<AbsPath> descriptor = (FsFile<AbsPath>) GetFile(block.RelFilePath);
            long position = ExchUtils.GetInFilePosition(block);
            int currentBlockSize = (int) (descriptor.Size - position);
            if (currentBlockSize > ExchUtils.StandardBlockSize)
            {
                throw new Exception("blockSize == " + currentBlockSize);
            }
            // var path = Path.ToFull(descriptor.Path);
            return new LocalBlockInfo(descriptor.Path, position, currentBlockSize);
        }
    }
}