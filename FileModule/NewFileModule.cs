namespace FileModule
{
    using CodeManagement.Definitions;

  

    [Name("File Module")]
    [Version(1, 0, 0)]
    [Author("Piotr 'Gibz' Trzpil", "gibzgibz@gmail.com")]
    [Description("Watches file changes in storage folder and handles peer to peer filesharing.")]
    [Module]
    public class NewFileModule
    {
        public MessageDispatcher dispatcher;
        public FileWatcher fileWatcher;
        public TableOverseer tableOverseer;


        public NewFileModule(NetworkModel network)
        {
            Network = new NetworkModel(@"C:\Test2");
            Network.GroupList.Add(new GroupModel(new[] {"Fol1", "Fol2"}));


            tableOverseer = new TableOverseer(Network);

            fileWatcher = new FileWatcher(this);

            dispatcher = new MessageDispatcher(this);


            LoginEnded();
            // logowanie arbitra:
            // tworzy liste plikow
            // czeka na innych


            // hash all files

            // send list to arbiter

            // arbiter: merge lists: Arbiter + User
            // send changes to all ( User - Arbiter)
            // send to user (Arbiter - User)
        }

        public NetworkModel Network { get; set; }


        public void LoginEnded()
        {
            tableOverseer.IndexAllFiles();
            fileWatcher.Active = true;
        }
    }
}