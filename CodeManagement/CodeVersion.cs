/*
 *
 * User: Expro
 * Date: 2010-07-29
 * Time: 12:47
 * 
 * 
 */
using System;
using System.Diagnostics.Contracts;

namespace CodeManagement
{
	[Serializable]
	public class CodeVersion
	{
		#region Fields
		private int major;
		private int minor;
		private int build;
		#endregion
		
		#region Constructors
		public CodeVersion(int major, int minor, int build)
		{
			Contract.Requires(major >= 0);
			Contract.Requires(minor >= 0);
			Contract.Requires(build >= 0);
			Contract.Requires((major + minor + build) > 0);
			
			this.major = major;
			this.minor = minor;
			this.build = build;
		}
		#endregion
		
		#region Public Methods
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", major.ToString(), minor.ToString(), build.ToString());
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				hashCode += 1000000007 * major.GetHashCode();
				hashCode += 1000000009 * minor.GetHashCode();
				hashCode += 1000000021 * build.GetHashCode();
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			CodeVersion other = obj as CodeVersion;
			if (other == null)
				return false;
			return this.major == other.major && this.minor == other.minor && this.build == other.build;
		}

		public static bool operator ==(CodeVersion lhs, CodeVersion rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CodeVersion lhs, CodeVersion rhs)
		{
			return !(lhs == rhs);
		}
		#endregion
		
		#region Properties
		public int Major
		{
			get {return major;}
		}
		
		public int Minor
		{
			get {return minor;}
		}
		
		public int Build
		{
			get {return build;}
		}
		#endregion
	}
}
