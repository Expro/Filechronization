// Author: Piotr Trzpil
namespace Network.States
{
    #region Usings

    using System.Connections;
    using System.MainParts;
    using Filechronization.Modularity.Messages;
    using Filechronization.UserManagement;
    using global::System.Net;
    using Messages;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Oznacza, ze dany komputer nie jest arbitrem, ale jest polaczony do sieci
    /// </summary>
    public class StateNonArbiter : StateConnected
    {
        public StateNonArbiter(NetworkModule netModule, IPEndPoint arbiterAddress)
            : base(netModule, arbiterAddress)
        {
        }

        public override void BuildTaskAssociations()
        {
            base.BuildTaskAssociations();
            _messageAssociations.Add(typeof (ConnectionLost), HandleConnectionLost);
            _messageAssociations.Add(typeof (UserStateChanged), HandleUserStateChanged);
        }

        private void HandleConnectionLost(User sender, Message message)
        {
            if (Arbiter.Equals(sender))
            {
                //NetworkModule.SendNotification("Lost connection to arbiter", NotificationType.DIAGNOSTIC);
                //P.pr("ArbiterLeft!!!");
            }
        }

        private void HandleUserStateChanged(User sender, Message message)
        {
            UserStateChanged userStateChanged = (UserStateChanged) message;
            User user = _netModule.UsersStructure[userStateChanged.Login];
            if (userStateChanged.State == UserState.ONLINE)
            {
                user.currentAddress = new IPEndPoint(RemotePeer.ExtractAddress(user.staticAddresses[0]),
                                                     NetworkModule.portNr);
            }
            else
            {
                user.currentAddress = null;
            }
        }

//        public void handleArbiterChange(User sender, Message message)
//        {
//            var arbiterChange = (ArbiterChange) message;
//        }
    }
}