namespace FileModule
{

    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    #endregion

    public static class PathUtils
    {
        public static readonly char Slash = Path.PathSeparator;//@"\";
        public const string AnySlash = @"[\\\/]";

        public static Name FileName(this IPath objPath)
        {
            return (Name)Path.GetFileName(objPath.ToString());
        }

        private static FsObject<Name> NewAsName<TPath>(this FsObject<TPath> fsObject) where TPath : IPath
        {
            Name name = fsObject.Path.FileName();
            if (fsObject is FsFile<TPath>)
            {
                var fsFile = fsObject as FsFile<TPath>;
                return new FsFile<Name>(name, fsFile.Size, fsFile.LastWrite);
            }
            return new FsFolder<Name>(name);
        }
        public static FsFolder<Name> NewAsName<TPath>(this FsFolder<TPath> dir) where TPath : IPath
        {
            return new FsFolder<Name>(dir.Path.FileName());
        }
        public static FsFile<Name> NewAsName<TPath>(this FsFile<TPath> fsFile) where TPath : IPath
        {
            return new FsFile<Name>(fsFile.Path.FileName(), fsFile.Size, fsFile.LastWrite);
        }

        #region WithNewPath

        public static FsFile<TPathResult> WithNewPath<TPath1, TPathResult>(this FsFile<TPath1> fsFile, TPathResult newPath)
            where TPath1 : IPath
            where TPathResult : IPath
        {

            return new FsFile<TPathResult>(newPath, fsFile.Size, fsFile.LastWrite);
        }

        public static FsFolder<TPathResult> WithNewPath<TPath1, TPathResult>(this FsFolder<TPath1> fsFolder, TPathResult newPath)
            where TPath1 : IPath
            where TPathResult : IPath
        {

            return new FsFolder<TPathResult>(newPath);
        }

        public static FsObject<TPathResult> WithNewPath<TPath1, TPathResult>(this FsObject<TPath1> fsObj, TPathResult newPath)
            where TPath1 : IPath
            where TPathResult : IPath
        {
            if (fsObj is FsFile<TPath1>)
            {
                return ((FsFile<TPath1>) fsObj).WithNewPath(newPath);
            }
            return ((FsFolder<TPath1>) fsObj).WithNewPath(newPath);
        }

        #endregion



//        public static FsObject<Name> ToName(this FsObject<IPath> fsObject, IAbsPath path)
//        {
//            return new Fs
//
        //        }


        #region RelativeIn

        /// <summary>
        /// Extends objPath making it relative to parentFolderPath
        /// </summary>
        /// <param name="objPath"></param>
        /// <param name="parentFolderPath"></param>
        /// <returns></returns>
        public static RelPath RelativeIn(this IRelPath objPath, RelPath parentFolderPath)
        {
            return (RelPath)Path.Combine(parentFolderPath.ToString(), objPath.ToString());
        }
        public static FsObject<RelPath> RelativeIn(this FsObject<RelPath> fsObject, RelPath path)
        {
            RelPath relPath = fsObject.Path.RelativeIn(path);
            return NewRelative(fsObject, relPath);

        }

        #endregion

        #region RelativeTo

        /// <summary>
        /// From objPath creates path relative to parentFolderPath.
        /// objPath must begin with parentFolderPath.
        /// </summary>
        /// <param name="objPath"></param>
        /// <param name="parentFolderPath"></param>
        /// <returns></returns>
        public static RelPath RelativeTo(this IAbsPath objPath, IAbsPath parentFolderPath)
        {
            return (RelPath)Regex.Replace(objPath.ToString(), Regex.Escape(parentFolderPath.ToString()) + AnySlash, "");
        }
        public static FsObject<RelPath> RelativeTo(this FsObject<AbsPath> fsObject, IAbsPath path) 
        {
            RelPath relPath = fsObject.Path.RelativeTo(path);
            return NewRelative(fsObject, relPath);
            
        }
        public static FsFile<RelPath> RelativeTo(this FsFile<AbsPath> fsFile, IAbsPath path)
        {
            RelPath relPath = fsFile.Path.RelativeTo(path);
            return new FsFile<RelPath>(relPath, fsFile.Size, fsFile.LastWrite);
        }

        #endregion





        private static FsObject<RelPath> NewRelative<TPath>(FsObject<TPath> fsObject, RelPath newPath) where TPath : IPath
        {
            if (fsObject is FsFile<TPath>)
            {
                var fsFile = fsObject as FsFile<TPath>;
                return new FsFile<RelPath>(newPath, fsFile.Size, fsFile.LastWrite);
            }
            return new FsFolder<RelPath>(newPath);
        }


        #region AbsoluteIn

        /// <summary>
        /// Creates absolute path combining parentFolderPath with objPath
        /// </summary>
        /// <param name="objPath"></param>
        /// <param name="parentFolderPath"></param>
        /// <returns></returns>
        public static AbsPath AbsoluteIn(this IRelPath objPath, IAbsPath parentFolderPath)
        {
            return (AbsPath) Path.Combine(parentFolderPath.ToString(), objPath.ToString());
        }
        /// <summary>
        /// Converts to FsObject[AbsPath] pointing to a location somewhere in given path
        /// </summary>
        /// <param name="fsObject"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FsObject<AbsPath> AbsoluteIn(this FsObject<RelPath> fsObject, IAbsPath path)
        {
            AbsPath absPath = fsObject.Path.AbsoluteIn(path);
            if (fsObject is FsFile<RelPath>)
            {
                var fsFile = fsObject as FsFile<RelPath>;
                return new FsFile<AbsPath>(absPath, fsFile.Size, fsFile.LastWrite);
            }
            else
            {
                return new FsFolder<AbsPath>(absPath);
            }

        }

        #endregion

        public static ICollection<Name> GetAncestorFolders(this RelPath path)
        {

            RelPath parentPath = (RelPath) Path.GetDirectoryName(path.ToString());
            if (parentPath.ToString()=="")
            {
                return new List<Name>();
            }
//            RelPath relativeParent = ((AbsPath) parentPath);
            return Regex.Split(parentPath.ToString(), AnySlash).Select(name => (Name)name).ToList();
        }
        public static ICollection<Name> SplitPath(this RelPath path)
        {
            //            RelPath relativeParent = ((AbsPath) parentPath);
            return Regex.Split(path.ToString(), AnySlash).Select(name => (Name)name).ToList();
        }
        public static ICollection<Name> GetParentFoldersUntil(this AbsPath path, AbsPath parentFolderPath)
        {
            AbsPath parentPath = (AbsPath)Path.GetDirectoryName(path);
            if (parentPath.Equals(parentFolderPath))
            {
                return new List<Name>();
            }
            RelPath relativeParent = parentPath.RelativeTo(parentFolderPath);
            return Regex.Split(relativeParent.ToString(), AnySlash).Select(name => (Name)name).ToList();
        }

        public static ICollection<RelPath> GetRelativeParentFolders(RelPath relPath)
        {
            RelPath parentPath = (RelPath)Path.GetDirectoryName(relPath.ToString());
            if (parentPath.ToString() == "")
            {
                return new List<RelPath>();
            }
            //            RelPath relativeParent = ((AbsPath) parentPath);
            List<string> list = Regex.Split(parentPath.ToString(), AnySlash).ToList();
            List<RelPath> accumulated = new List<RelPath>();

            string combined = "";
            foreach (string s in list)
            {
                combined = Path.Combine(combined, s);
                accumulated.Add((RelPath) combined);
            }
            accumulated.Reverse();
            return accumulated;

            // string combined = list.Aggregate("", Path.Combine);
            // return combined.Select(elem => (RelPath)elem).Reverse();
        }
    }
}