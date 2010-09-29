namespace Network
{
    using Filechronization.Modularity;
    using Filechronization.Tasks;

    public class Global
    {
        private static TaskManager _taskManager;

        public static TaskManager TaskManager
        {
            get { return _taskManager; }
        }
        public static Service Service
        {
            get
            {
                return null;
            }
        }
    }
}