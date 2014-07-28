// Author: Piotr Trzpil

namespace FileModule
{
    #region Usings

    using System;

    #endregion

    public static class Bug
    {
        public static void Pr(object obj)
        {
            Console.WriteLine(obj);
        }

        public static void Err(object obj)
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