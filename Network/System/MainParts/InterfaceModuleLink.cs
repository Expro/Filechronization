// Author: Piotr Trzpil
namespace Network.System.MainParts
{
    #region Usings

    using Filechronization.Modularity.Messages;
    using Filechronization.UserManagement;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Modul otrzymujacy wiadomosci od interfejsu
    /// </summary>
    public class InterfaceModuleLink
    {
        private readonly NetworkModule _netModule;

        public InterfaceModuleLink(NetworkModule netModule)
        {
            _netModule = netModule;


//            netModule.netQueue.Register(typeof (StartLogin), HandleLoginCommand);
        }

        /// <summary>
        ///   Z interfejsu otrzymano rozkaz logowania do sieci, sprawdzana jest obecnosci zadanego loginu na liscie
        ///   a nastepnie rozpoczynane jest laczenie do sieci
        /// </summary>
        /// <param name = "message">Wiadomosc z loginem i haslem</param>
        public void HandleLoginCommand(Message message)
        {
//            var loginMessage = (StartLogin) message;
//
//            if (string.IsNullOrEmpty(loginMessage.PasswordString))
//            {
//                SharedContext.service.EnqueueMessage(new ToInterfaceLoginResult(false, "Enter password"));
//                return;
//            }
//
//            User user = _netModule.UsersStructure[loginMessage.LoginString];
//
//            if (user == null)
//            {
//                SharedContext.service.EnqueueMessage(new ToInterfaceLoginResult(false, "User not found"));
//            }
//            else
//            {
//                user.password = new Password(loginMessage.PasswordString);
//                _netModule.CurrentUser = user;
//                _netModule.PeerCenter.StartConnectToNetwork();
//            }
        }
    }
}