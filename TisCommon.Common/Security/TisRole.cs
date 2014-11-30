using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.Security
{
	public delegate void RoleSystemUserAddedDelegate (RoleSystemUserArgs e);
	public delegate void RoleNameChangedDelegate (RoleNameChangedArgs e);

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisRole : ITisRole, ITisSystemUsers, ITisRolePermissions, IDeserializationCallback
	{
		internal static readonly string BUILTIN_ADMINISTRATORS_ROLE = "Administrators";

		private const string BUILTIN_ADMINISTRATORS_ROLE_DESCRIPTION = "Builtin Administrators";

        [DataMember]
		private string m_sName;
        [DataMember]
		private TisDefinedPermissionsMngr m_oPermissionMngr;
        [DataMember]
		private TisSystemUsersMngr m_oSystemUserMngr;

		[IgnoreDataMember]
		private ObtainSupportedPermissionsDelegate  m_oSupportedPermissionsDelegate;


		public TisRole ()
		{
		}

		public TisRole (string sRoleName, ObtainSupportedPermissionsDelegate  oSupportedPermissionsDelegate) : this ()
		{
			m_oPermissionMngr = new TisDefinedPermissionsMngr ();
			m_oSystemUserMngr = new TisSystemUsersMngr ();

			m_oSystemUserMngr.OnSystemUserAdded += new SystemUserAddedDelegate (OnSystemUserAddedHandler);
			m_oSystemUserMngr.OnFindSystemUserInNetGroup += new SystemUserInNetGroupDelegate (OnFindSystemUserInNetGroupHandler);

			m_sName = sRoleName;
			m_oSupportedPermissionsDelegate = oSupportedPermissionsDelegate;

			if (IsBuiltinAdministrators)
			{
				Description = BUILTIN_ADMINISTRATORS_ROLE_DESCRIPTION; 
			}
		}

		internal ObtainSupportedPermissionsDelegate SupportedPermissionsDelegate
		{
			get
			{
				return m_oSupportedPermissionsDelegate;
			}
			set
			{
				m_oSupportedPermissionsDelegate = value;
			}
		}
 
		#region ITisRole Members

		public string Name
		{
			get
			{
				return m_sName;
			}
			set
			{
				string sOldName = m_sName;
 
				m_sName = value;

				if (m_sName != sOldName && OnRoleNameChanged != null)
				{
					OnRoleNameChanged (new RoleNameChangedArgs (this, sOldName));
				}
			}
		}

        [DataMember]
		public string Description {get; set;}

		public bool IsBuiltinAdministrators
		{
			get
			{
				return StringUtil.CompareIgnoreCase (Name, BUILTIN_ADMINISTRATORS_ROLE);
			}
		}

		public ITisSystemUsers SystemUsers 
		{
			get
			{
				return (this as ITisSystemUsers);
			}
		}

		public ITisRolePermissions Permissions 
		{
			get
			{
				return (this as ITisRolePermissions);
			}
		}

		#endregion

		#region ITisSystemUsers Members

		ITisSystemUser[] ITisSystemUsers.All
		{
			get
			{
                return m_oSystemUserMngr.SystemUsers.ToArray();
			}
		}

		int ITisSystemUsers.Count 
		{
			get
			{
				return (this as ITisSystemUsers).All.Length;
			}
		}

		ITisSystemUser[] ITisSystemUsers.AddUsers (string[] SystemUsersName)
		{
			return m_oSystemUserMngr.Add (SystemUsersName);
		}

		ITisSystemUser[] ITisSystemUsers.AddUsersByType (string[] SystemUsersName, 
			AUTHENTICATION_TYPE enAuthenticationType)
		{
			return m_oSystemUserMngr.Add (SystemUsersName, enAuthenticationType);
		}

		ITisSystemUser ITisSystemUsers.AddUser (string sSystemUserName)
		{
			ITisSystemUser[] SystemUsers = (this as ITisSystemUsers).AddUsers (new string[] {sSystemUserName});

			return SystemUsers [SystemUsers.Length - 1];
		}

		ITisSystemUser ITisSystemUsers.AddUserByType (string sSystemUserName, 
			AUTHENTICATION_TYPE enAuthenticationType)
		{
			ITisSystemUser[] SystemUsers =  (this as ITisSystemUsers).AddUsersByType (
				new string[] {sSystemUserName}, 
				enAuthenticationType);

			return SystemUsers [SystemUsers.Length - 1];
		}

		void ITisSystemUsers.Clear ()
		{
			m_oSystemUserMngr.Clear ();
		}

		void ITisSystemUsers.RemoveAllByType (AUTHENTICATION_TYPE enAuthenticationType)
		{
			m_oSystemUserMngr.RemoveAll (enAuthenticationType);
		}

		void ITisSystemUsers.RemoveUsers (ITisSystemUser[] SystemUsers)
		{
			m_oSystemUserMngr.Remove(SystemUsers);
		}

		void ITisSystemUsers.RemoveUsersByName (string[] SystemUsersName)
		{
			m_oSystemUserMngr.Remove (SystemUsersName);
		}

		void ITisSystemUsers.RemoveUsersByType (string[] SystemUsersName, 
			AUTHENTICATION_TYPE enAuthenticationType)
		{
			m_oSystemUserMngr.Remove (SystemUsersName, enAuthenticationType);
		}

		void ITisSystemUsers.RemoveUser (ITisSystemUser oSystemUsers)
		{
			(this as ITisSystemUsers).RemoveUsers (new ITisSystemUser[] {oSystemUsers});
		}

		void ITisSystemUsers.RemoveUserByName (string sSystemUserName)
		{
			(this as ITisSystemUsers).RemoveUsersByName (new string[] {sSystemUserName});
		}

		void ITisSystemUsers.RemoveUserByType (string sSystemUserName, 
			AUTHENTICATION_TYPE enAuthenticationType)
		{
			(this as ITisSystemUsers).RemoveUsersByType (new string[] {sSystemUserName}, enAuthenticationType);
		}

		public bool Contains (ITisSystemUser oSystemUser)
		{
			return m_oSystemUserMngr.Contains (oSystemUser);
		}

		public bool ContainsByName (string sSystemUserName)
		{
			return m_oSystemUserMngr.Contains (sSystemUserName);
		}

		public bool ContainsByType (string sSystemUserName, AUTHENTICATION_TYPE enAuthenticationType)
		{
			return m_oSystemUserMngr.Contains (sSystemUserName, enAuthenticationType);
		}

		#endregion

		internal bool ContainsCurrentSystemUser ()
		{
			return m_oSystemUserMngr.ContainsCurrentSystemUser ();
		}

		internal bool ContainsCurrentSystemGroup ()
		{
			return m_oSystemUserMngr.ContainsCurrentSystemGroup ();
		}

        internal ITisSystemUser AddCurrentUser()
        {
            return m_oSystemUserMngr.AddCurrentUser();
        }

        internal void RemoveCurrentUser()
        {
            m_oSystemUserMngr.RemoveCurrentUser();
        }

        internal event RoleSystemUserAddedDelegate OnRoleSystemUserAdded;

		internal event RoleNameChangedDelegate OnRoleNameChanged;

		internal event SystemUserInNetGroupDelegate OnFindSystemUserInNetGroup;


		#region ITisRolePermissions Members

		ITisDefinedPermissionsSet[] ITisRolePermissions.All
		{
			get
			{
				return m_oPermissionMngr.PermissionsSets.ToArray(); 
			}
		}

		int ITisRolePermissions.Count 
		{
			get
			{
				return (this as ITisRolePermissions).All.Length;
			}
		}

		public object[] AllSecuredEntititesInfo 
		{
			get
			{
				ArrayList oSecuredEntitites = new ArrayList ();

				foreach (TisPermissionsSet oPermissionSet in m_oPermissionMngr.PermissionsSets)
				{
					oSecuredEntitites.Add (oPermissionSet.PersistKey);
				}

				return (object[]) ArrayBuilder.CreateArray (
					oSecuredEntitites, 
					typeof (object), 
					new ArrayElementFilter (AllSecuredEntititesFilter));
			}
		}

		public ITisDefinedPermissionsSet ObtainPermissionsSet (object oSecuredEntity)
		{
			return m_oPermissionMngr.GetByPersistKey (oSecuredEntity);
		}

		public ITisDefinedPermissionsSet[] AllowAllSupportedForEntities (object[] SecuredEntities)
		{
			ArrayList oPermissionsSets = new ArrayList ();
			ITisSupportedPermissionsSet oSupportedPermissionsSet;

			foreach (object oSecuredEntity in SecuredEntities)
			{
				oSupportedPermissionsSet = m_oSupportedPermissionsDelegate (oSecuredEntity);

				oPermissionsSets.Add (
					m_oPermissionMngr.AddPermissionsSet (
					oSecuredEntity, 
					oSupportedPermissionsSet.AllPermissions.Names));
			}

			return (ITisDefinedPermissionsSet[]) ArrayBuilder.CreateArray (
				oPermissionsSets, 
				typeof (ITisDefinedPermissionsSet), 
				new ArrayElementFilter (DefinedPermissionsSetFilter));
		}

		public ITisDefinedPermissionsSet AllowAllSupportedForEntity (object oSecuredEntity)
		{
			ITisDefinedPermissionsSet[] PermissionsSets = 
				AllowAllSupportedForEntities (new object[] {oSecuredEntity});
  
			return PermissionsSets [PermissionsSets.Length - 1]; 
		}

		public ITisDefinedPermissionsSet AllowPermissions (object oSecuredEntity, string[] Permissions)
		{
			ArrayList oTemp = new ArrayList (Permissions);

			ITisSupportedPermissionsSet SupportedPermissionsSet = 
				m_oSupportedPermissionsDelegate (oSecuredEntity);

			if (SupportedPermissionsSet != null)
			{
				foreach (string sPermissionName in Permissions)
				{
					if (!SupportedPermissionsSet.ContainsPermission (sPermissionName))
					{
						oTemp.Remove (sPermissionName);
					}
				}

				return (ITisDefinedPermissionsSet) m_oPermissionMngr.AddPermissionsSet (
					oSecuredEntity, 
					(string[]) oTemp.ToArray (typeof (string)));
			}
			else
			{
				return null;
			}
		}

		public ITisDefinedPermissionsSet AllowPermission (object oSecuredEntity, string sPermission)
		{
			return AllowPermissions (oSecuredEntity, new string[] {sPermission});
		}

		void ITisRolePermissions.Clear ()
		{
			m_oPermissionMngr.Clear ();
		}

		public void DenyAllForEntities (object[] SecuredEntities)
		{
			m_oPermissionMngr.RemovePermissionsSets (SecuredEntities);
		}

		public void DenyAllForEntity (object oSecuredEntity)
		{
			DenyAllForEntities (new object[] {oSecuredEntity});
		}

		void ITisRolePermissions.RemoveBySets (ITisDefinedPermissionsSet[] PermissionsSets)
		{
			m_oPermissionMngr.RemovePermissionsSets (PermissionsSets);
		}

		void ITisRolePermissions.RemoveBySet (ITisDefinedPermissionsSet oPermissionsSet)
		{
			(this as ITisRolePermissions).RemoveBySets (
				new ITisDefinedPermissionsSet[] {oPermissionsSet});
		}

		public bool HasDefined (object oSecuredEntity)
		{
			return m_oPermissionMngr.ContainsPermissionsSet (oSecuredEntity);
		}

		internal void PrePersistKeyChange (object oSecuredEntity, string sNewPersistKey)
		{
			ITisDefinedPermissionsSet oPermissionsSet = ObtainPermissionsSet (oSecuredEntity);

			if (oPermissionsSet != null)
			{
				string sOldPersistKey = m_oPermissionMngr.GetPersistKey (oSecuredEntity);

				m_oPermissionMngr.ReplacePersistKey (sOldPersistKey, sNewPersistKey);
			}
		}

		internal void PostPersistKeyChange (object oSecuredEntity, string sOldPersistKey)
		{
			ITisDefinedPermissionsSet oPermissionsSet = ObtainPermissionsSet (sOldPersistKey);

			if (oPermissionsSet != null)
			{
				string sNewPersistKey = m_oPermissionMngr.GetPersistKey (oSecuredEntity);

				m_oPermissionMngr.ReplacePersistKey (sOldPersistKey, sNewPersistKey);
			}
		}

		#endregion

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
			m_oSystemUserMngr.OnSystemUserAdded += new SystemUserAddedDelegate (OnSystemUserAddedHandler);
			m_oSystemUserMngr.OnFindSystemUserInNetGroup += new SystemUserInNetGroupDelegate (OnFindSystemUserInNetGroupHandler);
		}

		#endregion

		private bool DefinedPermissionsSetFilter (object oElement)
		{
			return oElement is ITisDefinedPermissionsSet;
		}

		private bool AllSecuredEntititesFilter (object oElement)
		{
			return true;
		}

		private void OnSystemUserAddedHandler(SystemUserArgs e)
		{
			e.IsValid = false;

			if (OnRoleSystemUserAdded != null)
			{
				RoleSystemUserArgs oRoleSystemUserArgs  = new RoleSystemUserArgs (this, e.SystemUser);

				OnRoleSystemUserAdded (oRoleSystemUserArgs);

				e.IsValid = oRoleSystemUserArgs.IsValid; 
			}
		}

		private bool OnFindSystemUserInNetGroupHandler(SystemUserInNetGroupArgs e)
		{
			if (OnFindSystemUserInNetGroup != null)
			{
				return OnFindSystemUserInNetGroup (e);
			}
			else
			{
				return false;
			}
		}
	}

	[ComVisible(false)]
	public class RoleNameChangedArgs : EventArgs
	{
		private ITisRole m_oRole;
		private string   m_sOldName;

		public RoleNameChangedArgs (ITisRole oRole, string sOldName)
		{
			m_oRole    = oRole;
			m_sOldName = sOldName;
		}

		public ITisRole Role
		{
			get
			{
				return m_oRole;
			}
		}

		public string OldName
		{
			get
			{
				return m_sOldName;
			}
		}
	}

	[ComVisible(false)]
	public class RoleSystemUserArgs : SystemUserArgs
	{
		private ITisRole m_oRole;

		public RoleSystemUserArgs (ITisRole oRole, ITisSystemUser oSystemUser) : base (oSystemUser)
		{
			m_oRole = oRole; 
		}

		public ITisRole Role
		{
			get
			{
				return m_oRole;
			}
		}
	}
}
