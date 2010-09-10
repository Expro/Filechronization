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
    public class Entropy
    {
        private UInt64 pValue;
		
        public static UInt64 MaxValue
        {
            get
            {
                return UInt64.MaxValue;
            }
        }
		
        public static UInt64 MinValue
        {
            get
            {
                return UInt64.MinValue;
            }
        }
		
        public static int BitsCount
        {
            get
            {
                return 64;
            }
        }
		
        public static int BytesCount
        {
            get
            {
                return BitsCount/8;
            }
        }
		
        public Entropy()
        {
            pValue = UInt64.MinValue;
        }
		
        public Entropy(UInt64 value)
        {
            pValue = value;
        }
		
        public Entropy(Entropy entropy)
        {
            pValue = entropy.value;
        }
		
        public UInt64 value
        {
            get
            {
                return pValue;
            }
			
            set
            {
                pValue = value;
            }
        }
		
        public void AddBytesArray(byte[] array)
        {
            int i;
			
            for (i = 0; i < array.Length; ++i)
                value += ((UInt64)array[i]) << (i*8)%BytesCount;
        }
		
        public void Salt(Entropy salt)
        {
            pValue = pValue^salt.pValue;
        }
		
        public override string ToString()
        {
            return value.ToString("X4");
        }
		
        public override bool Equals(object obj)
        {
            if (obj is Entropy)
                return value.Equals(((Entropy)obj).value);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return pValue.GetHashCode();
        }
    }
}


