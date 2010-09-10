namespace FileModule
{
    #region Usings

    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    #endregion

    public class MainStoragePath
    {
        private const string AnySlash = @"[\\\/]";


        private readonly string mainPath;


        public MainStoragePath(string path)
        {
            mainPath = Regex.Replace(path, AnySlash + "$", "");
        }

        public string Get
        {
            get { return mainPath; }
        }


        /// <summary>
        ///   Zwraca nazwe bezposredniego podfolderu sciezki glownej, w ktorym znajduje sie dany plik
        /// </summary>
        /// <param name = "fullPath">sciezka do danego pliku</param>
        /// <returns>nazwa bezposredniego podfolderu</returns>
        public string ExtractSubfolderName(string fullPath)
        {
            // ze sciezki wzglednej usun pierwszy znaleziony slash i wszystkie znaki po nim
            return Regex.Replace(CreateRelative(fullPath), AnySlash + ".*", "");
        }

        public string CreateRelative(string fullPath)
        {
            // usun sciezke glowna i slash
            return Regex.Replace(fullPath, Regex.Escape(mainPath) + AnySlash, "");
        }

        public string CreateFullPath(string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException("Path " + relativePath+" is not relative.");
            }

            return Path.Combine(mainPath, relativePath);
        }
    }
}