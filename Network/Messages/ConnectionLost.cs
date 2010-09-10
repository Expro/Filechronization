/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.Messages
{
    #region Usings

    using global::System;

    #endregion

    // obiekt tworzony, "wysylany" i przetwarzany lokalnie gdy nastapi utrata polaczenia
    [Serializable]
    public class ConnectionLost : ToStateMessage
    {
    }
}