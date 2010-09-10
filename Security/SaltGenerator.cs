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
	
    public class SaltGenerator
    {
        private Random random;
		
        public SaltGenerator()
        {
            random = new Random();
        }
		
        public SaltGenerator(int seed)
        {
            random = new Random(seed);
        }
		
        public Entropy Next()
        {
            Entropy result = new Entropy();
            byte[] array = new byte[Entropy.BytesCount];
            byte[] converted = null;
            int rand;
            int i;
			
            for (i = 0; i < array.Length; ++i)
            {
                if (i%4 == 0)
                {
                    rand = random.Next();
                    converted = System.BitConverter.GetBytes(rand);
                }
                array[i] = converted[i%4];
            }
            result.AddBytesArray(array);
			
            return result;
        }
    }
}


