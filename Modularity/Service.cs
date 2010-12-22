/*
 * 
 * Author: Maciej Grabowski
 * 
 */

#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using Filechronization.Modularity.Messages;
using CodeManagement;
using CodeManagement.Definitions;
#endregion

namespace Filechronization.Modularity
{
	#region Comment
	/// <summary>
	/// 	Enviroment for transported parts of code. Allows to process code on other thread.
	/// </summary>
	#endregion
	public delegate void Processor();
	
	#region Comment
	/// <summary>
	/// 	Pointer to foreign method handling assigned type of message.
	/// </summary>
	#endregion
	public delegate void MessageEvent(Message message);
	
	#region Comment
	/// <summary>
	/// 	Wrapper for processed message handler.
	/// </summary>
	/// <remarks>
	/// 	May be used as way to enqueue code to proper processor.
	/// </remarks>
	#endregion
	public delegate void ServiceTask();
	
	#region Comment
	/// <summary>
	/// 	Message transport and handling coordinator. Provides possibility to recognize type of incoming message, post it to proper handler and execute it on pointed thread.
	/// </summary>
	#endregion
	[Name("Service")]
	[Version(1, 0, 0)]
	[Author("Maciej 'Expro' Grabowski", "mds.expro@gmail.com")]
	[Description("Message communication provider, core of intermodule invocations.")]
	[Module]
	public class Service
	{
        private ConcurrentDictionary<Type, Action<Message>> pServices;
		private ConcurrentDictionary<Processor, BlockingCollection<Action>> pCores;
		private ConcurrentDictionary<Type, Processor> pProcessors;
		
		private event MessageEvent pDefaultMessageHandler;
		
		private void NullHandler(Message message)
		{
			/* no action on message */
		}
		
		#region Comment
		/// <summary>
		/// 	Creates <c>Service</c> object with empty hanlders and procesors lists.
		/// </summary>
		#endregion
		public Service()
		{
            pServices = new ConcurrentDictionary<Type, Action<Message>>();
            pCores = new ConcurrentDictionary<Processor, BlockingCollection<Action>>();
			pProcessors = new ConcurrentDictionary<Type, Processor>();
			
			pDefaultMessageHandler = NullHandler;
		}

		#region Comment
		/// <summary>
		/// 	Redirects message to handler and transports it to assigned processor.
		/// </summary>
		/// <param name="message">
		/// 	Message to be handled.
		/// </param>
		#endregion
		public void EnqueueMessage(Message message)
		{
			Processor processor;
            Action<Message> messageService = null;
			
			Monitor.Enter(this);
			
			if (pProcessors.ContainsKey(message.GetType()))
			{
				processor = pProcessors[message.GetType()];
				
				//if (pCores.ContainsKey(processor))
					//pCores[processor].Enqueue(delegate() {pServices[message.GetType()](message);});
				
				Monitor.Exit(this);
				
				return;
			}
			else
			{
				messageService = pServices[message.GetType()];
				
				if (messageService != null)
					messageService(message);
				else
				{
				}
				
				Monitor.Exit(this);
			}
		}
		
		#region Comment
		/// <summary>
		/// 	Insers wrapped code into particula processor queue.
		/// </summary>
		/// <param name="task">
		/// 	Wrapped code, will be executed on particular processor.
		/// </param>
		/// <param name="processor">
		///		Executor of <c>task</c> code.
		/// </param>
		#endregion
		public void EnqueueTask(ServiceTask task, Processor processor)
		{
			Monitor.Enter(this);
			
			if (processor != null)
			{
				//if (pCores.ContainsKey(processor))
					//pCores[processor].Enqueue(task);
			}
			else
				throw new ArgumentNullException();
			
			Monitor.Exit(this);
		}
		
		#region Comment
		/// <summary>
		/// 	Registers method as handler for this type of message.
		/// </summary>
		/// <param name="messageType">
		/// 	Type of message acting as key for handler.
		/// </param>
		/// <param name="messageEvent">
		/// 	Handler for pointed type fo message.
		/// </param>
		#endregion
		public void Register(Type messageType, MessageEvent messageEvent)
		{
			Monitor.Enter(this);
			
			//if (!pServices.ContainsKey(messageType))
				//pServices.Add(messageType, defaultMessageHandler + messageEvent);
			//else
				//pServices[messageType] += messageEvent;
			
			Monitor.Exit(this);
		}

        private Dictionary<Type, Action<Message>> actdict;
        public void Register<TMessage>(Action<TMessage> messageEvent)where TMessage : Message
        {
            
            actdict.Add(typeof(TMessage), (message) =>
            {

                messageEvent((TMessage)message);
                //messageService(message);
            });
        }
		#region Comment
		/// <summary>
		/// 	Removes method from list of handlers for this type of message.
		/// </summary>
		/// <param name="messageType">
		/// 	Type of message acting as key for handler.
		/// </param>
		/// <param name="messageEvent">
		/// 	Handler to be removed.
		/// </param>
		#endregion
        public void Unregister(Type messageType, Action<Message> messageEvent)
		{
			Monitor.Enter(this);
			
			if (pServices.ContainsKey(messageType))
				pServices[messageType] -= messageEvent;
			
			Monitor.Exit(this);
		}
		
		#region Comment
		/// <summary>
		/// 	Creates new processor. Processors works as thread enviroment, provides method of serial code execution.
		/// </summary>
		/// <returns>
		/// 	Reference to new, empty processor.
		/// </returns>
		#endregion
		public Processor CreateProcessor()
		{
			Processor result = null;
			
			Monitor.Enter(this);
			
			BlockingCollection<ServiceTask> queue = new BlockingCollection<ServiceTask>();
			
			result = delegate()
			{
				//ServiceTask task = queue.Dequeue();
				
				//task();
			};
			
			//pCores.Add(result, queue);
			
			Monitor.Exit(this);
			
			return result;
		}
		
		#region Comment
		/// <summary>
		/// 	Removes processor from being able to process messages.
		/// </summary>
		/// <param name="processor">
		/// 	Processor to be disabled and cleared.
		/// </param>
		#endregion
		public void DestroyProcessor(Processor processor)
		{
			Monitor.Enter(this);
			
			IEnumerator<KeyValuePair<Type, Processor>> iterator = pProcessors.GetEnumerator();

			while (iterator.MoveNext())
			{
				if (iterator.Current.Value.Equals(processor))
				{
					//pProcessors.Remove(iterator.Current.Key);
					break;
				}
			}
			
			//if (pCores.ContainsKey(processor))
				//pCores.Remove(processor);
			
			processor = null;
			
			Monitor.Exit(this);
		}
		
		#region Comment
		/// <summary>
		/// 	Marks processor as able to process this type of messages.
		/// </summary>
		/// <param name="messageType">
		/// 	Type of message that will be transported to particular processor.
		/// </param>
		/// <param name="processor">
		/// 	Processor to be marked as executor for this tape of message.
		/// </param>
		#endregion
		public void AssignProcessor(Type messageType, Processor processor)
		{
			Monitor.Enter(this);
			
			if (pProcessors.ContainsKey(messageType))
				//pProcessors.Remove(messageType);
			
			//pProcessors.Add(messageType, processor);
			
			Monitor.Exit(this);
		}
		
		#region Comment
		/// <summary>
		/// 	Marks processor as not able to process this type of messages.
		/// </summary>
		/// <param name="messageType">
		/// 	Type of message which is assigned to processor.
		/// </param>
		#endregion
		public void UnassignProcessor(Type messageType)
		{
			Monitor.Enter(this);
			
			if (pProcessors.ContainsKey(messageType))
				//pProcessors.Remove(messageType);
			
			Monitor.Exit(this);
		}

		#region Comment
		/// <summary>
		/// 	For each invocation, creates new abstract input adapter for messages to be redirected to <see cref="EnqueueMessage"/> method.
		/// </summary>
		/// <returns>
		/// 	Input adapter assigned with this instance of <c>Service</c> class.
		/// </returns>
		#endregion
		public InputAdapter<Message> InputAdapter()
		{
			InputAdapter<Message> result = EnqueueMessage;
			
			return result;
		}
		
		#region Comment
		/// <summary>
		/// 	Access to particular message handlers.
		/// </summary>
		#endregion
        public Action<Message> this[Type messageType]
		{
			get
			{
                Action<Message> result;
				
				Monitor.Enter(this);
				
				//if (!pServices.ContainsKey(messageType))
					//pServices.Add(messageType, defaultMessageHandler);
				
				result = pServices[messageType];
				
				Monitor.Exit(this);
				
				return result;
			}
			
			set
			{
				Monitor.Enter(this);
				
				//if (!pServices.ContainsKey(messageType))
					//pServices.Add(messageType, defaultMessageHandler + value);
				//else
					//pServices[messageType] = value;
				
				Monitor.Exit(this);
			}
		}
		
		#region Comment
		/// <summary>
		/// 	Default handler for all messages, assigned to all types of messages.
		/// </summary>
		#endregion
		public MessageEvent defaultMessageHandler
		{
			get
			{
				MessageEvent result;
				
				Monitor.Enter(this);
				
				result = NullHandler + pDefaultMessageHandler;
				
				Monitor.Exit(this);
				
				return result;
			}
			
			set
			{
				Monitor.Enter(this);

                Action<Message> messageEvent;
				
				if (pDefaultMessageHandler != null)
				{
					foreach (Type key in pServices.Keys)
					{
						messageEvent = pServices[key];
						
						messageEvent -= pDefaultMessageHandler;
						messageEvent += value;
					}
				}
				
				pDefaultMessageHandler = value;
				
				Monitor.Exit(this);
			}
		}
	}
}