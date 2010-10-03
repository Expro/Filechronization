namespace FileModule
{
    using System.Threading;

    internal class TimedFolderDeletion : TimedAction
    {
        private IndexedObjects _indexedContent;

        public TimedFolderDeletion(FsObject<RelPath> descriptor, TimerCallback callback, IndexedObjects indexedContent)
            : base(descriptor, callback, ChangeWatcher.DeleteWaitTime, Timeout.Infinite)
        {
            _indexedContent = indexedContent;
        }

        public IndexedObjects IndexedContent
        {
            get { return _indexedContent; }
        }
    }
}