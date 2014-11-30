using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Security
{
	[ComVisible(false)]
	public class AttributeExplorer
	{
		public static string[] ObtainDeclaredPermissions (object oSecuredEntity)
		{
			string[] SupportedPermissions = EmptyArrays.StringArray;

			if (oSecuredEntity == null)
			{
				return SupportedPermissions;
			}

			Type oSecuredEntityType = null; 

			if (oSecuredEntity is ITisServiceInfo)
			{
				string sServiceImplType = (oSecuredEntity as ITisServiceInfo).ServiceImplType;

				if (sServiceImplType != String.Empty)
				{
					oSecuredEntityType = Type.GetType (sServiceImplType); 
				}
			}
			else
			{
				oSecuredEntityType = oSecuredEntity.GetType (); 
			}

			if (oSecuredEntityType == null)
			{
				return SupportedPermissions;
			}

			TisSupportedPermissionsAttribute oSupportedPermissionsAttribute =
                (TisSupportedPermissionsAttribute)ReflectionUtil.GetAttribute(
				                               oSecuredEntityType,
                                               typeof(TisSupportedPermissionsAttribute));

			if (oSupportedPermissionsAttribute == null)
			{
				return SupportedPermissions;
			}

			return oSupportedPermissionsAttribute.SupportedPermissions;
		}
	}

	[ComVisible(false)]
	public class SecurityAccessException
	{
		public static void Raise (string sSecuredEntityName, string sFailedPermissionName)
		{
			Exception oInnerException = new UnauthorizedAccessException ();

			if (sFailedPermissionName == String.Empty)
			{
  			    throw new TisException (oInnerException, "Entity {0} has no defined permissions", sSecuredEntityName);
			}
			else
			{
  			    throw new TisException (oInnerException, "Entity {0} failed to validate permission {1}", sSecuredEntityName, sFailedPermissionName);
			}
		}
	}
}
