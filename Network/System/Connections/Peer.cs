/*
 * Author: Piotr Trzpil
 */
 
#region Usings
using Filechronization.Tasks.Messages;
using System.Net;
using Filechronization.Network.System.MainParts;
using Filechronization.Network.Messages;
#endregion
    
namespace Filechronization.Network.System.Connections
{
    public delegate void ObjectReceivedEventHandler(Peer sender, NetworkSend netMessage);

    public delegate void AfterConnectExec();

    /// <summary>
    /// Klasa opisujaca polaczenie - moze to byc polaczenie z innym komputerem lub loopback
    /// </summary>
    public abstract class Peer
    {
        public event ObjectReceivedEventHandler ObjectReceived;


        public virtual IPEndPoint EndPointAddress
        {
            get { return new IPEndPoint(IPAddress.Loopback, 0); }
        }
        /// <summary>
        /// Oznacza ze polaczenie jest aktywne
        /// </summary>
        public abstract bool Connected { get; }
        /// <summary>
        /// Oznacza, ze polaczenie nie zostanie przerwane przez timer
        /// </summary>
        public abstract bool Persistent { get; set; }

        /// <summary>
        ///  Otrzymano obiekt
        /// </summary>
        /// <param name="obj">Otrzymany obiekt</param>

        protected virtual void OnObjectReceived(object obj)
        {
            if (ObjectReceived != null)
            {
                if (obj is NetworkSend)
                {
                    var netMessage = (NetworkSend) obj;
                    if (netMessage.message is TaskMessage)
                    {
                        var task = (TaskMessage) netMessage.message;
                    }
                    else
                    {
						}

                    ObjectReceived(this, netMessage);
                }
                else
                {
                    //NetworkModule.SendNotification("Received unknown object: " + obj, NotificationType.WARNING);
                }
            }
            else
            {
                //NetworkModule.SendNotification("ObjectReceived == null", NotificationType.ERROR);
            }
        }

        /// <summary>
        /// Rozlaczenie aktywnego polaczenia
        /// </summary>
        public abstract void Disconnect();
        /// <summary>
        /// Wyslanie wiadomosci przez polaczenie
        /// </summary>
        /// <param name="message">Wiadomosc do wyslania</param>
        public abstract void Send(NetworkSend message);

        /// <summary>
        /// Proba zresetowania licznika rozlaczenia
        /// </summary>
        public abstract void TryRestartTimer();

        /// <summary>
        /// Zlecenie wykonania metody po polaczeniu
        /// </summary>
        /// <param name="exec">Metoda do wykonania</param>
        public abstract void AfterConnect(AfterConnectExec exec);
    }
}