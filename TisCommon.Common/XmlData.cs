using System;
using System.Xml;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon
{
	// Allows passing XML in string/DOM representation 
	// and delayed transformation (on demand).
	// Allows custom (possible more efficient) serialization when passed inter-process.
	public interface IXmlData: ISerializable
	{
		void InitFromString(string sXML);
		void InitFromDOM(XmlDocument Doc, bool bCopy);

		string		GetAsString();
		XmlDocument GetAsDOM();
	}

	public class XmlData: IXmlData
	{
		private string		m_sXML;
		private XmlDocument m_Doc; 

		public XmlData()
		{
		}

		public XmlData(string sXML)
		{
			InitFromString(sXML);
		}

		public XmlData(XmlDocument Doc, bool bCopy)
		{
			InitFromDOM(Doc, bCopy);
		}

		public void InitFromString(string sXML)
		{
			m_sXML = sXML;
			m_Doc  = null;
		}

		public void InitFromDOM(XmlDocument Doc, bool bCopy)
		{
			if(bCopy)
			{
				// Copy the provided document
				CopyIn(Doc);
			}
			else
			{
				// Attach
				m_Doc  = Doc;
			}
			
			// The string is not set until needed
			m_sXML = null;
		}

		public string GetAsString()
		{
			// If string representation not exists, create it
			if(m_sXML == null)
			{
				if(m_Doc != null)
				{
					CreateStringFromDOM();
				}
			}
			
			// Return the string representation if exists
			if(m_sXML != null)
			{
				return m_sXML;
			}
			
			// Return an empty string if the message is empty
			return "";
		}

		public XmlDocument GetAsDOM()
		{
			// If DOM representation not exists, create it
			if(m_Doc == null)
			{
				if(m_sXML != null)
				{
					CreateDOMFromString();
				}
			}
			
			if(m_Doc != null)
			{
				return m_Doc;
			}
			
			// Return a empty document if the message is empty
			return new XmlDocument();
		}

		public void GetObjectData(SerializationInfo Info, StreamingContext Context)
		{
			throw new TisException("Not implemented yet");
		}
		
		public static IXmlData FromFile(string sFileName)
		{
			return new XmlData(System.IO.File.OpenText(sFileName).ReadToEnd());
		}

		public static void ToFile(IXmlData Data, string sFileName)
		{
			System.IO.StreamWriter Writer = System.IO.File.CreateText(sFileName);
			Writer.Write(Data.GetAsString());
			Writer.Close();
		}

		// Creates DOM representation from string representation
		private void CreateDOMFromString()
		{
			XmlDocument Doc = new XmlDocument();

			Doc.LoadXml(m_sXML);
	
			m_Doc = Doc;
		}
		
		// Creates string representation from DOM representation
		private void CreateStringFromDOM()
		{
			m_sXML = m_Doc.InnerXml;
		}

		// Copy the provided document to the internal DOM representation
		private void CopyIn(XmlDocument Doc)
		{
			XmlDocument TmpDoc = new XmlDocument();
			TmpDoc.AppendChild(Doc.Clone());
			m_Doc = TmpDoc;
		}

	}
}
