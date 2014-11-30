using System;
using System.Xml;
using System.Runtime.Serialization;
using Microsoft.Win32;
using System.IO;
using System.Collections;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Configuration
{
	public abstract class BaseConfigStorage: IConfigStorage   
	{
		// XML Element and section names in the config file
		const string ELEMENT_CONFIG_STORAGE = "ConfigStorage";
		const string ELEMENT_SECTION = "SECTION";
		
		// attributes name and values
		const string ATTR_NAME = "name";
		const string ATTR_SERIALIZATION_TYPE = "serialize";
		const string ATTR_OBJECT_TYPE = "ObjectType";

		/// <summary>
		/// private members
		/// </summary>

		// map to hold all the section in the config file
		// <section name , IConfigSection>
		private Hashtable m_sections = null;


        private bool m_changed;
	
		//
		//	Public
		//


		public BaseConfigStorage()
		{
		}

		public IConfigSection[] Sections 
		{ 
			get
			{
				LoadDataIfRequired();

				IConfigSection[] configSectionArray = new IConfigSection [m_sections.Count];
				int index = 0;
				foreach(DictionaryEntry sectionEntry in m_sections)
				{
					configSectionArray[index] = (ConfigSectionImpl)sectionEntry.Value;
					index++;
				}
				return configSectionArray;
			}
		}

		/// <summary>
		/// return the specified config section object according to the
		/// section name
		/// </summary>
		/// <param name="sName"></param>
		/// <returns></returns>
		public IConfigSection GetSection(string sName)
		{
			LoadDataIfRequired();

			IConfigSection configSecObj = null;

			try
			{
				if (m_sections.Contains(sName))
				{
					configSecObj = (ConfigSectionImpl)m_sections[sName];
				}
			}
			catch(Exception e)
			{
				Log.Write(
					Log.Severity.ERROR,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					"failed  , Details :  {0}", e.Message); 

				throw new TiS.Core.TisCommon.TisException(
					e, 
					"failed , Details :  {0}", e.Message); 
			}
		
			return configSecObj;
		}
	
		/// <summary>
		/// return the specified config section object according to the
		/// section name , create if not exist
		/// </summary>
		/// <param name="sName"></param>
		/// <returns></returns>
		public IConfigSection GetOrCreateSection(string sName)
		{
			IConfigSection configSecObj = GetSection(sName);
			
			// create the section if not exist
			if (configSecObj == null)
			{
				configSecObj = CreateSectionInMemory(sName);			
			}

			return configSecObj;
		}

		public void DeleteSection(string sName)
		{
			LoadDataIfRequired();

			if (m_sections.Contains(sName))
			{
				// remove the section from the table
				m_sections.Remove(sName);

                m_changed = true;
			}
			else
			{
				throw new TisException("Section [{0}] does not exist", sName);
			}
		}

        public void RenameSection(string oldName, string newName)
        {
            LoadDataIfRequired();

            if (m_sections.Contains(oldName))
            {
                if (!m_sections.Contains(newName))
                {
                    ConfigSectionImpl configSection = (ConfigSectionImpl)m_sections[oldName];
                    configSection.Name = newName;
                    m_sections[newName] = configSection;
                    m_sections.Remove(oldName);

                    m_changed = true;
                }
                else
                {
                    throw new TisException("Section [{0}] already exists", newName);
                }
            }
            else
            {
                throw new TisException("Section [{0}] does not exist", oldName);
            }
        }

        /// <summary>
		/// Go through all the sections in memory and save them 
		/// to the config file .
		/// </summary>
		/// <returns></returns>
		public void SaveAllSections()
		{
			InitSectionsTableIfRequired();

			XmlDocument			xmlDoc = CreateConfigDocument();

			try
			{

				foreach(DictionaryEntry sectionEntry in m_sections)
				{
					ConfigSectionImpl configSection = 
						(ConfigSectionImpl)sectionEntry.Value;
			
					CreateSectionInDocument(
						configSection,
						xmlDoc);
				}
				
				// Prepare a formatted XML string
				StringWriter  oStringWriter = new StringWriter();
				XmlTextWriter oXmlWriter    = new XmlTextWriter(oStringWriter);
				oXmlWriter.Formatting       = Formatting.Indented;
				oXmlWriter.Indentation      = 3;

				xmlDoc.WriteTo(oXmlWriter);
				oXmlWriter.Flush();
				oStringWriter.Flush();
				
				string sXml = oStringWriter.GetStringBuilder().ToString();

				StoreConfigXML(sXml);

                m_changed = false;

			}
			catch(Exception e)
			{
				Log.Write(
					Log.Severity.ERROR,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					"failed  , Details :  {0}", e.Message); 

				throw new TiS.Core.TisCommon.TisException(
					e, 
					"failed , Details :  {0}", e.Message); 
			}
		}

        public virtual bool IsChanged
        {
            get
            {
                return m_changed;
            }
            set
            {
                m_changed = value;
            }
        }

        //
		//	Protected
		//

		protected abstract string LoadConfigXML();

		protected abstract void StoreConfigXML(string sXML);

		protected virtual bool NeedLoadData()
		{
			return m_sections == null;
		}


		protected void InvalidateData()
		{
			m_sections = null;
		}

		//
		//	Private
		//

		private void LoadDataIfRequired()
		{
			if(NeedLoadData())
			{
				LoadData();
			}
		}

		private void LoadData()
		{
			try
			{
				InitSectionsTable();

				// load all the config sections from the file
				LoadConfigSections();
			}
			catch(Exception oExc)
			{
				m_sections.Clear();

				Log.WriteException(oExc);

				throw;
			}
		}

		private void InitSectionsTable()
		{
			m_sections = new Hashtable();
		}

		private void InitSectionsTableIfRequired()
		{
			if(m_sections == null)
			{
				InitSectionsTable();
			}
		}
	
		/// <summary>
		/// Create empty config xml skelaton
		/// </summary>
		private XmlDocument CreateConfigDocument()
		{
			XmlDocument xmlDoc = new XmlDocument();

			try
			{
				// Write down the XML declaration
				XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0","utf-8",null); 
				XmlElement rootNode  = xmlDoc.CreateElement(ELEMENT_CONFIG_STORAGE);
				xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement); 
				xmlDoc.AppendChild(rootNode);					
			}
			catch(Exception e)
			{
				throw new TiS.Core.TisCommon.TisException(
					e, 
					"failed , Details :  {0}", e.Message);
			}

			return xmlDoc;
		}
		
		private XmlDocument LoadConfigDocument()
		{
			string sXML = LoadConfigXML();

			if( String.IsNullOrEmpty(sXML))
			{
				return null;
			}

			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.LoadXml(sXML);
			
			return xmlDoc;
		}


		// load all the config sections from the file
		// into m_sections table
		private void LoadConfigSections()
		{
			try
			{
                m_changed = false;

                // clear the table
				m_sections.Clear();

				// load the config file		
				XmlDocument xmlDoc = LoadConfigDocument();

				if(xmlDoc == null)
				{
					return;
				}

				string xPath = ELEMENT_SECTION;

				XmlNodeList sectionList =  xmlDoc.DocumentElement.SelectNodes(xPath);
				XmlNode		sectionNode;
				if (sectionList != null)
				{
					for (int i = 0; i < sectionList.Count; i++ )
					{
						sectionNode = sectionList.Item(i);
				
						if (sectionNode != null)
						{
							string serializeTypeName = "";
							string sectionName = "";
							string sInnerObjectType = string.Empty;
							ConfigSerializeType	serializeType = ConfigSerializeType.None;

							if (sectionNode.Attributes[ATTR_NAME] != null)
							{
								sectionName = sectionNode.Attributes[ATTR_NAME].Value.ToString();
							}
							else
							{
								throw new TisException("Load Config Sections failed , the Section is missing the 'name' attribute.");
							}

							if (sectionNode.Attributes[ATTR_OBJECT_TYPE] != null)
							{
								sInnerObjectType = sectionNode.Attributes[ATTR_OBJECT_TYPE].Value.ToString();
							}

							if (sectionNode.Attributes[ATTR_SERIALIZATION_TYPE] != null)
							{
								serializeTypeName = sectionNode.Attributes[ATTR_SERIALIZATION_TYPE].Value.ToString();
										
								object serializeTypeObj = Enum.Parse(typeof(ConfigSerializeType),serializeTypeName);
								serializeType = (ConfigSerializeType)serializeTypeObj;
							}
							else
							{
								throw new TisException("Load Config Sections failed , the Section {0} is missing the Attribute : {1} value",sectionName,ATTR_SERIALIZATION_TYPE);
							}

							ConfigSectionImpl configSection = new ConfigSectionImpl(
								sectionName,
								serializeType,
								sInnerObjectType,
								sectionNode.InnerXml);

							// build the sub sections if needed
							configSection.BuildSubSectionsElements(xmlDoc,true);

							// save the config section in the map
							m_sections[sectionName] = configSection;				
						}				
					}
				}			
			}
			catch(Exception e)
			{
				// write error to Log File.
				Log.Write(
					Log.Severity.ERROR,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					"failed  , Details :  {0}", e.Message); 

				throw new TiS.Core.TisCommon.TisException(
					e, 
					"failed  , Details :  {0}", 
					e.Message); 
			}
		}


		/// <summary>
		/// Create the config section in the memory map ( m_sections )
		/// </summary>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		private IConfigSection CreateSectionInMemory(string sectionName)
		{
			ConfigSectionImpl configSection = new ConfigSectionImpl(
				sectionName,
				ConfigSerializeType.None,
				String.Empty,
				string.Empty);		

			m_sections[sectionName] = configSection;

            m_changed = true;

			return configSection;
		}


		// create new section in the config file 
		private XmlNode CreateSectionInDocument(
			ConfigSectionImpl configSection ,
			XmlDocument		  xmlDoc)
		{
			XmlNode sectionElement = null;

			try
			{
				// create the section element with the specified attribute name = 'sectionName'
				// and save it in the config file
				sectionElement =  xmlDoc.CreateElement(ELEMENT_SECTION);
				
				// add the name attribute
				XmlAttribute nameAttribute = xmlDoc.CreateAttribute(ATTR_NAME);
				nameAttribute.Value = configSection.Name;
				sectionElement.Attributes.Append(nameAttribute);

				// add the serialize attribute 
				if (configSection.InnerObjectType != null)
				{
					XmlAttribute serializeAttribute = xmlDoc.CreateAttribute(ATTR_OBJECT_TYPE);
					serializeAttribute.Value = ReflectionUtil.GetFullTypeString(configSection.InnerObjectType);
					sectionElement.Attributes.Append(serializeAttribute);
				}

				// add the ObjectType attribute 
				XmlAttribute oObjectTypeAttribute = xmlDoc.CreateAttribute(ATTR_SERIALIZATION_TYPE);
				oObjectTypeAttribute.Value = Enum.GetName(typeof(ConfigSerializeType),configSection.SerializeType);
				sectionElement.Attributes.Append(oObjectTypeAttribute);

				// set the inner XML data for the section
				sectionElement.InnerXml = configSection.InnerXml;

				xmlDoc.DocumentElement.AppendChild(sectionElement);

				// Build the sub sections with all the parameters if needed
				// in the inner xml of the current config section
				configSection.BuildSubSectionsElements(xmlDoc,false);

                m_changed = true;
            }
			catch(Exception e)
			{
				Log.Write(
					Log.Severity.ERROR,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					"failed  , Details :  {0}", e.Message); 

				throw new TiS.Core.TisCommon.TisException(
					e, 
					"failed , Details :  {0}", e.Message); 
			}

			return sectionElement;
		}

	}
}
