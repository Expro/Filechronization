// Author: Piotr Trzpil
namespace Network
{
    #region Usings

    using global::System;

    #endregion

    public static class Bug
    {
        public static void Pr(object obj)
        {
            Console.WriteLine(obj);
#if DEBUG
            if (obj is Exception)
            {
                throw obj as Exception;
            }
#endif
        }
    }
}