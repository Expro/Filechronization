/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.Tasks.ArbiterInfo.Messages
{
    #region Usings

    using global::System;
    using Modularity.Messages;

    #endregion

    [Serializable]
    public class ReqArbiterInfo : Message
    {
        // public IPAddress yourAddress { get; private set; }
    }
}