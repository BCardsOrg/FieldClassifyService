using System;
using System.Xml;

namespace TiS.Core.TisCommon
{
	// Represents a hierarchical (tree) read-only data, navigable using XPath.
	public interface IHierarchicalData
	{
		// Strong-typed getters
		// "Mandatory" methods will throw exception if the specified value not exists
		string GetMandatoryString(string sXPath);
		string GetOptionalString(string sXPath, string sDefault);

		int    GetMandatoryInt(string sXPath);
		int    GetOptionalInt(string sXPath, int Default);

		short  GetMandatoryShort(string sXPath);
		short  GetOptionalShort(string sXPath, short Default);

		double GetMandatoryDouble(string sXPath);
		double GetOptionalDouble(string sXPath, double Default);

		decimal GetMandatoryDecimal(string sXPath);
		decimal GetOptionalDecimal(string sXPath, decimal Default);

		bool   GetMandatoryBool(string sXPath);
		bool   GetOptionalBool(string sXPath, bool Default);
		
		// General value getters
		object GetMandatoryValue(string sXPath, Type RetType);
		object GetOptionalValue(string sXPath, Type RetType, object Default);

		// Subnode getters
		IHierarchicalData   GetSubnode(string sXPath);
		IHierarchicalData[] GetSubnodes(string sXPath);
		
		// Throws exception if the path not exists
		IHierarchicalData   GetMandatorySubnode(string sXPath);
	}

	// Implementation of IHierarchicalData for XML DOM
	public class XmlHierarchicalData: IHierarchicalData
	{
		private XmlNode	m_BaseNode;

		// Cache the necessary Type objects
		static Type m_StringType  = Type.GetType("System.String");
		static Type m_IntType	  = Type.GetType("System.Int32");
		static Type m_ShortType   = Type.GetType("System.Int16");
		static Type m_BoolType	  = Type.GetType("System.Boolean");
		static Type m_DoubleType  = Type.GetType("System.Double");
		static Type m_DecimalType = Type.GetType("System.Decimal");

		// Public constructor
		public XmlHierarchicalData(IXmlData Data)
		{
			Init(Data.GetAsDOM());
		}

		// Private constructor (used by GetSubnode(s) methods)
		private XmlHierarchicalData(XmlNode BaseNode)
		{
			Init(BaseNode);
		}

		// Strong-typed getters:

		public string GetMandatoryString(string sXPath)
		{
			return (string)GetMandatoryValue(sXPath, m_StringType);
		}

		public string GetOptionalString(string sXPath, string sDefault)
		{
			return (string)GetOptionalValue(sXPath, m_StringType, sDefault);
		}

		public int GetMandatoryInt(string sXPath)
		{
			return (int)GetMandatoryValue(sXPath, m_IntType);
		}

		public int GetOptionalInt(string sXPath, int Default)
		{
			return (int)GetOptionalValue(sXPath, m_IntType, Default);
		}

		public short GetMandatoryShort(string sXPath)
		{
			return (short)GetMandatoryValue(sXPath, m_ShortType);
		}

		public short GetOptionalShort(string sXPath, short Default)
		{
			return (short)GetOptionalValue(sXPath, m_ShortType, Default);
		}

		public double GetMandatoryDouble(string sXPath)
		{
			return (double)GetMandatoryValue(sXPath, m_DoubleType);
		}

		public double GetOptionalDouble(string sXPath, double Default)
		{
			return (double)GetOptionalValue(sXPath, m_DoubleType, Default);
		}

		public decimal GetMandatoryDecimal(string sXPath)
		{
			return (decimal)GetMandatoryValue(sXPath, m_DecimalType);
		}

		public decimal GetOptionalDecimal(string sXPath, decimal Default)
		{
			return (decimal)GetOptionalValue(sXPath, m_DecimalType, Default);
		}

		public bool GetMandatoryBool(string sXPath)
		{
			return (bool)GetMandatoryValue(sXPath, m_BoolType);
		}

		public bool GetOptionalBool(string sXPath, bool Default)
		{
			return (bool)GetOptionalValue(sXPath, m_BoolType, Default);
		}
		
		// General value getters:

		public object GetOptionalValue(string sXPath, Type RetType, object Default)
		{
			object Result = Default;

			XmlNode Node = m_BaseNode.SelectSingleNode(sXPath);

			if(Node != null)
			{
				Result = Convert.ChangeType(Node.InnerText, RetType);
			}

			return Result;
		}

		public object GetMandatoryValue(string sXPath, Type RetType)
		{
			object Result = GetOptionalValue(sXPath, RetType, null);

			if(Result == null)
			{
				throw new TisException("No results for path [{0}]", sXPath);
			}

			return Result;
		}

		// Subnode getters:

		public IHierarchicalData GetSubnode(string sXPath)
		{
			XmlNode Node = m_BaseNode.SelectSingleNode(sXPath);

			if(Node != null)
			{
				return new XmlHierarchicalData(Node);
			}

			return null;
		}

		public IHierarchicalData GetMandatorySubnode(string sXPath)
		{
			IHierarchicalData Result = GetSubnode(sXPath);

			if(Result == null)
			{
				throw new TisException("node for path [{0}] not exist", sXPath);
			}

			return Result;
		}

		public IHierarchicalData[] GetSubnodes(string sXPath)
		{
			IHierarchicalData[] Subnodes = null;

			XmlNodeList Nodes = m_BaseNode.SelectNodes(sXPath);

			Subnodes = new IHierarchicalData[Nodes.Count];

			for(int i=0; i<Subnodes.Length; i++)
			{
				Subnodes[i] = new XmlHierarchicalData(Nodes[i]);
			}

			return Subnodes;
		}

		// Private

		private void Init(XmlNode BaseNode)
		{
			m_BaseNode = BaseNode;
		}

	}
}
