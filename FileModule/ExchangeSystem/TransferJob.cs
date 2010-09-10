namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///   Obiekt obslugujacy wymiane jednego pliku
    /// </summary>
    public class TransferJob
    {
        private readonly Dictionary<string, BitArray> bitArrays;
        private readonly FileDescriptor file;
        private readonly int pieceCount;

        private readonly List<PieceInfo> piecesIndexed;

        /// <summary>
        ///   Zawiera niesciagniete czesci, posortowane rosnaco wedlug liczby userow posiadajacych
        /// </summary>
        private readonly SortedSet<PieceInfo> piecesRaritySorted;

        private readonly Random random;

        private int completePieces;


        public TransferJob(FileDescriptor file, IList<PieceHash> hashes)
        {
            this.file = file;

            random = new Random();


            pieceCount = hashes.Count;

            bitArrays = new Dictionary<string, BitArray>();
            piecesRaritySorted = new SortedSet<PieceInfo>();
            piecesIndexed = new List<PieceInfo>(pieceCount);

            for (int i = 0; i < pieceCount; i++)
            {
                var pieceInfo = new PieceInfo(i, hashes[i]);
                piecesIndexed[i] = pieceInfo;
                piecesRaritySorted.Add(pieceInfo);
            }
        }

        public FileDescriptor File
        {
            get { return file; }
        }

        public bool Complete
        {
            get { return completePieces >= pieceCount; }
        }


        public bool CheckData(PieceID pieceId, byte[] data, int count)
        {
            try
            {
                return piecesIndexed[pieceId.Index].PieceHash.Equals(new PieceHash(data, count));
                //// /return hashes[pieceId].Equals(new PieceHash(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        ///   Losuje jedna czesc sposrod kilku najrzadszych
        ///   Nastepnie losuje usera sposrod posiadajacych ta czesc
        /// </summary>
        /// <returns>Para user i indeks czesci</returns>
        public Tuple<PieceID, int> GetNonActivePiece()
        {
            var selectedPieces = new List<PieceInfo>();
            const int sampleSize = 10;
            int counter = 0;

            foreach (PieceInfo piece in piecesRaritySorted)
            {
                selectedPieces.Add(piece);
                counter++;


                if (counter == sampleSize)
                {
                    break;
                }
            }

            PieceInfo selected = selectedPieces[random.Next(selectedPieces.Count)];

            int pieceSize = ExchUtils.PieceSize(file.Size, selected.Index);


            piecesRaritySorted.Remove(selected);
            piecesIndexed[selected.Index].Status = PieceStatus.Active;


            return new Tuple<PieceID, int>(new PieceID(file.RelativePath, selected.Index), pieceSize);
        }

        public void PieceComplete(PieceID piece)
        {
            piecesIndexed[piece.Index].Status = PieceStatus.Complete;
            completePieces++;
        }


        public void AddUser(String user)
        {
            bitArrays.Add(user, new BitArray(pieceCount));
        }

        public void RemoveUser(String user)
        {
            BitArray bits = bitArrays[user];
            foreach (PieceInfo piece in piecesRaritySorted)
            {
                if (bits.Get(piece.Index))
                {
                    piece.Count--;
                }
            }
        }

        public void UserHas(String user, int pieceIndex)
        {
            BitArray bits = bitArrays[user];
            if (!bits.Get(pieceIndex))
            {
                bits.Set(pieceIndex, true);
                piecesIndexed[pieceIndex].Count++;
            }
        }


        public String SelectUser(int pieceIndex)
        {
            string[] userList = (from pair in bitArrays
                                 where pair.Value.Get(pieceIndex)
                                 select pair.Key).ToArray();
            return userList.ElementAt(random.Next(userList.Length));
        }


//        public UserIndexPair GetRareRandom()
//        {
//            var selectedPieces = new List<PieceInfo>();
//            const int sampleSize = 10;
//            int counter = 0;
//
//            foreach (var piece in piecesRaritySorted)
//            {
//                
//                
//                selectedPieces.Add(piece);
//                counter++;
//                
//
//                if (counter == sampleSize)
//                {
//                    break;
//                }
//            }
//
//            PieceInfo selected = selectedPieces.ElementAt(random.Next(selectedPieces.Count));
//            
//
        //var userList = (from pair in bitArrays where pair.Value.Get(selected.Index) select pair.Key).ToArray();
        //  String user = userList.ElementAt(random.Next(userList.Length));
//
//            return new UserIndexPair(user, selected.Index);
//        }

        #region Nested type: PieceInfo

        private class PieceInfo : IComparable<PieceInfo>
        {
            private readonly int index;
            private readonly PieceHash pieceHash;

            public PieceInfo(int index, PieceHash pieceHash)
            {
                this.index = index;
                this.pieceHash = pieceHash;
            }

            public PieceStatus Status { get; set; }


            public PieceHash PieceHash
            {
                get { return pieceHash; }
            }

            public int Count { get; set; }


            public int Index
            {
                get { return index; }
            }

            #region IComparable<PieceInfo> Members

            public int CompareTo(PieceInfo other)
            {
                if (Count < other.Count)
                    return -1;
                if (Count > other.Count)
                    return 1;
                return 0;
            }

            #endregion
        }

        #endregion

        #region Nested type: PieceStatus

        private enum PieceStatus
        {
            Available,
            Active,
            Complete,
        }

        #endregion
    }
}