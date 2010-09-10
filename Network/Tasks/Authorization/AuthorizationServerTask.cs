/*
 * 
 * Author: Maciej Grabowski
 * 
 */

#region Usings
using System.Collections.Generic;
using System.Net;
using Filechronization.Network.Tasks.Authorization.Messages;
using Filechronization.Modularity.Messages;
using Filechronization.Network.States;
using Filechronization.Network.System.MainParts;
using Filechronization.Network.Tasks;
using Filechronization.UserManagement;
using Filechronization.Security;
#endregion

namespace Filechronization.Network.Tasks.Authorization
{
	
	
	/* To zadanie powstaje po stronie arbitra autoryzujacego nieznane IP */
	/* kluczem do tego zadania musi byc obiekt klasy IPEndPoint */

	public class AuthorizationServerTask : NetworkTask
	{
		private Users users;

		private SaltGenerator generator;
		private Entropy salt;

		public AuthorizationServerTask(NetworkModule netModule, Users users): base(netModule, true)
		{
			generator = new SaltGenerator();

			this.users = users;


			AddHandler(typeof (SaltRequest), AnswerSaltRequest, 0);
			AddHandler(typeof (UserAuthorization), AnswerUserAuthorization, 1);
		}

		public override bool CheckCondition()
		{
			/* tu nalezy sprawdzic czy uzytkownik jest arbitrem */
			return _netModule.NetworkState is StateArbiter;
		}

		public int AnswerSaltRequest(Message message)
		{
			var req = (SaltRequest) message;

			salt = generator.Next();
			//Notification.Diagnostic(this, "Generated salt:" + salt.ToString());
		  
			SendMessage(address, new SaltResponse(salt));
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



					user.currentAddress = address;

					var map = new Dictionary<string, IPAddress>();
					foreach (var user1 in users)
					{
						IPAddress addr = user1.currentAddress == null ? null : user1.currentAddress.Address;
						map.Add(user1.login, addr);
					}



					
					/* tutaj nalezy dodac jeszcze broadcastowa informacje o IP pod jakim zalogowal sie
			 		* uzytkownik
			 		*/
					_netModule.PeerCenter.EndUserLogin(user, address);

					SendMessage(address, new AuthorizationAccepted(userMessage.login, _netModule.CurrentUser.login, map));
					return PHASE_END;
				}
				else
					//Notification.Warning(this, "Rejected incorrect authorization session for login [" + user.login + "]");
			}
			else
				//Notification.Warning(this, "Rejected incorrect authorization login: " + userMessage.login);

			SendMessage(address, new AuthorizationRejected(userMessage.login, "Incorrect login or password"));

			return PHASE_END;
		}


		/* adres autoryzowanej maszyny */

		public IPEndPoint address
		{
			get { return key as IPEndPoint; }
		}
	}
}