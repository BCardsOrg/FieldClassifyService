using System;
using System.Xml;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Xml.Serialization;
using TiS.Core.TisCommon.Reflection;


namespace TiS.Core.TisCommon.Configuration
{
	/// <summary>
	/// Summary description for ConfigSectionImpl.
	/// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
	public class ConfigSectionImpl : IConfigSection
	{

		private const string XML_SERIALIZE_HEADER_TO_REMOVE = "<?xml version=\"1.0\"?>\r\n";


		// hold the type of serializer
		private ConfigSerializeType	 m_serializeType = ConfigSerializeType.None;
		
		private string	m_sectionName	= String.Empty;
		private string	m_innerXML		= String.Empty ;
		private string 	m_sObjType		= String.Empty;

		private IniXmlElements	m_oINISections = null;

		// indicates if Sub Sections was changed or not
		// use this to Build the sub sections node only if change was done.
		private bool			m_bSubSectionsChanged = false;


		/// <summary>
		/// The section in config file
		/// </summary>
		
		
		/// <summary>
		/// The section in config file
		/// </summary>
		/// <param name="sectionName">section name</param>
		/// <param name="serializeType">The serialization type of the section</param>
		/// <param name="innerXML">The inner xml of the section</param>
		public ConfigSectionImpl(
			string sectionName,
			ConfigSerializeType serializeType,
			string sInnerObjType,
			string innerXML)
		{
			m_sectionName = sectionName;

			// strore the serialize type
			m_serializeType = serializeType;

			m_sObjType = sInnerObjType;

			Init(innerXML);
		}
		
		#region properties

		public string Name 
		{
			get { return m_sectionName; }
            set { m_sectionName = value; }
		}

		public ConfigSerializeType SerializeType
		{
			get {return m_serializeType;}
			set {m_serializeType = value;}
		}

		/// <summary>
		/// return the Type of the object to do the serialization/deserialization
		/// </summary>
		public Type InnerObjectType
		{
			get 
			{
				if(!StringUtil.IsStringInitialized(m_sObjType))
				{
					return null;
				}

				return Type.GetType(m_sObjType, true);
			}

			set 
			{ 
				m_sObjType = ReflectionUtil.GetFullTypeString(value); 
			}
		}
		
		public string InnerXml 
		{
			get { return m_innerXML; }
			
			set 
			{
				if (value != "")
				{
					m_innerXML = value;
				}
				else
				{
					throw new  TisException("ConfigSectionImpl , error trying to assign empty InnerXml .");
				}
			}		
		}

		/// <summary>
		/// get collection of sub section names
		/// </summary>
		public ISubSectionsCollection SubSections 
		{ 
			get
			{
				if (m_oINISections != null)
				{
					return m_oINISections.SubSections;
				}

				return null;
			}
		}	


		/// <summary>
		/// Indicates if the serialized type for the config section already
		/// initialized
		/// </summary>
		public bool SerializedTypeInitialized
		{
			get
			{
				if (m_serializeType == ConfigSerializeType.None)
				{
					return false;
				}

				return true;
			}
		}

		#endregion


		private void Init(string innerXML)
		{
			m_innerXML = innerXML;	
			m_oINISections = new IniXmlElements(m_sectionName);
		}

		/// <summary>
		/// Build the sub sections with all the parameters if needed
		/// in the inner xml of the config section
		/// </summary>
		public void BuildSubSectionsElements(XmlDocument oXmlDoc,bool bInitSubSections)
		{
			if (bInitSubSections == true)
			{
				// initialize the sub sections ini elements object				
				m_oINISections.InitSubSections(oXmlDoc);
			}
			else if (m_bSubSectionsChanged == true)
			{
				m_oINISections.BuildSubSectionsElements(oXmlDoc,ref m_innerXML);	
				m_bSubSectionsChanged = false;
			}
		}

		public void SetParam(
			string sSubSectionName,
			string sParamName, 
			object oValue)
		{
			m_oINISections.SetParam(sSubSectionName,sParamName,oValue);

			m_bSubSectionsChanged = true;
		}

		public object GetParam(
			string sSubSectionName,
			string sParamName)
		{
			return m_oINISections.GetParam(sSubSectionName,sParamName);
		}

		public string[] GetParamsNames(string sSubSectionName)
		{
			return m_oINISections.GetParamsNames(sSubSectionName);
		}


		public void CreateSubSection(string sSubSectionName)
		{
			m_oINISections.CreateSubSection(sSubSectionName);
			m_bSubSectionsChanged = true;
		}


		public void DeleteSubSection(string sSubSectionName)
		{
			m_oINISections.RemoveSubSection(sSubSectionName);
			m_bSubSectionsChanged = true;
		}

		public void  StoreAsXml(object oObj)
		{
 
			try
			{
				// find the serialize attribute type from the input object
				ConfigSerializeAttribute serializeType = (ConfigSerializeAttribute)ReflectionUtil.GetAttribute(
														oObj.GetType(),
														typeof(ConfigSerializeAttribute));

				if (serializeType == null)
				{
					throw new TisException("Class {0} must have attribute {1} ",oObj.GetType(),typeof(ConfigSerializeAttribute));
				}

				m_serializeType = serializeType.SerializeType;

				// store the current type of the object
				InnerObjectType = oObj.GetType();

				switch(serializeType.SerializeType)
				{
					case ConfigSerializeType.SoapFormatter:
							DoSoapSerialization(oObj);
						break;

					case ConfigSerializeType.XmlSerializer:
							DoXmlSerialization(oObj);
						break;

					default:
					{
						throw new TisException("Serialization type {0} not supported . ",serializeType.SerializeType);
					}
				}
			}

			catch(Exception e)
			{
				// write error to Log File.
				Log.WriteError("ConfigSectionImpl.StoreAsXml failed to store as XML , Details :  {0}", e.Message); 
				throw new TiS.Core.TisCommon.TisException(
					e, 
					"ConfigSectionImpl.StoreAsXml failed to store as XML , Details :  {0}", e.Message); 

			}

		}

		/// <summary>
		/// Convert from byte array to the original string 
		/// </summary>
		/// <param name="characters"></param>
		/// <returns></returns>
		private string FromASCIIByteArray(byte[] characters)
		{
			ASCIIEncoding encoding = new ASCIIEncoding( );
			string constructedString = encoding.GetString(characters);

			return (constructedString);
		}

		private string FromUNICODEByteArray(byte[] characters)
		{
			UnicodeEncoding encoding = new UnicodeEncoding( );
			string constructedString = encoding.GetString(characters);

			return (constructedString);
		}


		/// <summary>
		/// do the soap serialization into the inner xml of the config section object
		/// </summary>
		/// <param name="oObj"></param>
		private void DoSoapSerialization(object oObj)
		{
			SoapFormatter soapFormatter = new SoapFormatter();			
			MemoryStream memStream = new MemoryStream();

			try 
			{
				soapFormatter.Serialize(memStream,oObj);			
				// convert the byte array from the memStream to string 
				// and store it in the InnerXML member				
				byte[] buf = memStream.GetBuffer();
				m_innerXML = FromASCIIByteArray(buf);
			}
			catch (SerializationException e) 
			{
				// write error to Log File.
				Log.WriteError("ConfigSectionImpl.DoSoapSerialize failed to Serialize config section , Details :  {0}", e.Message); 
				throw new TiS.Core.TisCommon.TisException(
					e, 
					"ConfigSectionImpl.DoSoapSerializefailed failed to Serialize config section , Details :  {0}", e.Message); 
			}
			finally 
			{
				memStream.Close();
			}
		}

		/// <summary>
		/// do the xml serialization into the inner xml of the config section object
		/// </summary>
		/// <param name="oObj"></param>
		private void DoXmlSerialization(object oObj)
		{
          //  TEMP TODO 
            //using (ReflectXmlSerializer oSerializer = new ReflectXmlSerializer())
            //{
            //    oSerializer.AddType(oObj.GetType());

            //    m_innerXML = oSerializer.Serialize(oObj);
            //}


		}

        public bool CanBeLoadedAsXml
        {
            get
            {
                switch (m_serializeType)
                {
                    case ConfigSerializeType.SoapFormatter:
                    case ConfigSerializeType.XmlSerializer:
                        return true;

                    default:
                        return false;
                }
            }
        }

		/// <summary>
		/// Do the deserialization of the inner xml data 
		/// and return the object
		/// </summary>
		/// <returns>Serialized object</returns>
		public object LoadFromXml()
		{
			if(!StringUtil.IsStringInitialized(m_innerXML))
			{
				return null;
			}		

			object oXmlDataObj = null;

			try
			{
				switch(m_serializeType)
				{
					case ConfigSerializeType.SoapFormatter:
						oXmlDataObj = DoSoapDeserialization(m_innerXML);
						break;

					case ConfigSerializeType.XmlSerializer:
						oXmlDataObj = DoXmlDeserialization(m_innerXML);
						break;
					
					default:
					{
						throw new TisException("Serialization type {0} not supported . ",m_serializeType);
					}
				}
			
			}
			catch(Exception e)
			{
				// write error to Log File.
				Log.WriteError("ConfigSectionImpl.LoadFromXml failed to load from XML , Details :  {0}", e.Message); 
//				throw new TiS.TisCommon.TisException(
//					e, 
//					"ConfigSectionImpl.LoadFromXml failed to load from XML , Details :  {0}", e.Message); 
			}


			return oXmlDataObj;
		}

		/// <summary>
		/// do the xml deserialization from the string
		/// </summary>
		/// <param name="strData">the string of data from the serialization operation</param>
		/// <returns>the object after the deserialize operation</returns>
		private object DoXmlDeserialization(string strData)
		{
			object oObject = null;

			if (InnerObjectType == null || !StringUtil.IsStringInitialized(m_innerXML))
				return oObject;

            //TEMP TODO
            //using (ReflectXmlSerializer oSerializer = new ReflectXmlSerializer())
            //{
            //    oSerializer.AddType(InnerObjectType);

            //    using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
            //    {
            //        return oSerializer.Deserialize(strData);
            //    }
            //}

            return null;
		}


		/// <summary>
		/// do the soap deserialization from the string
		/// </summary>
		/// <param name="strData">the string of data from the serialization operation</param>
		/// <returns>the object after the deserialization</returns>
		private object DoSoapDeserialization(string strData)
		{
			object oObject = null;
			MemoryStream memStream = null;

			if (!StringUtil.IsStringInitialized(m_innerXML))
				return oObject;

            using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
            {
                try
                {
                    SoapFormatter soapFormatter = new SoapFormatter();

                    byte[] buf = Encoding.ASCII.GetBytes(m_innerXML);
                    memStream = new MemoryStream(buf);

                    oObject = soapFormatter.Deserialize(memStream);
                }
                catch (SerializationException e)
                {
                    // write error to Log File.
                    Log.WriteError("ConfigSectionImpl.DoXmlDeserialization failed to do Xml Deserialization in config section , Details :  {0}", e.Message);
                    //				throw new TiS.TisCommon.TisException(
                    //					e, 
                    //					"ConfigSectionImpl.DoXmlDeserialization failed to do Xml Deserialization in config section , Details :  {0}", e.Message); 
                }
                finally
                {
                    memStream.Close();
                }
            }

			return oObject;
		}


	}
}
