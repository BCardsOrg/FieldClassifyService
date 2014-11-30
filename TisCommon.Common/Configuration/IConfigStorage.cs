using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Configuration
{

	[Guid("0E937679-400F-41da-88D9-E2500B3A0F7E")]	
	public interface ISubSectionsCollection
	{
		int		Count { get; }
		string	GetByIndex(int nIndex);   
	}

	[Guid("91FC633E-894D-48bf-9104-D03D01C1DDB8")]
	public interface IConfigSection
	{
		// Properties
		string Name { get; }		
		string InnerXml { get; set; }

		ISubSectionsCollection SubSections { get; }	

		// methods
		void SetParam(
						string sSubSectionName,
						string sParamName, 
						object oValue);

		object GetParam(
						string sSubSectionName,
						string sParamName);

		string[] GetParamsNames(string sSubSectionName);

		void	CreateSubSection(string sSubSectionName);	
		void	DeleteSubSection(string sSubSectionName);	


		void	StoreAsXml(object oObj);
		object	LoadFromXml();
	}
	
	[Guid("5988DF46-BA47-41d2-A629-2287A188FEBC")]
	public interface IConfigStorage
	{
		IConfigSection[] Sections { get; }

		IConfigSection GetSection(string sName);
		IConfigSection GetOrCreateSection(string sName);
		
		void SaveAllSections();

		void DeleteSection(string sName);

		//void SetDataPath(string sPathName);
		//string GetDataPath();
	}


}
