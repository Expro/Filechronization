/*
 *
 * User: Expro
 * Date: 2010-07-18
 * Time: 02:08
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CodeManagement
{
	#region Comment
	/// <summary>
	/// 	Provides all creation-independent informations about code.
	/// </summary>
	#endregion
	[Serializable]
	public class CodeDetails: IComparable<CodeDetails>
	{
		#region Fields
		private string file;
		private string className;
		private string name;
		private Author[] authors;
		private string[] descriptions;
		private string[] interfaces;
		private CodeVersion version;
		private bool isShared;
		private bool isControllable;
		private bool isModule;
		private string moduleCondition;
		#endregion
		
		#region Constructors
		public CodeDetails(string file, string className)
		{
			Contract.Requires(file != null);
			Contract.Requires(className != null);
			
			this.file = file;
			this.className = className;
		}
		#endregion
		
		#region Public Methods
		public int CompareTo(CodeDetails other)
		{
			Contract.Requires(other != null);
			
			if (other.Equals(this))
				return 0;
			else
				return ToString().CompareTo(other.ToString());
		}
		
		public override string ToString()
		{
			return string.Format("{0} ({1})", name, version);
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				if (file != null)
					hashCode += 1000000007 * file.GetHashCode();
				if (className != null)
					hashCode += 1000000009 * className.GetHashCode();
				if (name != null)
					hashCode += 1000000021 * name.GetHashCode();
				if (authors != null)
					hashCode += 1000000033 * authors.GetHashCode();
				if (descriptions != null)
					hashCode += 1000000087 * descriptions.GetHashCode();
				if (interfaces != null)
					hashCode += 1000000093 * interfaces.GetHashCode();
				if (version != null)
					hashCode += 1000000097 * version.GetHashCode();
				hashCode += 1000000103 * isShared.GetHashCode();
				hashCode += 1000000123 * isControllable.GetHashCode();
				hashCode += 1000000181 * isModule.GetHashCode();
			}
			
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			CodeDetails other = obj as CodeDetails;
			if (other == null)
				return false;
			
			return other.ClassName.Equals(ClassName) && other.Version.Equals(Version);
		}

		public static bool operator ==(CodeDetails lhs, CodeDetails rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CodeDetails lhs, CodeDetails rhs)
		{
			return !(lhs == rhs);
		}
		#endregion
		
		#region Properties
		public string ModuleCondition
		{
			get {return moduleCondition;}
			protected internal set {moduleCondition = value;}
		}
		
		public string ClassName
		{
			get {return className;}
			protected internal set {className = value;}
		}
		
		public string File
		{
			get {return file;}
			protected internal set {file = value;}
		}
		
		public bool IsControllable
		{
			get {return isControllable;}
			protected internal set {isControllable = value;}
		}
		
		public bool IsModule
		{
			get {return isModule;}
			protected internal set {isModule = value;}
		}
		
		public bool IsShared
		{
			get {return isShared;}
			protected internal set {isShared = value;}
		}
		
		public CodeVersion Version
		{
			get {return version;}
			protected internal set {version = value;}
		}
		
		public ICollection<string> Descriptions
		{
			get {return descriptions;}
			
			protected internal set
			{
				descriptions = new string[value.Count];
				value.CopyTo(descriptions, 0);
			}
		}
		
		public ICollection<Author> Authors
		{
			get {return authors;}
			protected internal set
			{
				authors = new Author[value.Count];
				value.CopyTo(authors, 0);
			}
		}
		
		public string Name
		{
			get {return name;}
			protected internal set {name = value;}
		}
		
		public ICollection<string> Interfaces
		{
			get {return interfaces;}
			protected internal set
			{
				interfaces = new string[value.Count];
				value.CopyTo(interfaces, 0);
			}
		}
		#endregion
	}
}
