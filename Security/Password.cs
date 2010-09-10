/*
 * 
 * Author: Maciej Grabowski
 * 
 */

namespace Filechronization.Security
{
	#region
	using global::System;
	#endregion
	
    [Serializable]
    public class Password
    {
        private Entropy hash;
		
        private void hashString(string value)
        {
            hash.value = (UInt64)value.GetHashCode();
            //Notification.Diagnostic(this, "Password string: " + value + " | Password hash: " + hash.ToString());
        }
		
        public Password(string value)
        {
            hash = new Entropy();
            hashString(value);
        }
		
        public Password(UInt64 passwordHash)
        {
            hash = new Entropy();
            hash.value = passwordHash;
        }
		
        public override int GetHashCode()
        {
            return (int)hashCode.value;
        }
		
        public override string ToString()
        {
            return hash.ToString();
        }
		
        public override bool Equals(object obj)
        {
            if (obj is Password)
                return hashCode.Equals(((Password)obj).hashCode);
            else
                return false;
        }
		
        public string password
        {
            get
            {
                return hash.ToString();
            }
			
            set
            {
                hashString(value);
            }
        }
		
        public Entropy hashCode
        {
            get
            {
                return hash;
            }
			
            set
            {
                hash = value;
            }
        }
    }
}


