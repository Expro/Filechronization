// Author: Piotr Trzpil

#region Usings



#endregion

namespace Network.Connections
{
    #region Usings

    using global::System.Net;

    #endregion

    public class HostAddress : IPAddress
    {
        public HostAddress(long newAddress) : base(newAddress)
        {
        }

        public HostAddress(byte[] address, long scopeid) : base(address, scopeid)
        {
        }

        public HostAddress(byte[] address) : base(address)
        {
        }
    }
}