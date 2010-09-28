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
//        public MessageDispatcher dispatcher;
//        public FileWatcher fileWatcher;
//        public TableOverseer tableOverseer;
        public NetworkContext _context;

        public NewFileModule(NetworkModel network)
        {
            network = new NetworkModel(@"C:\Test2");
            network.GroupList.Add(new GroupModel(new[] {"Fol1", "Fol2"}));

            _context = new NetworkContext(this, network, network.MainPath);
//            tableOverseer = new TableOverseer(Network);


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

//        public NetworkModel Network { get; set; }


        public void LoginEnded()
        {
            _context.TableOverseer.IndexAllFiles();
            _context.FileWatcher.Active = true;
        }
    }
}