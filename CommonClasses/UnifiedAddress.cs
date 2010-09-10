/*
 * 
 * Author: Maciej Grabowski
 * 
 */

#region Usings
using System;
using System.Net;
#endregion

namespace Filechronization.CommonClasses
{
    [Serializable]
    public class UnifiedAddress
    {
        private readonly string pAddress;
        private int pPort;

        public UnifiedAddress(string address, int port)
        {
            if (port < 0)
                throw new ArgumentException("Port number below zero.");
            if (address.Length == 0)
                throw new ArgumentException("Address length equal to zero.");

            pAddress = address;
            pPort = port;
        }

        public UnifiedAddress(IPEndPoint IPPort)
        {
            pAddress = IPPort.Address.ToString();
            pPort = IPPort.Port;
        }

        public string address
        {
            get
            {
                return pAddress;
            }
        }
        
        public int port
        {
            get
            {
                return pPort;
            }
        }

        public override string ToString()
        {
            return (address+":"+pPort);
        }

        public override bool Equals(object obj)
        {
            if (obj is UnifiedAddress)
                return (ToString().Equals((obj as UnifiedAddress).ToString())) && (port == (obj as UnifiedAddress).port);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return pAddress.GetHashCode() + port;
        }
    }
}