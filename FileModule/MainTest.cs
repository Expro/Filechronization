// Author: Piotr Trzpil

namespace FileModule
{
    #region Usings

    using System;
    using CodeExecutionTools.Logging;

    #endregion

    public class MainTest
    {
        public static void Main()
        {
            //LoggingService.Trace.Error("ss");
            NewFileModule mod = new NewFileModule(null);
            
            while (Console.ReadLine() != "q")
            {
            }
        }
    }
}