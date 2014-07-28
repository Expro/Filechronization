// Author: Piotr Trzpil
namespace Network.System.MainParts
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using Messages;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Modul otrzymujacy wiadomosci od modulu plikowego
    /// </summary>
    public class FileModulelLink
    {
        private readonly NetworkModule _netModule;

        public FileModulelLink(NetworkModule netModule)
        {
            _netModule = netModule;

            _netModule.netQueue.Register(typeof (Send), HandleExternalSend);
        }

        /// <summary>
        ///   Obsluga wiadomosci wyslania wiadomosci do konretnego uzytkownika
        /// </summary>
        /// <param name = "incMessage">Wiadomosc enkapsulujaca</param>
        private void HandleExternalSend(Message incMessage)
        {
            Send sendMessage = (Send) incMessage;

            _netModule.NetworkManager.SendTo(sendMessage.reciver, sendMessage.message);
        }

        /// <summary>
        ///   Obsluga wiadomosci wyslania wiadomosci do wszystkich
        /// </summary>
        /// <param name = "incMessage">Wiadomosc enkapsulujaca</param>
        private void HandleToAllMessage(Message incMessage)
        {
//            var toAllMessage = (ToAllMessage) incMessage;
//
//            _netModule.PeerCenter.SendToAll(toAllMessage.message);
        }

        /// <summary>
        ///   Obsluga wiadomosci wyslania wiadomosci do wszystkich z wyjatkiem jednego
        /// </summary>
        /// <param name = "incMessage">Wiadomosc enkapsulujaca</param>
        private void HandleToAllButMessage(Message incMessage)
        {
//            var toAllButMessage = (ToAllButMessage) incMessage;
//
//            _netModule.PeerCenter.SendToAllBut(toAllButMessage.message, toAllButMessage.ExcludedUser);
        }

        /// <summary>
        ///   Obsluga wiadomosci wyslania wiadomosci do arbitra
        /// </summary>
        /// <param name = "incMessage">Wiadomosc enkapsulujaca</param>
        private void HandleToArbiterMessage(Message incMessage)
        {
//            var arbiterMessage = (ArbiterMessage) incMessage;
//
//            
//            _netModule.PeerCenter.SendToArbiter(arbiterMessage.message);
        }
    }
}