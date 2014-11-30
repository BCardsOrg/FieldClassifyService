using System;

namespace TiS.Core.TisCommon
{
	#region EmbedInheritedAttribute

	[AttributeUsage(AttributeTargets.Interface)]
	public class EmbedInheritedAttribute: Attribute
	{
		public EmbedInheritedAttribute()
		{
		}
	}

	#endregion

	#region EmbedAttributeAttribute

	[AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
	public class EmbedAttributeAttribute: Attribute
	{
		private string			m_sAttributeName;
		private NameValue[]		m_Arguments;

		public EmbedAttributeAttribute(
			string sAttributeDeclaration)
		{
			Parse(sAttributeDeclaration);
		}

		public string AttributeName
		{
			get
			{
				return m_sAttributeName;
			}
		}

		public NameValue[] Arguments
		{
			get
			{
				return m_Arguments;
			}
		}

		//
		//	Private 
		//

		// Parsed form: "SomeAttribute(ParamVal1, ParamName2=ParamVal2)"
		private void Parse(string sDeclaration)
		{
			string[] Tokens = sDeclaration.Split(new char[] { '(', ')', ',', ' ' } );

			if(Tokens.Length == 0)
			{
				throw new TisException("Invalid attrribute declaration [{0}]", sDeclaration);
			}

			// First token is name
			m_sAttributeName = Tokens[0];

			// Parse parameters
			
			ArrayBuilder oParamsArray = new ArrayBuilder();

			for(int i=1; i<Tokens.Length; i++)
			{
				string sParam = Tokens[i];

				string[] ParamTokens = sParam.Split(new char[] { '=' });
				
				string sName  = String.Empty;
				string sValue = String.Empty;

				if(ParamTokens.Length > 1)
				{
					// Named parameter, name and value provided
					sName  = ParamTokens[0];
					sValue = ParamTokens[1];
				}
				else
				{
					// Ordered parameter, only value provided
					sValue = ParamTokens[0];
				}

				if(StringUtil.IsStringInitialized(sValue))
				{
					NameValue oParam = new NameValue(sName, sValue);

					oParamsArray.Add(oParam);
				}

			}

			m_Arguments = (NameValue[])oParamsArray.GetArray(typeof(NameValue));
		}

		#region Inner NameValue class

		public class NameValue
		{
			private string m_sName;
			private string m_sValue;
			
			public NameValue(string sName, string sValue)
			{
				m_sName  = sName;
				m_sValue = sValue;
			}

			public string Name
			{
				get { return m_sName; }
			}

			public string Value
			{
				get { return m_sValue; }
			}
		}

		#endregion
	}

	#endregion 
}
