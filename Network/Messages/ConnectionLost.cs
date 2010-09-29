// Author: Piotr Trzpil
namespace Network.Messages
{
    #region Usings

    using global::System;

    #endregion

    #region Usings

    #endregion

    // obiekt tworzony, "wysylany" i przetwarzany lokalnie gdy nastapi utrata polaczenia
    [Serializable]
    public class ConnectionLost : ToStateMessage
    {
    }
}