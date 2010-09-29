// Author: Piotr Trzpil
namespace Network.Tasks.ArbiterInfo.Messages
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using global::System;
    using global::System.Net;

    #endregion

    #region Usings

    #endregion

    [Serializable]
    public class ArbiterInfo : Message
    {
        public readonly IPEndPoint Arbiter;

        public ArbiterInfo(IPEndPoint arbiter)

        {
            Arbiter = arbiter;
        }
    }
}