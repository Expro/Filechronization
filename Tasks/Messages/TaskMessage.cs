/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Tasks.Messages
{
	#region
	using global::System;
	using Modularity.Messages;
	#endregion
	
    /*
	 * Komunikat zadania - wersja do przesylania przez siec.
	 */
    [Serializable]
    public class TaskMessage: Message
    {
        private uint pTaskID;
        private uint pForeignTaskID;
        private bool pSynchronized;
        private Message pMessage;
		
        public TaskMessage(Message message, uint foreignTaskID): base()
        {
            pTaskID = 0;
            pForeignTaskID = foreignTaskID;
            pSynchronized = false;
            pMessage = message;
        }
        
        public TaskMessage(Message message, uint taskID, uint foreignTaskID, bool synchronized): base()
        {
            pTaskID = taskID;
            pForeignTaskID = foreignTaskID;
            pSynchronized = synchronized;
            pMessage = message;
        }
        
        public override string ToString()
        {
            if (synchronized)
                return "taskID: " + taskID.ToString() + " | foreignID: " + foreignTaskID.ToString() + " | message: " + message.GetType().Name;
            else
                return "taskID: " + taskID.ToString() + " | message: " + message.GetType().Name;
        }
		
        public uint taskID
        {
            get
            {
                return pTaskID;
            }
            
            set
            {
                if (!synchronized)
                    pTaskID = value;
            }
        }
        
        public uint foreignTaskID
        {
            get
            {
                return pForeignTaskID;
            }
        }
        
        public bool synchronized
        {
            get
            {
                return pSynchronized;
            }
        }
		
        public Message message
        {
            get
            {
                return pMessage;
            }
        }
    }
}