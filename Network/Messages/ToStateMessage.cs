/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.Messages
{
    #region Usings

    using global::System;
    using Modularity.Messages;

    #endregion

    [Serializable]
    public abstract class ToStateMessage : UserMessage
    {
    }
}