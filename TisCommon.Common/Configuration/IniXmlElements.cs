using System;
using System.Xml;
using System.Runtime.Serialization;
using Microsoft.Win32;
using System.IO;
using System.Collections;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Configuration
{

	#region SubSection Data Structures

	public class SubSectionParameter
	{
		string m_sParamName;
		string m_sParamType;
		object m_oParamValue;		

		public SubSectionParameter(string sParamName,string sParamType,object oParamValue)
		{
			m_sParamName = sParamName;
			m_sParamType = sParamType;
			m_oParamValue = oParamValue;			
		}

		#region Properties

		public string ParamName
		{
			get { return m_sParamName; }
			set { m_sParamName = value;}
		}
		public string ParamType
		{
			get { return m_sParamType; }
			set { m_sParamType = value;}
		}
		public object ParamValue
		{
			get { return m_oParamValue; }
			set { m_oParamValue = value;}
		}
		#endregion
	}


	public class SubSection
	{
		// hold collection of parameters in the sub section
		private Hashtable	m_oParams;

		private string		m_sSubSectionName = ""; 		

		public SubSection(string sSubSectionName)
		{
			m_sSubSectionName = sSubSectionName;

			// create the hashtable with case insensitive
            //CaseInsensitiveHashCodeProvider oCaseInsHCProvider = new CaseInsensitiveHashCodeProvider();
            //CaseInsensitiveComparer oCaseInsComparer = new CaseInsensitiveComparer();
            //m_oParams = new Hashtable(oCaseInsHCProvider,oCaseInsComparer);

            // Changed after moving from .NET 1.1
            // TODO : check whether we should use InvariantCulture or CurrentCulture
            m_oParams = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

		}

		public string Name
		{
			get { return m_sSubSectionName; }
			set { m_sSubSectionName = value; }
		}

		/// <summary>
		/// Collection of all the parameters in sub section
		/// </summary>
		public SubSectionParameter[] Parameters
		{
			get
			{
				SubSectionParameter[] oParametersArray = new SubSectionParameter [m_oParams.Count];
				int index = 0;
				foreach(DictionaryEntry paramEntry in m_oParams)
				{
					oParametersArray[index] = (SubSectionParameter)paramEntry.Value;
					index++;
				}
				return oParametersArray;
			}
		}

		public void AddParam(string sParamName,string sParamType,object oParamValue)
		{
			SubSectionParameter oSubSectionParam = new SubSectionParameter(sParamName,
																			sParamType,
																			oParamValue);
			m_oParams[sParamName] = oSubSectionParam;
		}


		public void RemoveParam(string sParamName)
		{
			if (m_oParams.Contains(sParamName))
			{
				m_oParams.Remove(sParamName);
			}
		}

		public bool IsParamExist(string sParamName)
		{
			return m_oParams.Contains(sParamName);
		}

		public void SetParamValue(string sParamName,object oParamValue)
		{
			if (IsParamExist(sParamName))
			{
				((SubSectionParameter)m_oParams[sParamName]).ParamValue = oParamValue;
			}
		}

		public object GetParamValue(string sParamName)
		{
			object oValue = null;
			if (IsParamExist(sParamName))
			{
				oValue = ((SubSectionParameter)m_oParams[sParamName]).ParamValue;
			}
			return oValue;
		}
	}

	#endregion

	/// <summary>
	/// This class build xml elements like the ini data file
	/// format in the inner xml of Section in the config file.
	/// </summary>
	public class IniXmlElements
	{
		// Constants
		private const string INI_SECTION = "INISection";
		private const string PARAM_ELEMENT = "Param";
		private const string ATTR_NAME = "name";
		private const string ATTR_TYPE = "type";

		// The table holds all the sub sections in the inner xml
		// < sub section name , sub section Node >
		private Hashtable	m_oSubSections ;
		private string		m_sConfigSectionName;

		//	hold the sections to delete from config file
		private ArrayList	m_oSectionsToDelete = new ArrayList();

		// hold the sections to create in config file
		private ArrayList	m_oSectionsToCreate = new ArrayList();



		public IniXmlElements(string sConfigSectionName)
		{
			m_sConfigSectionName = sConfigSectionName;


			// create the hashtable with case insensitive

            // Changed after moving from .NET 1.1
            //CaseInsensitiveHashCodeProvider oCaseInsHCProvider = new CaseInsensitiveHashCodeProvider();
            //CaseInsensitiveComparer oCaseInsComparer = new CaseInsensitiveComparer();
            //m_oSubSections = new Hashtable(oCaseInsHCProvider,oCaseInsComparer);

           // TODO : check whether we should use InvariantCulture or CurrentCulture
            m_oSubSections = new Hashtable( StringComparer.InvariantCultureIgnoreCase );

		}

		public void InitSubSections(XmlDocument oXmlDoc)
		{
			try
			{
				XmlNodeList oSections = null;
			
				// select all the INISections under the config section name
				// and build the SubSections collection in memory

				string xPath = "SECTION" + "[@" + ATTR_NAME + " = \"" + m_sConfigSectionName + "\"]" + "/" + INI_SECTION;;				
				oSections =  oXmlDoc.DocumentElement.SelectNodes(xPath);					
				
				if (oSections != null && oSections.Count > 0)
				{
					m_oSubSections.Clear();

					string sSectionName = "";

					for (int i=0; i < oSections.Count; i++)
					{
						XmlNode oSectionNode =  oSections[i];
						if (oSectionNode.Attributes[ATTR_NAME] != null)
						{
							sSectionName = oSectionNode.Attributes[ATTR_NAME].Value.ToString();
					
							// build the SubSection object with parameters
							SubSection	oSubSection = new SubSection(sSectionName);
						
							if (oSectionNode.HasChildNodes)
							{
								string sParamName = "";
								string sParamType = "";
								object oParamValue = null;

								for (int j = 0; j < oSectionNode.ChildNodes.Count; j++)
								{
									XmlNode oParamNode = oSectionNode.ChildNodes[j];
									if (oParamNode.Attributes[ATTR_NAME] != null)
									{
										sParamName = oParamNode.Attributes[ATTR_NAME].Value.ToString();
									}
									if (oParamNode.Attributes[ATTR_TYPE] != null)
									{
										sParamType = oParamNode.Attributes[ATTR_TYPE].Value.ToString();
									}

									if (StringUtil.IsStringInitialized(sParamName) && 
										StringUtil.IsStringInitialized(sParamType))
									{
										Type oParamType = Type.GetType(sParamType);					
										oParamValue = Convert.ChangeType(oParamNode.InnerText.ToString(),oParamType);
									
										// add the parameter to the sub section
										oSubSection.AddParam(sParamName,sParamType,oParamValue);
									}
								}
							}

							// save the current sub section
							m_oSubSections[sSectionName] = oSubSection;
						}
					}
				}
			}		// of try

			catch(Exception e)
			{
				// write error to Log File.
				Log.WriteError("IniXmlElements.InitSubSections  initialization failed  , Details :  {0}", e.Message); 
				throw new TisException(
					e, 
					"IniXmlElements.InitSubSections initialization failed , Details :  {0}", e.Message);
			}
		}

		/// <summary>
		/// Add new sub section to the sub sections collection
		/// </summary>
		/// <param name="sSectionName"></param>
		public void CreateSubSection(string sSectionName)
		{
			if (!m_oSubSections.Contains(sSectionName))
			{
				SubSection	oSubSection = new SubSection(sSectionName);	
				m_oSubSections[sSectionName] = oSubSection;

				m_oSectionsToCreate.Add(sSectionName);
			}
		}

		public void RemoveSubSection(string sSectionName)
		{
			if (m_oSubSections.Contains(sSectionName))
			{
				m_oSubSections.Remove(sSectionName);

				m_oSectionsToDelete.Add(sSectionName);
			}
		}

		/// <summary>
		/// get collection of sub section names
		/// </summary>
		public ISubSectionsCollection SubSections
		{
			get
			{
				string [] oSubSectionNames = new string [m_oSubSections.Count];

				int index = 0;
				foreach(DictionaryEntry sectionEntry in m_oSubSections)
				{
					oSubSectionNames[index] = (string)sectionEntry.Key;
					index++;
				}

				return new SubSectionsCollection(oSubSectionNames);
			}
		}

		public bool IsSubSectionExist(string sSubSectionName)
		{
			return m_oSubSections.Contains(sSubSectionName);
		}

		/// <summary>
		/// Set the param value in the sub section
		/// The sub section will be created if not exist.
		/// </summary>
		/// <param name="sSubSectionName"></param>
		/// <param name="sParamName"></param>
		/// <param name="oValue"></param>
		public void SetParam(string sSubSectionName,
							string sParamName, 
							object oValue)
		{
			// if section not found create it.
			if (!IsSubSectionExist(sSubSectionName))
			{
				SubSection	oSubSection = new SubSection(sSubSectionName);	
				oSubSection.AddParam(sParamName,oValue.GetType().ToString(),oValue);
				
				m_oSubSections[sSubSectionName] = oSubSection;
			}
			else
			{
				SubSection oSection = (SubSection)m_oSubSections[sSubSectionName];
				
				// find if parameter exists set it's value				
				if (oSection.IsParamExist(sParamName))
				{
					oSection.SetParamValue(sParamName,oValue);
				}
				else // create the parameter and set it's value
				{
					oSection.AddParam(sParamName,oValue.GetType().ToString(),oValue);
				}
			}
		}


		/// <summary>
		/// Get param value according to sub section and param name
		/// </summary>
		/// <param name="sSubSectionName"></param>
		/// <param name="sParamName"></param>
		public object GetParam(	string sSubSectionName,
								string sParamName)							
		{
			object oValue = null;

			// check if section exist
			if (IsSubSectionExist(sSubSectionName))
			{
				SubSection oSection = (SubSection)m_oSubSections[sSubSectionName];				
				oValue = oSection.GetParamValue(sParamName);
			}

			return oValue;
		}

		public string[] GetParamsNames(string sSubSectionName)
		{
			if (!IsSubSectionExist(sSubSectionName))
			{
				return EmptyArrays.StringArray;
			}

			SubSection oSection = (SubSection)m_oSubSections[sSubSectionName];
			
			string[] ParamNames = new string[oSection.Parameters.Length];
			
			for(int i=0; i<ParamNames.Length; i++)
			{
				ParamNames[i] = oSection.Parameters[i].ParamName;
			}

			return ParamNames;
		}


		/// <summary>
		/// Set the param value in the sub section
		/// The sub section will be created if not exist.
		/// </summary>
		/// <param name="sInnerXML"></param>
		/// <param name="sSubSectionName"></param>
		/// <param name="sParamName"></param>
		/// <param name="oValue"></param>
		/// <returns>The update inner xml with the new sub section and param value</returns>
		private void BuildParam(XmlDocument xmlDoc,
									string sSubSectionName,
									string sParamName, 
									object oValue)
		{
			XmlNode oSectionElement = null;
			XmlNode oParamElement = null;

			// first find if section exist
			//			// example for xPath looking for element <INISection name="log" />  :
			//			"INISection[@name = "log"]

			string xPath = "SECTION" + "[@" + ATTR_NAME + " = \"" + m_sConfigSectionName + "\"]";				
			XmlNode oConfigSectionNode =  xmlDoc.DocumentElement.SelectSingleNode(xPath);					
				
			if (oConfigSectionNode != null)
			{			
				xPath = INI_SECTION + "[@" + ATTR_NAME + " = \"" + sSubSectionName + "\"]";						
				oSectionElement =  oConfigSectionNode.SelectSingleNode(xPath);					
			}

			// if section not found create it.
			if (oSectionElement == null && oConfigSectionNode != null)
			{
				oSectionElement =  xmlDoc.CreateElement(INI_SECTION);
				
				// add the name attribute of the sub section
				XmlAttribute nameAttribute = xmlDoc.CreateAttribute(ATTR_NAME);
				nameAttribute.Value = sSubSectionName;
				oSectionElement.Attributes.Append(nameAttribute);

				// add the param element and value under the sub section element
				AppendParamElement(xmlDoc,
									oSectionElement,
									sParamName,
									oValue);


				oConfigSectionNode.AppendChild(oSectionElement);
			}
			else if (oConfigSectionNode != null)
			{
				// find if param element exists				
				xPath = PARAM_ELEMENT + "[@" + ATTR_NAME + " = \"" + sParamName + "\"]";						
				oParamElement =  oSectionElement.SelectSingleNode(xPath);					

				if (oParamElement == null)
				{
					AppendParamElement(xmlDoc,
										oSectionElement,
										sParamName,
										oValue);
				}
				else // set param value
				{
					oParamElement.InnerText = oValue.ToString();
				}
			}
		}


		private void CreateSubSectionElement(XmlDocument xmlDoc,
			string sSubSectionName)
		{
			XmlNode oSectionElement = null;

			string xPath = "SECTION" + "[@" + ATTR_NAME + " = \"" + m_sConfigSectionName + "\"]";				
			XmlNode oConfigSectionNode =  xmlDoc.DocumentElement.SelectSingleNode(xPath);					
				
			if (oConfigSectionNode != null)
			{			
				xPath = INI_SECTION + "[@" + ATTR_NAME + " = \"" + sSubSectionName + "\"]";						
				oSectionElement =  oConfigSectionNode.SelectSingleNode(xPath);					
			}

			// if section not found create it.
			if (oSectionElement == null && oConfigSectionNode != null)
			{
				oSectionElement =  xmlDoc.CreateElement(INI_SECTION);
				
				// add the name attribute of the sub section
				XmlAttribute nameAttribute = xmlDoc.CreateAttribute(ATTR_NAME);
				nameAttribute.Value = sSubSectionName;
				oSectionElement.Attributes.Append(nameAttribute);

				oConfigSectionNode.AppendChild(oSectionElement);
			}
		}


		private void DeleteSubSectionElement(XmlDocument xmlDoc,
											string sSubSectionName)
		{
			XmlNode oSectionElement = null;

			string xPath = "SECTION" + "[@" + ATTR_NAME + " = \"" + m_sConfigSectionName + "\"]";				
			XmlNode oConfigSectionNode =  xmlDoc.DocumentElement.SelectSingleNode(xPath);					
				
			if (oConfigSectionNode != null)
			{			
				xPath = INI_SECTION + "[@" + ATTR_NAME + " = \"" + sSubSectionName + "\"]";						
				oSectionElement =  oConfigSectionNode.SelectSingleNode(xPath);					
			}

			// Delete the sub section
			
			if (oSectionElement != null && oConfigSectionNode != null)
			{
//				XmlNode oParentNode = oSectionElement.ParentNode;
//				oParentNode.RemoveChild(oSectionElement);

				oConfigSectionNode.RemoveChild(oSectionElement);
			}
		}


		/// <summary>
		/// Go through all the Sub Sections in memory and
		/// Build the sub sections with all the parameters 
		/// as childs elements of the inner xml .
		/// </summary>
		/// <param name="sInnerXML"></param>
		public void BuildSubSectionsElements(XmlDocument oXmlDoc,ref string sInnerXML)
		{
			int i = 0 ;

			// create new sub sections if needed
			for (i=0; i < m_oSectionsToCreate.Count; i++)
			{
				CreateSubSectionElement(oXmlDoc,m_oSectionsToCreate[i].ToString());
			}

			// delete the sub sections if needed
			for (i=0; i < m_oSectionsToDelete.Count; i++)
			{
				DeleteSubSectionElement(oXmlDoc,m_oSectionsToDelete[i].ToString());
			}

			// build all the parameters for the sub section
			SubSection oSection = null;
			foreach(DictionaryEntry sectionEntry in m_oSubSections)
			{
				oSection = (SubSection)sectionEntry.Value;
				
				// build the parameters of the section
				SubSectionParameter [] oParameters = oSection.Parameters;
				if (oParameters != null)
				{
					for(i = 0; i < oParameters.Length; i++)
					{
						SubSectionParameter oParam = (SubSectionParameter)oParameters.GetValue(i);
						if (oParam != null)
						{
							// build the parameter
							BuildParam(oXmlDoc,
										oSection.Name,
										oParam.ParamName,
										oParam.ParamValue);
						}
					}
				}
			}

			// find the ConfigSection node and update the inner xml
			
			string xPath = "SECTION" + "[@" + ATTR_NAME + " = \"" + m_sConfigSectionName + "\"]";				
			XmlNode oConfigSectionNode =  oXmlDoc.DocumentElement.SelectSingleNode(xPath);					
			if (oConfigSectionNode != null)
			{
				// update the inner xml of the parent Config section
				// with the changes of new sub sections
				sInnerXML = oConfigSectionNode.InnerXml.ToString();
			}
		}

		/// <summary>
		/// Add param
		/// </summary>
		/// <param name="oXmlDoc"></param>
		/// <param name="oSectionElement"></param>
		/// <param name="sParamName"></param>
		/// <param name="oValue"></param>
		/// <returns></returns>
		private XmlNode AppendParamElement(XmlDocument oXmlDoc,
										XmlNode oSectionElement,
										string sParamName,
										object oValue)
		{
			XmlNode oParamElement =  oXmlDoc.CreateElement(PARAM_ELEMENT);
			// add the name attribute of the param element
			XmlAttribute nameAttribute = oXmlDoc.CreateAttribute(ATTR_NAME);
			nameAttribute.Value = sParamName;
			oParamElement.Attributes.Append(nameAttribute);

			// save the param type
			XmlAttribute oTypeAttr = oXmlDoc.CreateAttribute(ATTR_TYPE);
			oTypeAttr.Value = ReflectionUtil.GetShortTypeString(oValue.GetType());
			oParamElement.Attributes.Append(oTypeAttr);

			oParamElement.InnerText = oValue.ToString();

			oSectionElement.AppendChild(oParamElement);

			return oParamElement;
		}


	}
}
