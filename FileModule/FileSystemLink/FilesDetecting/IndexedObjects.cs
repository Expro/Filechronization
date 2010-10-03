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


        public IEnumerable<DescriptorPair> RelativeDescriptorPairs
        {
            get
            {
                return new Transformer(this);
            }
            
        }
        public class DescriptorPair
        {
            private FsObject<RelPath> _relToMainPath;
            private FsObject<RelPath> _relToRootDir;

            public DescriptorPair(FsObject<RelPath> relToMainPath, FsObject<RelPath> relToRootDir)
            {
                _relToMainPath = relToMainPath;
                _relToRootDir = relToRootDir;
            }

            public FsObject<RelPath> RelToMainPath
            {
                get { return _relToMainPath; }
            }

            public FsObject<RelPath> RelToRootDir
            {
                get { return _relToRootDir; }
            }
        }
        public class Transformer : IEnumerable<DescriptorPair>
        {
            private readonly IndexedObjects _indexed;

            public Transformer(IndexedObjects indexed)
            {
                _indexed = indexed;
            }

            public IEnumerator<DescriptorPair> GetEnumerator()
            {
                return new Transformerator(_indexed);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public struct Transformerator : IEnumerator<DescriptorPair>
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

            public DescriptorPair Current
            {
                get
                {
                    return new DescriptorPair(_valueEnumerator.Current.RelativeIn(_indexed.RootDir), _valueEnumerator.Current);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }


    }
}