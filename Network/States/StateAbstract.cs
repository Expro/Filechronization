// Author: Piotr Trzpil
namespace Network.States
{
    #region Usings

    using System.MainParts;
    using Filechronization.Modularity.Messages;
    using Filechronization.UserManagement;
    using global::System;
    using global::System.Collections.Generic;

    #endregion

    #region Usings

    #endregion

    /// <summary>
    ///   Reprezentuje stan aktualnego polaczenia do sieci
    /// </summary>
    public abstract class StateAbstract
    {
        #region Delegates

        public delegate void MessageHandler(User sender, Message message);

        #endregion

        protected readonly NetworkModule _netModule;
        protected readonly NetQueue _netQueue;


        protected Dictionary<Type /*Message*/, MessageHandler> _messageAssociations;

        protected StateAbstract(NetworkModule netModule)
        {
            _netModule = netModule;
            _netQueue = netModule.netQueue;

            _messageAssociations = new Dictionary<Type, MessageHandler>();

            BuildTaskAssociations();
        }

        /**
         * Podlaczenie typow, by dana wiadomosc powodowala tworzenie zadania danego typu
         * oraz przypisanie wiadomosci do metod je obslugujacych
         */
        public abstract void BuildTaskAssociations();


        public bool handleMessage(User sender, Message message)
        {
            MessageHandler handler;
            if (_messageAssociations.TryGetValue(message.GetType(), out handler))
            {
                handler(sender, message);
                return true;
            }
            else
            {
//                P.pr("Unhandled message: " + message);
                return false;
            }
        }
    }
}