/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.UserManagement
{
	#region
	using global::System;
	using global::System.Collections.Generic;
	using global::System.Net;
	using Security;
	using CommonClasses;
	#endregion
	
	[Serializable]
	public class Users: ICollection<User>
	{
		private HashSet<User> pUsers;
		
		private SaltGenerator saltGenerator;
		private Entropy pSalt;
		
		private User pArbiter;
		
		public Users()
		{
			pUsers = new HashSet<User>();
			
			saltGenerator = new SaltGenerator();
			pSalt = saltGenerator.Next();
			
			pArbiter = null;
		}
		
		public int Count
		{
			get
			{
				return pUsers.Count;
			}
		}
		
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		
		public void Add(User item)
		{
			if (item != null)
			{	
				pUsers.Add(item);
				pSalt = saltGenerator.Next();
				
				//Notification.Diagnostic(this, "User [" + item.login + "] has beed added");
			}
		}
		
		public void Clear()
		{
			pUsers.Clear();
			pSalt = saltGenerator.Next();
		}
		
		public bool Contains(User item)
		{
			return pUsers.Contains(item);
		}
		
		public void CopyTo(User[] array, int arrayIndex)
		{
			pUsers.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(User item)
		{
			if (item != null)
			{
				pSalt = saltGenerator.Next();
				
				//Notification.Diagnostic(this, "User [" + item.login + "] has been removed");
				
				return pUsers.Remove(item);
			}
			else
				return false;
		}
		
		public IEnumerator<User> GetEnumerator()
		{
			return pUsers.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return pUsers.GetEnumerator();
		}
		
		public override bool Equals(object obj)
		{
			if (obj is Users)
				return (pUsers.Equals((obj as Users).pUsers)) && ((obj as Users).pSalt == pSalt);
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			string result = "Users list: \n";
			
			foreach (User user in users)
				result += "- " + user.ToString() + "\n";
			
			return result;
		}
		
		public User findConnectedUser(IPAddress address)
		{
			List<User> connected = connectedUsers;
			
			foreach (User user in connected)
			{
				if (user.currentAddress.Equals(address))
					return user;
			}
			
			return null;
		}
		
		/*
		 * Returns list of all know static adresses from all users excluding user
		 * passed as parameter.
		 */
		public List<UnifiedAddress> staticAddresses(User excluded)
		{
			/* TODO: Optymalizacja */

			var result = new List<UnifiedAddress>();
			bool added = true;

			for (int i = 0; added; i++ )
			{
				added = false;
				foreach (User user in users)
				{
					if (!user.Equals(excluded) && i < user.staticAddresses.Count)
					{
						result.Add(user.staticAddresses[i]);
						added = true;
					}
				}
			}	
			return result;
		}
		
		/* 
		 * Merge two users list. Current objects becomes set of unique users
		 * from both lists. Function returns list of conflicted users.
		 */
		public List<User> Merge(Users usersList)
		{
			List<User> result = new List<User>();
			
			foreach (User user in usersList)
			{
				if (!Contains(user))
					pUsers.Add(user);
				else
					result.Add(user);
			}
			pSalt = saltGenerator.Next();
			
			return result;
		}

		public User this[string login]
		{
			get
			{
				foreach (User user in pUsers)
				{
					if (user.login.Equals(login))
						return user;
				}
				
				return null;
			}
			
			set
			{
				IEnumerator<User> iterator = pUsers.GetEnumerator();
				
				iterator.Reset();
				while (iterator.MoveNext())
				{
					if (iterator.Current.login.Equals(login))
					{
						pUsers.Remove(iterator.Current);
						pUsers.Add(value);
						pSalt = saltGenerator.Next();
						return;
					}
				}
			}
		}
		
		public List<User> connectedUsers
		{
			get
			{
				List<User> result = new List<User>();
				
				foreach (User user in pUsers)
				{
					if (user.isConnected)
						result.Add(user);
				}
				
				return result;
			}
		}
		
		public List<User> disconnectedUsers
		{
			get
			{
				List<User> result = new List<User>();
				
				foreach (User user in pUsers)
				{
					if (!user.isConnected)
						result.Add(user);
				}
				
				return result;
			}
		}
	
		public List<User> users
		{
			get
			{
				return new List<User>(pUsers);
			}
		}
		
		public User arbiter
		{
			get
			{
				return pArbiter;
			}
			
			set
			{
				if (value != null)
				{
					if (value.isConnected)
						pArbiter = value;
				}
				else
					pArbiter = null;
			}
		}
		
		public Entropy salt
		{
			get
			{
				return pSalt;
			}
		}
	}
}