namespace ConsoleApplication1
{
    using System.Net;

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