using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Customizations
{
	[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[ComVisible(false)]
	public class TisSupportedEventsAttribute : Attribute
	{
		private Type[] m_DisabledTypes;

		public TisSupportedEventsAttribute()
		{
			m_DisabledTypes = new Type[] {};
		}

		public TisSupportedEventsAttribute(params Type[] DisabledTypes)
		{
			m_DisabledTypes = DisabledTypes;
		}

		public Type[] DisabledTypes
		{
			get
			{
				return m_DisabledTypes;
			}
		}

		public bool IsDisabled (Type oType)
		{
			return Array.IndexOf(m_DisabledTypes, oType) > -1 ? true : false;
		}
	}
}
