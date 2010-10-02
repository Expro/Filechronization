namespace FileModule
{

    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    #endregion
    /// <summary>
    /// Dictionary of indexed objects contained in rootDir.
    /// </summary>
    public class IndexedObjects
    {
        /// <summary>
        /// Relative to MainStoragePath
        /// </summary>
        private RelPath _rootDir;

        /// <summary>
        /// NOTICE: Here AbsPath represents path relative to rootDir.
        /// </summary>
        private readonly Dictionary<RelPath, FsObject<RelPath>> _index;

        public IndexedObjects(RelPath rootDir, Dictionary<RelPath, FsObject<RelPath>> index)
        {
            _rootDir = rootDir;
            _index = index;
        }

        public IndexedObjects(RelPath rootDir)
        {
            _rootDir = rootDir;
            _index = new Dictionary<RelPath, FsObject<RelPath>>();
        }

        public RelPath RootDir
        {
            get { return _rootDir; }
        }

        public Dictionary<RelPath, FsObject<RelPath>> Index
        {
            get { return _index; }
        }


        public IEnumerable<FsObject<RelPath>> ValuesRelativeToMainPath
        {
            get
            {
                return new Transformer(this);
            }
            
        }

        public class Transformer : IEnumerable<FsObject<RelPath>>
        {
            private readonly IndexedObjects _indexed;

            public Transformer(IndexedObjects indexed)
            {
                _indexed = indexed;
            }

            public IEnumerator<FsObject<RelPath>> GetEnumerator()
            {
                return new Transformerator(_indexed);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public struct Transformerator : IEnumerator<FsObject<RelPath>>
        {
            private readonly IndexedObjects _indexed;
            private IEnumerator<FsObject<RelPath>> _valueEnumerator;
            public Transformerator(IndexedObjects indexed)
            {
                _indexed = indexed;
                _valueEnumerator = _indexed.Index.Values.GetEnumerator();
                

            }

            public void Dispose()
            {
                _valueEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _valueEnumerator.MoveNext();
            }

            public void Reset()
            {
                _valueEnumerator.Reset();
            }

            public FsObject<RelPath> Current
            {
                get
                {
                    return _valueEnumerator.Current.RelativeIn(_indexed.RootDir);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }


    }
}