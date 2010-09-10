/*
 * Author: Piotr Trzpil
 */
namespace Filechronization.Network.Messages
{
    public class ArbiterChange : ToStateMessage
    {
        public string login { get; private set; }

        public ArbiterChange(string login)
        {
            this.login = login;
        }
    }
}