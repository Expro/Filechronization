namespace FileModule
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    [Serializable]
    public class Name : IPath
    {
        private readonly string _name;

        public Name(string name)
        {
            if (String.IsNullOrEmpty(name) ||  Regex.IsMatch(name, MainStoragePath.AnySlash))
            {
                throw new ArgumentException("Path " + name + " is not filename.");
            }
            _name = name;
        }

        public string Get
        {
            get
            {
                return _name;
            }
        }
        public static implicit operator string(Name path)
        {
            return path.Get;
        }

        public static explicit operator Name(string name)
        {
            return new Name(name);
        }

        public override string ToString()
        {
            return Get;
        }

        public bool Equals(Name other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Name)) return false;
            return Equals((Name) obj);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}