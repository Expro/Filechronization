// Author: Piotr Trzpil
namespace Network.Messages
{
    public class ArbiterChange : ToStateMessage
    {
        public ArbiterChange(string login)
        {
            this.login = login;
        }

        public string login { get; private set; }
    }
}