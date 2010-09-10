/*
 *
 * User: Expro
 * Date: 2010-07-21
 * Time: 17:48
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using CodeManagement.Definitions;
using System.Diagnostics.Contracts;

namespace CodeManagement
{
	public class CodeAnalizer: MarshalByRefObject
	{
		#region Fields
		private CodeDetails[] codes;
		
		private string path;
		#endregion
		
		#region Private Methods
		private CodeDetails RetriveDetails(Type type)
		{
			Contract.Requires(type != null);
			Contract.Ensures(Contract.Result<CodeDetails>() != null);
			
			CodeDetails result = new CodeDetails(path, type.FullName);
			object[] attributes = type.GetCustomAttributes(true);
			Type[] allInterfaces = type.GetInterfaces();
			ISet<Author> authors = new HashSet<Author>();
			ISet<string> description = new HashSet<string>();
			ISet<string> interfaces = new HashSet<string>();

			if (typeof(IControllableCode).IsAssignableFrom(type))
				result.IsControllable = true;
			
			if (typeof(SharedCode).IsAssignableFrom(type))
				result.IsShared = true;
			
			if (typeof(MarshalByRefObject).IsAssignableFrom(type))
				result.IsShared = true;
			
			foreach (object attribute in attributes)
			{
				if (attribute is NameAttribute)
					result.Name = (attribute as NameAttribute).Value;
				
				if (attribute is DescriptionAttribute)
					description.Add((attribute as DescriptionAttribute).Text);
				
				if (attribute is AuthorAttribute)
				{
					var authorAttribute = (AuthorAttribute)attribute;
					authors.Add(new Author(authorAttribute.Name, authorAttribute.Email));
				}
				
				if (attribute is VersionAttribute)
				{
					var versionAttribute = (VersionAttribute)attribute;
					result.Version = new CodeVersion(versionAttribute.Major, versionAttribute.Minor, versionAttribute.Build);
				}
				
				if (attribute is ModuleAttribute)
				{
					result.IsModule = true;
					result.ModuleCondition = (attribute as ModuleAttribute).Condition;
				}
			}
			
			foreach (Type iface in allInterfaces)
				interfaces.Add(iface.FullName);
			
			var authorsArray = new Author[authors.Count];
			authors.CopyTo(authorsArray, 0);
			result.Authors = authorsArray;
			
			var descriptionArray = new string[description.Count];
			description.CopyTo(descriptionArray, 0);
			result.Descriptions = descriptionArray;
			
			var interfacesArray = new string[interfaces.Count];
			interfaces.CopyTo(interfacesArray, 0);
			result.Interfaces = interfacesArray;
			
			return result;
		}
		#endregion
		
		#region Constructors
		public CodeAnalizer()
		{
			this.path = "";
		}
		#endregion
		
		#region Public Methods
		public void Analize()
		{
			Assembly assembly;
			Type[] types;
			ISet<CodeDetails> allCodes = new HashSet<CodeDetails>();
			
			assembly = Assembly.LoadFrom(path);
			types = assembly.GetExportedTypes();

			foreach (Type type in types)
			{
				if (type.IsClass && !type.IsInterface && !type.IsAbstract && !type.IsGenericType)
				{	
					if (typeof(ICode).IsAssignableFrom(type))
					    allCodes.Add(RetriveDetails(type));
				}
			}
			
			codes = new CodeDetails[allCodes.Count];
			allCodes.CopyTo(codes, 0);
		}
		#endregion
		
		#region Properties
		public string Path
		{
			get {return path;}
			set {path = value;}
		}
		
		public ICollection<CodeDetails> Codes
		{
			get {return codes;}
		}
		#endregion
	}
}
