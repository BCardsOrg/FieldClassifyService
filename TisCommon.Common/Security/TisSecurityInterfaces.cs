using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Transactions;

namespace TiS.Core.TisCommon.Security
{
    public enum AUTHENTICATION_TYPE { NTLM, KERBEROS, TIS, MICROSOFT_AUTHENTICATION_PACKAGE_V1_0, FORMS, NEGOTIATE };

    public enum TRANSACTION_TYPE { SECURED, INTERNAL };
    public enum BYPASS_MODE { ADD_ADMIN_USER, CLEAR_ALL_USERS };

    public delegate void SecurityDataDelegate(EventArgs e);
    public delegate void PermissionsSetDelegate(PermissionsSetArgs e);
    public delegate TisSupportedPermissionsSet ObtainSupportedPermissionsDelegate(object oSecuredEntity);

    public enum USER_TYPE { USER, GROUP };

    [Guid("93905133-BA51-487b-ADF7-2235A4B8ABD2")]
	public interface ITisServiceSecurityCheck
	{
		string [] AllowedPermissions {get;}
		int AllowedPermissionsCount {get;}
		bool HasPermission (string sPermissionName);
		bool HasPermissions (string[] PermissionsName, 
		               	     out string sFailedPermissionName);
		void ValidatePermission (string sPermissionName);
		void ValidatePermissions (string[] PermissionsName, out string sFailedPermissionName);
	}

	[Guid ("F02E3623-2AAD-4e3b-8323-890FBB4C5EFB")]
	public interface ITisSecurityCheck
	{
		string [] ObtainAllowedPermissions (object oSecuredEntity);
		int AllowedPermissionsCount (object oSecuredEntity);
		bool HasPermission (object oSecuredEntity, string sPermissionName);
		bool HasPermissions (object oSecuredEntity, 
			                 string[] PermissionsName, 
			                 out string sFailedPermissionName);
		void ValidatePermission (object oSecuredEntity, string sPermissionName);
		void ValidatePermissions (object oSecuredEntity, string[] PermissionsName, out string sFailedPermissionName);
	}

	[Guid ("4174A650-8CB5-4373-92BE-1F4AC7100FDA")]
	public interface ITisRoles
	{				 
		ITisRole [] All {get;}
		int Count {get;}
		ITisRole[] CurrentRoles {get;}
		ITisRole GetByName (string sRoleName);

		ITisRole[] AddRoles (string[] RolesName); 
		ITisRole AddRole (string RoleName); 

		void Clear ();

		void RemoveRoles (ITisRole[] Roles);
		void RemoveRolesByName (string[] RolesName);

		void RemoveRole (ITisRole oRole);
		void RemoveRoleByName (string sRoleName);

		bool Contains (ITisRole oRole);
		bool ContainsByName (string sRoleName);

		ITisRole[] ObtainAttachedToEntity (object oSecuredEntity);
		void RemoveAttachedToEntity (object oSecuredEntity);

		void PreEntityPersistKeyChange (object oSecuredEntity, string sNewPersistKey);
		void PostEntityPersistKeyChange (object oSecuredEntity, string sOldPersistKey);
	}

	[Guid ("B5E726C6-F5C8-4789-853E-AB754AAB6BE8")]
	public interface ITisSecurityMngr : IDisposable
	{
		ITisRoles Roles {get;}
		
		ITisSupportedPermissions SupportedPermissions {get;}

		ITransactable SecurityTransaction {get;}

		void Clear ();

		event SecurityDataDelegate OnSecurityDataLoaded;
		event SecurityDataDelegate OnSecurityDataSaved;

		void RefreshSecurityData ();
	}

	[Guid ("9CDAF12E-E288-47d7-A1FC-D8844D6E9D37")]
	public interface ITisSystemUsers
	{
		ITisSystemUser[] All {get;}

		int Count {get;}

		ITisSystemUser[] AddUsers (string[] SystemUsersName);
		ITisSystemUser[] AddUsersByType (string[] SystemUsersName, AUTHENTICATION_TYPE enAuthenticationType);

		ITisSystemUser AddUser (string sSystemUserName);
		ITisSystemUser AddUserByType (string sSystemUserName, AUTHENTICATION_TYPE enAuthenticationType);

		void Clear ();
		void RemoveAllByType (AUTHENTICATION_TYPE enAuthenticationType);

		void RemoveUsersByName (string[] SystemUsersName);
		void RemoveUsersByType (string[] SystemUsersName, AUTHENTICATION_TYPE enAuthenticationType);
		void RemoveUsers (ITisSystemUser[] SystemUsers);

		void RemoveUserByName (string sSystemUserName);
		void RemoveUserByType (string sSystemUserName, AUTHENTICATION_TYPE enAuthenticationType);
		void RemoveUser (ITisSystemUser oSystemUser);

		bool ContainsByName (string sSystemUserName); 
		bool ContainsByType (string sSystemUserName, AUTHENTICATION_TYPE enAuthenticationType); 
		bool Contains (ITisSystemUser oSystemUser);
	}

	[Guid ("B5AF0D1F-B1AC-49c0-84F0-D8509994F250")]
	public interface ITisSupportedPermissions
	{
		ITisSupportedPermissionsSet[] All {get;}
		int Count {get;}

		ITisSupportedPermissionsSet ObtainPermissionsSet (object oSecuredEntity);

		ITisSupportedPermissionsSet AddPermissions (object SecuredEntity, string[] Permissions);
		ITisSupportedPermissionsSet AddPermission (object SecuredEntity, string sPermission);

		void Clear ();

		void RemoveByEntities (object[] SecuredEntities);
		void RemoveBySets (ITisSupportedPermissionsSet[] PermissionsSets);

		void RemoveByEntity (object oSecuredEntity);
		void RemoveBySet (ITisSupportedPermissionsSet oPermissionsSet);

		bool HasDefined (object oSecuredEntity);
	}

	[Guid ("176898CD-D0EE-4147-AC86-893A229B3000")]
	public interface ITisRolePermissions
	{
		ITisDefinedPermissionsSet[] All {get;}
		int Count {get;}

		ITisDefinedPermissionsSet ObtainPermissionsSet (object oSecuredEntity);

		object[] AllSecuredEntititesInfo {get;}

		ITisDefinedPermissionsSet[] AllowAllSupportedForEntities (object[] SecuredEntities);
		ITisDefinedPermissionsSet AllowAllSupportedForEntity (object oSecuredEntity);

		ITisDefinedPermissionsSet AllowPermissions (object oSecuredEntity, string[] Permissions);
		ITisDefinedPermissionsSet AllowPermission (object oSecuredEntity, string sPermission);

		void DenyAllForEntities (object[] SecuredEntities);
		void DenyAllForEntity (object oSecuredEntity);

		bool HasDefined (object oSecuredEntity);

		void Clear ();

		void RemoveBySets (ITisDefinedPermissionsSet[] PermissionsSets);
		void RemoveBySet (ITisDefinedPermissionsSet oPermissionsSet);
	}

	[Guid ("27667F37-7223-482e-9A57-E9A2910241C5")]
	public interface ITisRole		
	{
		string Name {get; set;}
		string Description {get; set;}
  
		ITisSystemUsers SystemUsers {get;}
		ITisRolePermissions Permissions {get;}

		bool IsBuiltinAdministrators {get;}
	}

	[Guid ("A5A80A59-45A9-4b72-B117-0D4273645F25")]
	public interface ITisSystemUser
	{
		string Name {get;}
		AUTHENTICATION_TYPE AuthenticationType {get;}
		bool IsAuthenticated {get;}
	}

	[Guid ("A87BA712-A9BA-44de-87CC-69BD4B335055")]
	public interface ITisDefinedPermissionsSet
	{
		ITisWritablePermissions DefinedPermissions {get;}
		bool ContainsPermission (string sPermission);
	}

	[Guid ("D61F1B94-02FC-4558-B5D3-418019BD9F88")]
	public interface ITisSupportedPermissionsSet : ITisDefinedPermissionsSet
	{
		ITisReadOnlyPermissions AllPermissions {get;}
		ITisReadOnlyPermissions BuiltInPermissions {get;}
	}

	[Guid ("E9168B09-4DC9-4593-857B-313A60C625B4")]
	public interface ITisReadOnlyPermissions
	{
		string[] Names {get;}
		int Count {get;}
		bool Contains (string sPermission);
	}

	[Guid ("EBB15B49-D8EA-4b47-A763-76F1BC4B7A6A")]
	public interface ITisWritablePermissions : ITisReadOnlyPermissions
	{
		void AddPermissions (string[] Permissions);
		void AddPermission (string sPermission);
		void SetPermissions (string[] Permissions);
		void SetPermission (string sPermission);
		void RemovePermissions (string[] Permissions);
		void RemovePermission (string sPermission);
		void Clear ();
	}

    public delegate void SystemUserAddedDelegate(SystemUserArgs e);
    public delegate bool SystemUserInNetGroupDelegate(SystemUserInNetGroupArgs e);
}
