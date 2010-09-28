namespace FileModule.ExchangeSystem
{
    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///   Information about transfer of one file.
    /// </summary>
    public class TransferJob
    {
        private readonly Dictionary<UserID, BitArray> bitArrays;
        private readonly FsFile<RelPath> file;
        private readonly int pieceCount;

        private readonly List<PieceProperties> piecesIndexed;

        /// <summary>
        ///   Zawiera niesciagniete czesci, posortowane rosnaco wedlug liczby userow posiadajacych
        /// </summary>
        private readonly SortedSet<PieceProperties> piecesRaritySorted;

        private readonly Random random;

        private int completePieces;


        public TransferJob(FsFile<RelPath> file, IList<PieceHash> hashes)
        {
            this.file = file;

            random = new Random();


            pieceCount = hashes.Count;

            bitArrays = new Dictionary<UserID, BitArray>();
            piecesRaritySorted = new SortedSet<PieceProperties>();
            piecesIndexed = new List<PieceProperties>(pieceCount);

            for (int i = 0; i < pieceCount; i++)
            {
                var pieceInfo = new PieceProperties(i, hashes[i]);
                piecesIndexed[i] = pieceInfo;
                piecesRaritySorted.Add(pieceInfo);
            }
        }

        public FsFile<RelPath> File
        {
            get { return file; }
        }

        public bool Complete
        {
            get { return completePieces >= pieceCount; }
        }


        public bool CheckData(PieceInfo pieceInfo, byte[] data, int count)
        {
            try
            {
                return piecesIndexed[pieceInfo.Index].PieceHash.Equals(new PieceHash(data, count));
                //// /return hashes[PieceInfo].Equals(new PieceHash(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        ///   Chooses piece according to rarity
        /// </summary>
        /// <returns>New active piece</returns>
        public ActivePiece GetNonActivePiece()
        {
            
            var selectedPieces = new List<PieceProperties>();
            const int sampleSize = 10;
            int counter = 0;

            foreach (PieceProperties piece in piecesRaritySorted)
            {
                selectedPieces.Add(piece);
                counter++;


                if (counter == sampleSize)
                {
                    break;
                }
            }

            PieceProperties selected = selectedPieces[random.Next(selectedPieces.Count)];

            int pieceSize = ExchUtils.PieceSize(file.Size, selected.Index);


            piecesRaritySorted.Remove(selected);
            piecesIndexed[selected.Index].Status = PieceStatus.Active;

            throw new NotImplementedException();
            //TODO: return new ActivePiece(new PieceInfo(file.Path, selected.Index), pieceSize);
        }

        public void PieceComplete(PieceInfo piece)
        {
            piecesIndexed[piece.Index].Status = PieceStatus.Complete;
            completePieces++;
        }


        public void AddUser(UserID user)
        {
            bitArrays.Add(user, new BitArray(pieceCount));
        }

        public void RemoveUser(UserID user)
        {
            BitArray bits = bitArrays[user];
            foreach (PieceProperties piece in piecesRaritySorted)
            {
                if (bits.Get(piece.Index))
                {
                    piece.Count--;
                }
            }
        }

        public void UserHas(UserID user, int pieceIndex)
        {
            BitArray bits = bitArrays[user];
            if (!bits.Get(pieceIndex))
            {
                bits.Set(pieceIndex, true);
                piecesIndexed[pieceIndex].Count++;
            }
        }


        public UserID SelectUser(int pieceIndex)
        {
            UserID[] userList = (from pair in bitArrays
                                 where pair.Value.Get(pieceIndex)
                                 select pair.Key).ToArray();
            return userList.ElementAt(random.Next(userList.Length));
        }


//        public UserIndexPair GetRareRandom()
//        {
//            var selectedPieces = new List<PieceProperties>();
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
//            PieceProperties selected = selectedPieces.ElementAt(random.Next(selectedPieces.Count));
//            
//
        //var userList = (from pair in bitArrays where pair.Value.Get(selected.Index) select pair.Key).ToArray();
        //  String user = userList.ElementAt(random.Next(userList.Length));
//
//            return new UserIndexPair(user, selected.Index);
//        }

        #region Nested type: PieceProperties

        private class PieceProperties : IComparable<PieceProperties>
        {
            private readonly int index;
            private readonly PieceHash pieceHash;

            public PieceProperties(int index, PieceHash pieceHash)
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

            #region IComparable<PieceProperties> Members

            public int CompareTo(PieceProperties other)
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