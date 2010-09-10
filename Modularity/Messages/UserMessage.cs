/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Modularity.Messages
{
	#region
	using global::System;
	#endregion
	
    [Serializable]
    public abstract class UserMessage: Message
    {
        // to pole jest uzupelniane po odebraniu wiadomosci z sieci
        // wpisywany jest w nie obiekt typu User
        public object UserSender;






    }
}


