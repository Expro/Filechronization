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

        protected MainStoragePath WorkPath
        {
            get { return _network.Path; }
        }

        protected TableOverseer IndexedTable
        {
            get { return _network.TableOverseer; }
        }

        protected FileWatcher FileWatcher
        {
            get { return _network.FileWatcher; }
        }
    }
}