/*
 * 	Modul zadan
 * 
 * 		Zadania nalezy definiowac zawsze, gdy obsluga komunikatu wymaga zachowania pewnych informacji czy stanu
 * 	powiazanych z odbiorca lub nadawca komunikatu.
 * 
 * 		Na zadanie skladaja sie:
 * 		- warunek - metoda sprawdzajaca czy to zadanie nalezy w danej chwili obslugiwac
 * 		- metody obslugi - metody obslugi poszczegolnych komunikatow
 * 		- faza - deskryptor stanu, pozwala tworzyc grupy komunikatow obslugiwanych zamiennie,
 * 				 warunkiem obslugi komunikatu jest posiadanie metody obslugi zarejestrowanej dla
 * 				 danej fazy. Nowe zadania znajduja sie w fazie 0.
 *				 
 * 				 Faze nalezy zmieniac poprzez zwracana wartosc z metody obslugi. Obslugiwane wartosci:
 * 				 - PHASE_BEGIN - ustawia faze na poczatkowa (0)
 *				 - PHASE_END - informuje modul zadan o zakonczeniu obslugi tego zadania
 * 				 - PHASE_BLOCK - pozwala blokowac zadanie (nadchodzace komunikaty beda ignorowane)
 * 				 - PHASE_CURRENT - faza nie zostanie zmieniona
 * 				 - PHASE_NEXT - spowoduje przejscie do nastepnej fazy (np. z 3 n 4)
 * 				 - PHASE_PREVIOUS - spowoduje cofniecie do porzedniej fazy
 * 				 - dowolna dodatnia wartosc - spowoduje ustawienie fazy na wskazana (umozliwia skoki)
 * 
 * 				 Dobrym przykladem dzialania faz jest zadanie AuthorizationClientTask:
 * 				 - w fazie 0 zadanie bedzie oczekiwac na sol nadeslana przez arbitra (i ignorowac inne komunikaty, zablakane lub bledne)
 * 				   po otrzymaniu soli przejdzie do fazy 1 (a co za tym idzie kolejne komunikaty z sola zostana zignorowane)
 * 				 - w fazie 1 zadanie bedzie obslugiwac dwie mozliwe odpowiedzi na probe autoryzacji: powodzenie lub porazka,
 * 				   po otrzymaniu ktoregos z tych komunikatow zadanie przejdzie do fazy PHASE_END
 * 				 - w fazie PHASE_END zadanie zostanie zamkniete i automatycznie usuniete
 * 
 * 
 * 		- ID - numer zadania (nadawany automatycznie)
 * 		- klucz - identyfikator do kogo nalezy zadanie (np. do gniazda sieciowego)
 * 		- znacznik unikatowosci - okresla czy moze wystapic wiecej niz jedno zadanie danego typu dla danego klucza
 * 								  np. autoryzacja jest unikatowa, nie mozna autoryzowac sie dwa razy w tym samym czasie
 * 								  z ta sama maszyna
 * 	
 * 		Typowy scenariusz uzycia zadania:
 * 		1. Utworzenie zadania
 * 		2. Zarejestrowanie zadania w module zadan (zostanie nadane wowczas ID, a zadanie zostanie odblokowane)
 * 		3. Obsluga naplywajacych komunikatow
 * 			3.1 Sterujac wartoscia fazy mozna sterowac samym zadaniem.
 * 		4. Zakonczenie zadania i jego usuniecie (zazwyczaj automatycznie, przez ustawienie fazy na PHASE_END)
 */
/*
 * 
 * Author: Maciej Grabowski
 * 
 */

#region Usings
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Filechronization.Modularity;
using Filechronization.Modularity.Messages;
using Filechronization.Tasks.Messages;
using CodeManagement;
using CodeManagement.Definitions;
#endregion

namespace Filechronization.Tasks
{
	public delegate int TaskHandler(Message message);
	public delegate SymTask TaskCreator();
	public delegate bool Condition();
	
	[Name("Task Manager")]
	[Version(1, 0, 0)]
	[Author("Maciej 'Expro' Grabowski", "mds.expro@gmail.com")]
	[Description("Provides task handling and synchronization.")]
	public class TaskManager: SharedCode
	{
		private ConcurrentDictionary<Object, ConcurrentDictionary<uint, SymTask>> tasksLists;
		private ConcurrentDictionary<Type, TaskCreator> initializableTasks;
		private Service service;
		
		public TaskManager(Service service)
		{
			if (service == null)
				throw new ArgumentNullException(String.Format("{0} canot be null.", typeof(Service)));
			
			tasksLists = new ConcurrentDictionary<Object, ConcurrentDictionary<uint, SymTask>>();
			initializableTasks = new ConcurrentDictionary<Type, TaskCreator>();
			this.service = service;
			
			service.Register(typeof(LocalTaskMessage), ProcessMessage);
		}
		
		public void AddInitializableTask(Type messageType, TaskCreator taskType)
		{
			//initializableTasks.Add(messageType, taskType);
			//Notification.Diagnostic(this, "Assigned " + messageType.Name + " with task creator");
		}
		
		public uint AddTask(Object key, SymTask symTask)
		{
			uint result = 0;
			bool found = false;
			ConcurrentDictionary<uint, SymTask> tasks = null;
			
			if ((key != null) && (symTask != null))
			{
				if (!tasksLists.ContainsKey(key))
				{
					tasks = new ConcurrentDictionary<uint, SymTask>();
					//tasksLists.Add(key, tasks);
				}
				else
				{
					tasks = tasksLists[key];
					while (!found)
					{
						found = true;
						foreach (SymTask t in tasks.Values)
						{
							if (t.taskID == result)
							{
								found = false;
								break;
							}
						}
						
						if (!found)
							++result;
					}
				}
				
				if (symTask.isUnique)
				{
					foreach (SymTask t in tasks.Values)
					{
						if (t.GetType().Equals(symTask.GetType()))
						{
							//tasks.Remove(t.taskID);
							//Notification.Diagnostic(this, "Due to isUnique flag, task (" + t.ToString() + ") was removed and will be replaced by task (" + task.ToString() + ")");
							break;
						}
					}
				}
				
				symTask.key = key;
				symTask.taskID = result;
				symTask.messageQueue = service.InputAdapter();
				//tasks.Add(result, task);
				
				//Notification.Diagnostic(this, "Task added: " + task.ToString());
			}
			else
				throw new ArgumentNullException();
			
			return result;
		}
		
		public void ProcessMessage(Message message)
		{
			LocalTaskMessage taskMessage = null;
			ConcurrentDictionary<uint, SymTask> tasks = null;
			SymTask symTask = null;
			
			taskMessage = (LocalTaskMessage)message;
			
			//Notification.Diagnostic(this, "Recived task message: " + taskMessage.ToString());
			
			try
			{
				if (!taskMessage.synchronized)
				{
					if (initializableTasks.ContainsKey(taskMessage.message.GetType()))
						taskMessage.taskID = AddTask(taskMessage.key, initializableTasks[taskMessage.message.GetType()]());
					//else
						//Notification.Warning(this, "Cannod synchronize with foreign task, missing initializable creator. Message: " + taskMessage.ToString());
				}
	
				tasks = tasksLists[taskMessage.key];
				if (tasks.ContainsKey(taskMessage.taskID))
				{
					symTask = tasks[taskMessage.taskID];
					
					//TODO: Dodac szeregowanie elementow o tym samym kluczu
					System.Threading.Tasks.Task.Factory.StartNew(delegate()
					{
				 		if (!symTask.synchronized)
				 			symTask.foreignTaskID = taskMessage.foreignTaskID;
	
						if (symTask.ProcessMessage(taskMessage.message))
						{
							//tasks.Remove(taskMessage.taskID);
							//Notification.Diagnostic(this, "Task finished: " + task.ToString());
						}
					});
				}
				//else
					//Notification.Warning(this, "Ignored message with incorrect task ID. Message: " + taskMessage.ToString());
			}
			catch (KeyNotFoundException )
			{
				//Notification.Warning(this, "Ignored message with incorrect key. Message: " + taskMessage.ToString());
			}
		}
		
		public override void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}


