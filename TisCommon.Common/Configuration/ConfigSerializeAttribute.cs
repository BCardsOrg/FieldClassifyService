using System;

namespace TiS.Core.TisCommon.Configuration
{
	public enum ConfigSerializeType
	{
		None , SoapFormatter , XmlSerializer, SmartSerializer
	}

	public class ConfigSerializeAttribute: Attribute
	{
		private ConfigSerializeType m_enType;

		public ConfigSerializeAttribute(ConfigSerializeType enType)
		{
			m_enType = enType;
		}

		public ConfigSerializeType SerializeType
		{
			get { return m_enType; }
		}
	}
}
