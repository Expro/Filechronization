// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.System.Connections
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using global::System.Net;
    using Messages;

    #endregion

    public delegate void ObjectReceivedEventHandler(Peer sender, NetworkSend netMessage);

    public delegate void AfterConnectExec();

    /// <summary>
    ///   Klasa opisujaca polaczenie - moze to byc polaczenie z innym komputerem lub loopback
    /// </summary>
    public abstract class Peer
    {
        public abstract IPEndPoint Endpoint { get; }

        /// <summary>
        ///   Oznacza ze polaczenie jest aktywne
        /// </summary>
        public abstract bool Connected { get; }

        /// <summary>
        ///   Oznacza, ze polaczenie nie zostanie przerwane przez timer
        /// </summary>
        public abstract bool Persistent { get; set; }


        /// <summary>
        ///   Wyslanie wiadomosci przez polaczenie
        /// </summary>
        /// <param name = "message">Wiadomosc do wyslania</param>
        public abstract void Send(Message message);
    }
}