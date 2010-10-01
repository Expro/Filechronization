// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion


    public class MainStoragePath
    {
        public const string AnySlash = @"[\\\/]";


        private readonly AbsPath mainPath;


        public MainStoragePath(AbsPath path)
        {
            mainPath = (AbsPath) Regex.Replace(path, AnySlash + "$", "");
        }

        public AbsPath Get
        {
            get { return mainPath; }
        }

        public ICollection<Name> GetParentFolders(AbsPath path)
        {
            AbsPath parentPath = (AbsPath) Path.GetDirectoryName(path);
            if (parentPath.Equals(mainPath))
            {
                return new List<Name>();
            }
            RelPath relativeParent = CreateRelative(parentPath);
            return Regex.Split(relativeParent, AnySlash).Select(name => (Name)name).ToList();
        }
        /// <summary>
        ///   Zwraca nazwe bezposredniego podfolderu sciezki glownej, w ktorym znajduje sie dany plik
        /// </summary>
        /// <param name = "anyPath">sciezka do danego pliku</param>
        /// <returns>nazwa bezposredniego podfolderu</returns>
        public string ExtractSubfolderName(IPath anyPath)
        {
            // ze sciezki wzglednej usun pierwszy znaleziony slash i wszystkie znaki po nim
            return Regex.Replace(CreateRelative(anyPath), AnySlash + ".*", "");
        }

        public RelPath CreateRelative(IPath anyPath)
        {
            // usun sciezke glowna i slash
            return (RelPath) Regex.Replace(anyPath.Get, Regex.Escape(mainPath) + AnySlash, "");
        }

        public AbsPath ToFull(RelPath relativePath)
        {
            return (AbsPath) Path.Combine(mainPath, relativePath);
        }

        public override string ToString()
        {
            return mainPath;
        }
    }
}