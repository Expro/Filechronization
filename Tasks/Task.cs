/*
 * 
 * Author: Maciej Grabowski
 * 
 */

using System;
using System.Collections.Concurrent;

namespace Filechronization.Tasks
{
	#region
	using Modularity;
	using Modularity.Messages;
	using Tasks.Messages;
	#endregion
	
    /*
	 * 		Klasa bazowa dla wszystkich klas zadan. Zapewnia wszystkie informacje identyfikujace
	 * 	zadanie (klucz, ID), obsluge filtrowania wiadomosci (przez warunek oraz faze) oraz podstawowy
	 * 	kontekst do komunikacy (messageQueue przekazuje komunikaty do obiektu klasy Service).
	 */
    public abstract class Task
    {
        private class TaskHandlerDescriptor
        {
            private int pAllowedPhase;
            private TaskHandler pTaskHandler;
			
            public TaskHandlerDescriptor(TaskHandler taskHandler, int allowedPhase)
            {
                pTaskHandler = taskHandler;
                pAllowedPhase = allowedPhase;
            }
			
            public int allowedPhase
            {
                get
                {
                    return pAllowedPhase;
                }
            }
			
            public TaskHandler taskHandler
            {
                get
                {
                    return pTaskHandler;
                }
            }
        }
		
        private Object pKey;
        private uint pTaskID;
        private uint pForeignTaskID;
        private bool pSynchronized;
        private int phase;
		
        private bool pIsUnique;
		
        private ConcurrentDictionary<Type, TaskHandlerDescriptor> handlers;
        private InputAdapter<Message> pMessageInput;
		
        public const int PHASE_BEGIN = 0;
        public const int PHASE_END = -1;
        public const int PHASE_BLOCK = -2;
        public const int PHASE_CURRENT = -3;
        public const int PHASE_NEXT = -4;
        public const int PHASE_PREVIOUS = -5;
		
        public Task(bool isUnique)
        {
            pKey = null;
            pTaskID = 0;
            phase = PHASE_BLOCK;
            pIsUnique = isUnique;
            pMessageInput = null;
            pSynchronized = false;
            pForeignTaskID = 0;
			
            handlers = new ConcurrentDictionary<Type, TaskHandlerDescriptor>();
        }
		
        public void AddHandler(Type messageType, TaskHandler handler, int allowedPhase)
        {
            //if (allowedPhase >= 0)
                //handlers.Add(messageType, new TaskHandlerDescriptor(handler, allowedPhase));
        }
		
        public bool ProcessMessage(Message message)
        {
            TaskHandlerDescriptor descriptor = handlers[message.GetType()];
            int result = PHASE_BLOCK;
            bool isFinished = false;
			
            if ((descriptor != null) && (CheckCondition()))
            {
                if (phase == descriptor.allowedPhase)
                {
                    result = descriptor.taskHandler(message);
                    switch(result)
                    {
                        case PHASE_CURRENT:
                            /* nothing to do */
                            break;
                        case PHASE_NEXT:
                            ++phase;
                            break;
                        case PHASE_PREVIOUS:
                            --phase;
                            break;
                        case PHASE_END:
                            isFinished = true;
                            break;
                        default:
                            phase = result;
                            break;
                    }
                }
            }
			
            return isFinished;
        }
		
        /* zamiana foreignTaskID -> taskID jest tutaj celowa */
        public virtual TaskMessage CreateTaskMessage(Message message)
        {
            return new TaskMessage(message, foreignTaskID, taskID, pSynchronized);
        }
        
        public override string ToString()
        {
        	string keyString = "";
        	
        	if (key != null)
        		keyString = key.ToString();
        	
            if (synchronized)
                return GetType().Name + " - key: " + keyString + " | taskID: " + taskID.ToString() + " | foreignID: " + foreignTaskID.ToString();
            else
                return GetType().Name + " - key: " + keyString + " | taskID: " + taskID.ToString();
        }	
		
        public abstract bool CheckCondition();
		
        public uint taskID
        {
            get
            {
                return pTaskID;
            }
			
            set
            {
                if (phase == PHASE_BLOCK)
                {
                    phase = PHASE_BEGIN;
                    pTaskID = value;
                }
            }
        }
        
        public uint foreignTaskID
        {
            get
            {
                return pForeignTaskID;
            }
			
            set
            {
                if (!pSynchronized)
                {
                    pSynchronized = true;
                    pForeignTaskID = value;
                }
            }
        }
		
        public Object key
        {
            get
            {
                return pKey;
            }
			
            set
            {
                if (pKey == null)
                    pKey = value;
            }
        }
		
        public bool isUnique
        {
            get
            {
                return pIsUnique;
            }
        }
        
        public bool synchronized
        {
            get
            {
                return pSynchronized;
            }
        	
            set
            {
                if (!pSynchronized)
                    pSynchronized = value;
            }
        }
		
        public InputAdapter<Message> messageQueue
        {
            get
            {
                return pMessageInput;
            }
			
            set
            {
                if (pMessageInput == null)
                    pMessageInput = value;
            }
        }
    }
}