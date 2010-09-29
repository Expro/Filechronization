// Author: Piotr Trzpil
namespace Network.Tasks
{
    #region Usings

    using System.MainParts;
    using Filechronization.Modularity.Messages;
    using Filechronization.Tasks;
    using global::System.Net;
    using Messages;

    #endregion

    #region Usings

    #endregion

    public abstract class NetworkTask : Task
    {
        protected readonly NetworkModule _netModule;

        protected NetworkTask(NetworkModule netModule, bool isUnique) : base(isUnique)
        {
            _netModule = netModule;
        }

  
    }
}