namespace FileModule
{
    using System;
    using System.Collections.Generic;
    using ExchangeSystem;

    public class NetworkContext
    {
        private NewFileModule _fileModule;
        private NetworkModel _netModel;
        private MainStoragePath _path;
        private TableOverseer _tableOverseer;
        private FileWatcher _fileWatcher;

        public List<GroupModel> GroupList
        {
            get { return _netModel.GroupList; }
        }


        private MessageDispatcher _dispatcher;


        public NewFileModule FileModule
        {
            get { return _fileModule; }
        }

        public MainStoragePath Path
        {
            get { return _path; }
        }

        public TableOverseer TableOverseer
        {
            get { return _tableOverseer; }
        }

        public FileWatcher FileWatcher
        {
            get { return _fileWatcher; }
        }

        public NetworkContext(NewFileModule module, NetworkModel model, MainStoragePath path)
        {
            _fileModule = module;
            _path = path;
            _netModel = model;
            _tableOverseer = new TableOverseer(this);
            _fileWatcher = new FileWatcher(this);
            _dispatcher= new MessageDispatcher(this);

        }
        public FsObject<AbsPath> GetFile(RelPath fileName)
        {
            var path = Path.ToFull(fileName);
            return TableOverseer.GetObject(path);
        }
        public LocalBlockInfo ToLocal(BlockInfo block)
        {
            FsFile<AbsPath> descriptor = (FsFile<AbsPath>)GetFile(block.RelFilePath);
            long position = ExchUtils.GetInFilePosition(block);
            int currentBlockSize = (int)(descriptor.Size - position);
            if (currentBlockSize > ExchUtils.StandardBlockSize)
            {
                throw new Exception("blockSize == " + currentBlockSize);
            }
           // var path = Path.ToFull(descriptor.Path);
            return new LocalBlockInfo(descriptor.Path, position, currentBlockSize);
        }
    }
}