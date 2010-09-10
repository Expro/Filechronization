/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Network.Tasks.Authorization
{
	#region
	using global::System.Net;
	using Authorization.Messages;
	using Modularity.Messages;
	using Network.System.MainParts;
	using Network.Tasks;
	using UserManagement;
	#endregion
	
    /* zadanie powstajace po stronie autoryzujacej sie
	 * kluczem do tego zadania musi byc obiekt klasy IPEndPoint
	 * metode Start nalezy wykonac PO dodaniu zadania do modulu zadan (dopiero wowczas znane bedzie ID)
	 */

    public class AuthorizationClientTask : NetworkTask
    {
        private User user;

        public AuthorizationClientTask(NetworkModule netModule, User user)
            : base(netModule, true)
        {
            this.user = user;

            AddHandler(typeof (SaltResponse), AnswerSaltResponse, 0);
            AddHandler(typeof (AuthorizationAccepted), HandleAuthorizationAccepted, 1);
            AddHandler(typeof (AuthorizationRejected), HandleAuthorizationRejected, 1);
        }

        public void Start()
        {
            //   messageQueue(new NetworkSend(address, CreateTaskMessage(new SaltRequest())));
            SendMessage(address, new SaltRequest());
        }


        public int AnswerSaltResponse(Message message)
        {
            SaltResponse response = (SaltResponse) message;
            Entropy saltedPassword = new Entropy(user.password.hashCode);
			
            //Notification.Diagnostic(this, "Retrived salt:" + response.salt.ToString());
            //Notification.Diagnostic(this, "Clear password:" + user.password.ToString());
            saltedPassword.Salt(response.salt);
			
            //Notification.Diagnostic(this, "Sending salted password:" + saltedPassword.ToString());
            
            SendMessage(address, new UserAuthorization(user.login, saltedPassword));

            return PHASE_NEXT;
        }

        public int HandleAuthorizationAccepted(Message message)
        {
            var mess = (AuthorizationAccepted) message;

            /* tutaj nalezy dodac kod reagujacy na poprawna autoryzacje */
         


            _netModule.PeerCenter.EndLoginToNetwork(address, mess.ArbiterLogin, mess.UserAddresses);
            SharedContext.service.EnqueueMessage(new ToInterfaceLoginResult(true, null));

            return PHASE_END;
        }

        public int HandleAuthorizationRejected(Message message)
        {
            var answer = (AuthorizationRejected) message;

            /* tutaj nalezy dodac kod reagujacy na niepoprawna autoryzacje
			 * np. opieprzajacy uzytkownika
			 */
            //NetworkModule.SendNotification(answer.message + " login: " + answer.login, NotificationType.WARNING);
            SharedContext.service.EnqueueMessage(new ToInterfaceLoginResult(false, answer.message));

            return PHASE_END;
        }

        public override bool CheckCondition()
        {
            return true;
        }

        /* adres maszyny autoryzujacej */

        public IPEndPoint address
        {
            get { return key as IPEndPoint; }
        }
    }
}