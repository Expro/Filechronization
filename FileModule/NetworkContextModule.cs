// Author: Piotr Trzpil
namespace FileModule
{
    public abstract class NetworkContextModule
    {
        private readonly NetworkContext _network;

        protected NetworkContextModule(NetworkContext network)
        {
            _network = network;
        }

        protected NetworkContext Network
        {
            get { return _network; }
        }

        protected NewFileModule FileModule
        {
            get { return _network.FileModule; }
        }

        protected MainStoragePath MainPath
        {
            get { return _network.MainPath; }
        }

        protected MainFileIndex FileIndex
        {
            get { return _network.MainFileIndex; }
        }

        protected ChangeWatcher ChangeWatcher
        {
            get { return _network.ChangeWatcher; }
        }
    }
}