using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Security
{
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = true)]
	[ComVisible(false)]
	public class TisSupportedPermissionsAttribute : Attribute
	{
		private string[] m_SupportedPermissions;

		public TisSupportedPermissionsAttribute (string sSupportedPermission) : 
			this (new string [] {sSupportedPermission})
		{
		}

		public TisSupportedPermissionsAttribute (string[] SupportedPermissions)
		{
			m_SupportedPermissions = SupportedPermissions;
		}

		public string[] SupportedPermissions
		{
			get
			{
				return m_SupportedPermissions;
			}
		}
	}
}
