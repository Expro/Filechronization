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
    public class IndexedObjects : FileTree
    {
        /// <summary>
        /// Relative to MainStoragePath
        /// </summary>
      //  private RelPath _rootDir;


        public IndexedObjects(RelPath rootDir)
            : base(rootDir)
        {
          //  _rootDir = rootDir;
    
        }

        public RelPath RootDir
        {
            get { return (RelPath) base.RootPath; }
        }

//        public Dictionary<RelPath, FsObject<RelPath>> Index
//        {
//            get { return _index; }
//        }


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

        private class Transformer : IEnumerable<DescriptorPair>
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

        private struct Transformerator : IEnumerator<DescriptorPair>
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