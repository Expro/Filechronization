/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.Tasks.ArbiterInfo.Messages
{
    #region Usings

    using global::System;
    using global::System.Net;
    using Modularity.Messages;

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