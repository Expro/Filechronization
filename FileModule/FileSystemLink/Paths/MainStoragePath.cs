// Author: Piotr Trzpil
namespace FileModule
{
    #region Usings

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion


    public class MainStoragePath : IAbsPath
    {
        private readonly AbsPath _mainPath;


        public MainStoragePath(AbsPath path)
        {
            _mainPath = (AbsPath) Regex.Replace(path, PathUtils.AnySlash + "$", "");
        }

        public string Get
        {
            get { return _mainPath; }
        }

//        public ICollection<Name> GetParentFolders(AbsPath path)
//        {
//            AbsPath parentPath = (AbsPath) Path.GetDirectoryName(path);
//            if (parentPath.Equals(_mainPath))
//            {
//                return new List<Name>();
//            }
//            RelPath relativeParent = parentPath.RelativeTo(_mainPath);
//            return Regex.Split(relativeParent, PathUtils.AnySlash).Select(name => (Name)name).ToList();
//        }
        /// <summary>
        ///   Zwraca nazwe bezposredniego podfolderu sciezki glownej, w ktorym znajduje sie dany plik
        /// </summary>
        /// <param name = "anyPath">sciezka do danego pliku</param>
        /// <returns>nazwa bezposredniego podfolderu</returns>
        public Name ExtractSubfolderName(RelPath anyPath)
        {
            // ze sciezki wzglednej usun pierwszy znaleziony slash i wszystkie znaki po nim
            return (Name) Regex.Replace(anyPath.ToString(), PathUtils.AnySlash + ".*", "");
        }

//        public RelPath CreateRelative(IPath anyPath)
//        {
            // usun sciezke glowna i slash
//            return (RelPath) Regex.Replace(anyPath.Get, Regex.Escape(_mainPath) + PathUtils.AnySlash, "");
//        }

        public AbsPath ToFull(RelPath relativePath)
        {
            return (AbsPath)Path.Combine(_mainPath.ToString(), relativePath.ToString());
        }

        public override string ToString()
        {
            return _mainPath;
        }
    }
}