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
	public class User
	{
		private String pLogin;
		private Password pPassword;
		private Int32 pID;
		
		private List<UnifiedAddress> pStaticAddressess;
		private List<UnifiedAddress> pDynamicAddressess;
		private IPEndPoint pCurrentAddress;
		
		public User(String login, string password, Int32 ID)
		{
			Random random = new Random();

			pLogin = login ;
			pPassword = new Password(password);
			pID = ID;
			
			pStaticAddressess = new List<UnifiedAddress>();
			pDynamicAddressess = new List<UnifiedAddress>();
			pCurrentAddress = null;
		}
		
		public User(string login, UInt64 passwordHash, Int32 ID)
		{
			pLogin = login;
			pPassword = new Password(passwordHash);
			pID = ID;
			
			pStaticAddressess = new List<UnifiedAddress>();
			pDynamicAddressess = new List<UnifiedAddress>();
			pCurrentAddress = null;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is User)
				return (obj as User).pLogin.Equals(pLogin);
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return pLogin.GetHashCode();
		}
		
		public override string ToString()
		{
			if (currentAddress != null)
			{
				return pLogin + "(" + currentAddress.ToString() + ")";
			}
			else
			{
				return pLogin.ToString();
			}
			
		}
		
		public void AddStaticAddress(UnifiedAddress address)
		{
			if (address != null)
			{
				if (!pStaticAddressess.Contains(address))
					pStaticAddressess.Insert(0, address);
			}
		}
		
		/*
		 * Text form user's login.
		 */
		public string login
		{
			get
			{
				return pLogin;
			}
			
			set
			{
				pLogin = value;
			}
		}
		
		public Password password
		{
			get
			{
				return pPassword;
			}
			
			set
			{
				if (value != null)
					pPassword = value;
			}
		}
		
		public Int32 ID
		{
			get
			{
				return pID;
			}
		}
		
		/*
		 * Current address of connected user.	
		 */
		public IPEndPoint currentAddress
		{
			get
			{
				return pCurrentAddress;
			}
			
			set
			{
				if ((value != null) && (!pDynamicAddressess.Contains(new UnifiedAddress(value))))
					pDynamicAddressess.Insert(0, new UnifiedAddress(value));
				
				if ((pCurrentAddress == null) && (value != null))
					//Notification.Notify(this, login + " is now online");
				
				if ((pCurrentAddress != null) && (value == null))
					//Notification.Notify(this, login + " is now offline");
					
				pCurrentAddress = value;
			}
		}
		
		/*
		 * List of known static addresses (both IP or URL) in text form.
		 */
		public List<UnifiedAddress> staticAddresses
		{
			get
			{
				return new List<UnifiedAddress>(pStaticAddressess);
			}
		}
		
		/*
		 * List of observed addresses (both IP or URL) in text form.
		 */
		public List<UnifiedAddress> dynamicAddresses
		{
			get
			{
				return new List<UnifiedAddress>(pDynamicAddressess);
			}
		}
		
		/*
		 * List of all know addresses, beginning from static type.
		 */
		public List<UnifiedAddress> addresses
		{
			get
			{
				List<UnifiedAddress> result = new List<UnifiedAddress>();
				
				foreach(UnifiedAddress addr in staticAddresses)
					result.Add(addr);
				
				foreach(UnifiedAddress addr in dynamicAddresses)
					result.Add(addr);
				
				return result;
			}
		}
		
		public bool isConnected
		{
			get
			{
				return (currentAddress != null);
			}
		}
	}
}


