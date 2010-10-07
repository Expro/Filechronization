// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.Tasks.Authorization
{
    #region Usings

    using System.Connections;
    using System.MainParts;
    using Filechronization.Modularity.Messages;
    using Filechronization.Security;
    using Filechronization.UserManagement;
    using global::System.Collections.Generic;
    using global::System.Net;
    using Messages;
    using States;

    #endregion

    /* To zadanie powstaje po stronie arbitra autoryzujacego nieznane IP */
    /* kluczem do tego zadania musi byc obiekt klasy IPEndPoint */

    public class AuthorizationServerSymTask : NetworkSymTask
    {
        private readonly SaltGenerator generator;
        private readonly Users users;
        private Entropy salt;

        public AuthorizationServerSymTask(NetworkModule netModule, Users users) : base(netModule, true)
        {
            generator = new SaltGenerator();

            this.users = users;


            AddHandler(typeof (SaltRequest), AnswerSaltRequest, 0);
            AddHandler(typeof (UserAuthorization), AnswerUserAuthorization, 1);
        }

        public Peer PeerHandle
        {
            get
            {
                return key as Peer;
            }
        }

        public override bool CheckCondition()
        {
            /* tu nalezy sprawdzic czy uzytkownik jest arbitrem */
            return _netModule.NetworkState is StateArbiter;
        }

        public int AnswerSaltRequest(Message message)
        {
            SaltRequest req = (SaltRequest) message;

            salt = generator.Next();
            //Notification.Diagnostic(this, "Generated salt:" + salt.ToString());

            PeerHandle.Send(new SaltResponse(salt));
            return PHASE_NEXT;
        }

        public int AnswerUserAuthorization(Message message)
        {
            UserAuthorization userMessage = (UserAuthorization) message;
            User user = users[userMessage.login];

            //Notification.Diagnostic(this, "Retrived authorization data (Login: " + userMessage.login.ToString() + " | Salted password: " + userMessage.saltedPassword.ToString() + ")");

            if (user != null)
            {
                userMessage.saltedPassword.Salt(salt);
                //Notification.Diagnostic(this, "Password after removing salt: " + userMessage.saltedPassword.ToString());
                //Notification.Diagnostic(this, "Correct password for user [" + user.login + "]:" + user.password.ToString());

                if (userMessage.saltedPassword.Equals(user.password.hashCode))
                {
                    user.currentAddress = PeerHandle.Endpoint;

                    Dictionary<string, IPAddress> map = new Dictionary<string, IPAddress>();
                    foreach (User user1 in users)
                    {
                        IPAddress addr = user1.currentAddress == null ? null : user1.currentAddress.Address;
                        map.Add(user1.login, addr);
                    }


                    /* tutaj nalezy dodac jeszcze broadcastowa informacje o IP pod jakim zalogowal sie
			 		* uzytkownik
			 		*/
                    _netModule.NetworkManager.EndUserLogin(user, PeerHandle);

                    PeerHandle.Send(new AuthorizationAccepted(userMessage.login, _netModule.CurrentUser.login, map));
                    
                    return PHASE_END;
                }
                //else
                //Notification.Warning(this, "Rejected incorrect authorization session for login [" + user.login + "]");
            }
            else
                //Notification.Warning(this, "Rejected incorrect authorization login: " + userMessage.login);

                PeerHandle.Send( new AuthorizationRejected(userMessage.login, "Incorrect login or password"));

            return PHASE_END;
        }


        /* adres autoryzowanej maszyny */
    }
}