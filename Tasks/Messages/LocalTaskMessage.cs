/*
 * 
 * Author: Maciej Grabowski
 * 
 */
 
namespace Filechronization.Tasks.Messages
{
	#region
	using global::System;
	#endregion
	
    /*
	 * 		Komunikat zadania - wersja do uzytku lokalnego. Po odebraniu komunikatu typu TaskMessage
	 * 	wystarczy podac go wraz z kluczem do konstruktora, a nastepnie komunikat typu LocalTaskMessage
	 * 	wrzucic w obiekt klasy Service.
	 * 
	 * 		Jako klucz mozna podac dowolny obiekt charakteryzujacy odbiorce zadania. Przyklady: gniazdo
	 * 	sieciowe dla zadan typu autoryzacja/dialog sieciowy, plik dla zadan typu wysylanie fragmentow
	 * tego pliku etc.
	 */
    public class LocalTaskMessage: TaskMessage
    {
        private Object pKey;
		
        public LocalTaskMessage(Object key, TaskMessage taskMessage): base(taskMessage.message, taskMessage.taskID, taskMessage.foreignTaskID, taskMessage.synchronized)
        {
            pKey = key;
        }
		
        public override string ToString()
        {
            return "LocalTaskMessage - key: " + key.ToString() + " | " + base.ToString();
        }
        
        public Object key
        {
            get
            {
                return pKey;
            }
        }
    }
}