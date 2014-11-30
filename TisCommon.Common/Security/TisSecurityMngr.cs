using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Storage;
using TiS.Core.TisCommon.Services;
using TiS.Core.TisCommon.Storage.ObjectStorage;
using System.Runtime.Serialization;
using TiS.Core.TisCommon.Transactions;

namespace TiS.Core.TisCommon.Security
{
	[ComVisible (false)]
	[TisSupportedPermissionsAttribute (PermissionsConst.TIS_PERMISSION_WRITE)]
    [TisServiceAliasAttribute("Security")]
    public class TisSecurityMngr : 
        ITisSecurityMngr, 
        ITisRoles, 
        ITisSupportedPermissions, 
        ITisSecurityCheck,
        ITransactable
	{
		private const string LOGINS_BLOB                = "Roles.xml";
        private const string SUPPORTED_PERMISSIONS_BLOB = "SupportedPermissions.xml"; 

		ITransactionalStorage m_oApplicationResources;

		ITisObjectStorage m_rolesStorage;
        ITisObjectStorage m_permissionsStorage;

		private TisRolesMngr m_oRoleMngr;
		private TisSupportedPermissionsMngr m_oSupportedPermissionsMngr;
		private TisTransactionManager m_oTransactionManager;
        private TRANSACTION_TYPE m_enTransactionType = TRANSACTION_TYPE.SECURED;
		private bool m_bLoaded;
        private PermissionValidationInvocator m_permissionValidationInvocator;
        private ITisSecurityPolicyProviderService m_securityPolicy;
        private TisSecurityPolicy? m_TisSecurityPolicy;

		public TisSecurityMngr (
            ITransactionalStorage oApplicationResources, 
			string sAppName,
            ITisSecurityPolicyProviderService securityPolicy,
            PermissionValidationInvocator permissionValidationInvocator)
		{
			m_oApplicationResources = oApplicationResources as ITransactionalStorage;

            DataContractSerializer rolesDataContractSerializer = new DataContractSerializer(typeof(TisRolesMngr), new Type[] { typeof(TisRole), typeof(TisSystemUsersMngr), typeof(TisSystemUser) });
            DataContractSerializer permissionsDataContractSerializer = new DataContractSerializer(typeof(TisSupportedPermissionsMngr), new Type[] { typeof(TisSupportedPermissionsSet) });

            m_rolesStorage = new ObjectStorage(
                m_oApplicationResources, 
                new ObjectReadDelegate(rolesDataContractSerializer.ReadObject), 
                new ObjectWriteDelegate(rolesDataContractSerializer.WriteObject));

            m_permissionsStorage = new ObjectStorage(
                m_oApplicationResources,
                new ObjectReadDelegate(permissionsDataContractSerializer.ReadObject),
                new ObjectWriteDelegate(permissionsDataContractSerializer.WriteObject));

            m_permissionValidationInvocator = permissionValidationInvocator;
            m_securityPolicy = securityPolicy;
 
			InitTransactionManager ();

			m_bLoaded = false;
		}

        public TisSecurityPolicy TisSecurityPolicy
        {
            get
            {
                // NOTE (IMPORTANT!!!!): The TisSecurityPolicy should not be set in the constructor because going to the server
                // in the constructor cases problems in ConCurrent users. (Binding mismatch -> WebCompletion)
                if (m_TisSecurityPolicy == null)
                {
                    m_TisSecurityPolicy = m_securityPolicy.TisSecurityPolicy;
                }

                return m_TisSecurityPolicy.Value;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_oTransactionManager != null)
            {
                m_oTransactionManager.OnExecuteTransaction -= new TransactionManagerEvent(OnCommitTransaction);
                m_oTransactionManager.OnRollbackTransaction -= new TransactionManagerEvent(OnRollbackTransaction);

              //  m_oTransactionManager.Dispose();
                m_oTransactionManager = null;
            }
        }

        #endregion

		#region ITisSecurityMngr Members

		public ITisRoles Roles 
		{
			get
			{
				LoadIfNeeded ();

				return this as ITisRoles;
			}
		}
		
		public ITisSupportedPermissions SupportedPermissions 
		{
			get
			{
				LoadIfNeeded ();

				return this as ITisSupportedPermissions;
			}
		}

		public ITransactable SecurityTransaction
		{
			get
			{
                return this;
			}
		}

		public void RefreshSecurityData ()
		{
			Clear ();
		}

		public void Clear ()
		{
			if (m_bLoaded)
			{
				m_oSupportedPermissionsMngr.Clear ();
				m_oRoleMngr.RemoveAllRoles ();
			}

			m_bLoaded = false;
		}

		public event SecurityDataDelegate OnSecurityDataLoaded;
		public event SecurityDataDelegate OnSecurityDataSaved;

		#endregion

		#region ITisRoles Members

		ITisRole[] ITisRoles.All
		{
			get
			{
				return oRoleMngr.Roles; 
			}
		}

		public int Count 
		{
			get
			{
				return Roles.All.Length;
			}
		}

		public ITisRole[] CurrentRoles
		{
			get
			{
				return oRoleMngr.CurrentRoles; 
			}
		}

		public ITisRole[] ObtainAttachedToEntity (object oSecuredEntity)
		{
			ArrayList oAttachedRoles = new ArrayList ();

			foreach (ITisRole oRole in Roles.All)
			{
				if (oRole.Permissions.ObtainPermissionsSet (oSecuredEntity) != null)
				{
					oAttachedRoles.Add (oRole);
				}
			}

			ITisRole[] AttachedRoles = new ITisRole [oAttachedRoles.Count];

			oAttachedRoles.CopyTo (AttachedRoles);

			return AttachedRoles; 
		}

		public ITisRole GetByName (string sRoleName)
		{
			return oRoleMngr.GetByName (sRoleName);
		}

		ITisRole[] ITisRoles.AddRoles(string[] RolesName)
		{
			return oRoleMngr.AddRoles (RolesName);
		}

		ITisRole ITisRoles.AddRole (string RoleName)
		{
			ITisRole[] AddedRoles = Roles.AddRoles (new string[] {RoleName});

			return AddedRoles [0];
		}

		public bool Contains (ITisRole oRole)
		{
			return oRoleMngr.ContainsRole (oRole);
		}

		public bool ContainsByName (string sRoleName)
		{
			return oRoleMngr.ContainsRole (sRoleName);
		}

		void ITisRoles.Clear ()
		{
			oRoleMngr.RemoveAllRoles ();
		}

		void ITisRoles.RemoveRoles (ITisRole[] Roles)
		{
			oRoleMngr.RemoveRoles (Roles);
		}

		void ITisRoles.RemoveRolesByName (string[] RolesName)
		{
			oRoleMngr.RemoveRoles (RolesName);
		}


		void ITisRoles.RemoveRole (ITisRole oRole)
		{
			Roles.RemoveRoles (new ITisRole[] {oRole});
		}

		void ITisRoles.RemoveRoleByName (string sRoleName)
		{
			Roles.RemoveRolesByName (new string[] {sRoleName});
		}


		public void RemoveAttachedToEntity (object oSecuredEntity)
		{
			ITisRole[] AttachedRoles = ObtainAttachedToEntity (oSecuredEntity);

			foreach (ITisRole oRole in AttachedRoles)
			{
				oRole.Permissions.DenyAllForEntity (oSecuredEntity);
			}
		}

		public void PreEntityPersistKeyChange (object oSecuredEntity, string sNewPersistKey)
		{
			foreach (TisRole oRole in Roles.All)
			{
				oRole.PrePersistKeyChange (oSecuredEntity, sNewPersistKey);
			}
		}

		public void PostEntityPersistKeyChange (object oSecuredEntity, string sOldPersistKey)
		{
			foreach (TisRole oRole in Roles.All)
			{
				oRole.PostPersistKeyChange (oSecuredEntity, sOldPersistKey);
			}
		}


		#endregion

		#region ITisSupportedPermissions Members

		ITisSupportedPermissionsSet[] ITisSupportedPermissions.All
		{
			get
			{
				return oSupportedPermissionsMngr.PermissionsSets.ToArray(); 
			}
		}

		int ITisSupportedPermissions.Count 
		{
			get
			{
				return SupportedPermissions.All.Length;
			}
		}

		public ITisSupportedPermissionsSet ObtainPermissionsSet (object oSecuredEntity)
		{
			return (ITisSupportedPermissionsSet) oSupportedPermissionsMngr.GetByPersistKey (oSecuredEntity);
		}

		ITisSupportedPermissionsSet ITisSupportedPermissions.AddPermissions (
			object oSecuredEntity, 
			string[] Permissions)
		{
			return (ITisSupportedPermissionsSet) oSupportedPermissionsMngr.AddPermissionsSet (
				oSecuredEntity, 
				Permissions);
		}

		ITisSupportedPermissionsSet ITisSupportedPermissions.AddPermission (
			object oSecuredEntity, 
			string sPermission)
		{
			return SupportedPermissions.AddPermissions (
				oSecuredEntity, 
				new string[] {sPermission});
		}

		void ITisSupportedPermissions.Clear ()
		{
			oSupportedPermissionsMngr.Clear ();
		}

		void ITisSupportedPermissions.RemoveByEntities (object[] SecuredEntities)
		{
			oSupportedPermissionsMngr.RemovePermissionsSets (SecuredEntities);
		}

		void ITisSupportedPermissions.RemoveBySets (ITisSupportedPermissionsSet[] PermissionsSets)
		{
			oSupportedPermissionsMngr.RemovePermissionsSets (PermissionsSets);
		}

		void ITisSupportedPermissions.RemoveByEntity (object oSecuredEntity)
		{
			SupportedPermissions.RemoveByEntities (
				new object[] {oSecuredEntity});
		}

		void ITisSupportedPermissions.RemoveBySet (ITisSupportedPermissionsSet oPermissionsSet)
		{
			SupportedPermissions.RemoveBySets (
				new ITisSupportedPermissionsSet[] {oPermissionsSet});
		}

		public bool HasDefined (object oSecuredEntity)
		{
			return oSupportedPermissionsMngr.ContainsPermissionsSet (oSecuredEntity);
		}

		#endregion

		#region ITisSecurityCheck Members

		public string[] ObtainAllowedPermissions (object oSecuredEntity)
		{
            List<string> allowedPermissions = new List<string>();

            ITisRole[] currentRoles = CurrentRoles;

            foreach (ITisRole role in currentRoles)
            {
                ITisDefinedPermissionsSet rolePermissions =
                    role.Permissions.ObtainPermissionsSet(oSecuredEntity);

                foreach (string permission in rolePermissions.DefinedPermissions.Names)
                {
                    if (!allowedPermissions.Contains(permission))
                    {
                        allowedPermissions.Add(permission);
                    }
                }
            }

            return allowedPermissions.ToArray();
        }

		public int AllowedPermissionsCount (object oSecuredEntity) 
		{
			return ObtainAllowedPermissions (oSecuredEntity).Length; 
		}

		public bool HasPermission (object oSecuredEntity, string sPermissionName)
		{
			return HasPermissions (oSecuredEntity, new string[] {sPermissionName}, out sPermissionName);
		}

        public bool HasPermissions(object oSecuredEntity,
                                    string[] PermissionsName,
                                    out string sFailedPermissionName)
        {
            bool bIsByPassMode;

            sFailedPermissionName = String.Empty;

            List<ITisDefinedPermissionsSet> rolePermissions = ObtainCurrentRolePermissions(oSecuredEntity,
                                                                                            out bIsByPassMode);

            if (rolePermissions.Count == 0)
            {
                return bIsByPassMode;
            }

            bool containsPermission = false;
            bool success = false;

            foreach (string sPermissionName in PermissionsName)
            {
                success = (TisSecurityPolicy == TisSecurityPolicy.Restrictive);

                foreach (ITisDefinedPermissionsSet permissionsSet in rolePermissions)
                {
                    containsPermission = permissionsSet.ContainsPermission(sPermissionName);

                    if (containsPermission && TisSecurityPolicy == TisSecurityPolicy.Permissive)
                    {
                        success = true;
                        break;
                    }
                    else
                    {
                        if (!containsPermission && TisSecurityPolicy == TisSecurityPolicy.Restrictive)
                        {
                            success = false;
                            break;
                        }
                    }
                }

                if (!success)
                {
                    sFailedPermissionName = sPermissionName;
                    break;
                }
            }

            return success;
        }

		public void ValidatePermission (object oSecuredEntity, string sPermissionName)
		{
			ValidatePermissions (oSecuredEntity, new string[] {sPermissionName}, out sPermissionName);
		}

		public void ValidatePermissions (object oSecuredEntity, string[] PermissionsName, out string sFailedPermissionName)
		{
			if (!HasPermissions (oSecuredEntity, PermissionsName, out sFailedPermissionName))
			{
				SecurityAccessException.Raise (oSecuredEntity.ToString (), sFailedPermissionName);
			}
		}

		#endregion

		#region ITisSecurityMiscellaneous Members

		public string[] StorageNames
		{
			get
			{
				return new string[] {LOGINS_BLOB,
									 SUPPORTED_PERMISSIONS_BLOB};
			}
		}

        public TRANSACTION_TYPE TransactionType
        {
            get
            {
                return m_enTransactionType;
            }
            set
            {
                m_enTransactionType = value;
            }
        }

        public void SetBypassMode(BYPASS_MODE bypassMode)
        {
            if (bypassMode == BYPASS_MODE.ADD_ADMIN_USER)
            {
                TisRole adminRole =
                    (TisRole)Roles.GetByName(TisRole.BUILTIN_ADMINISTRATORS_ROLE);

                adminRole.AddCurrentUser();
            }
            else
            {
                foreach (TisRole oRole in Roles.All)
                {
                    oRole.SystemUsers.Clear();
                }
            }
        }

        public void ResetBypassMode()
        {
            TisRole adminRole =
                (TisRole)Roles.GetByName(TisRole.BUILTIN_ADMINISTRATORS_ROLE);

            adminRole.RemoveCurrentUser();
        }

        #endregion

        public TisTransactionManager TransactionManager
        {
            get
            {
                return m_oTransactionManager;
            }
        }


        private List<ITisDefinedPermissionsSet> ObtainCurrentRolePermissions(object oSecuredEntity,
			                                                                 out bool bIsByPassMode)
		{
            List<ITisDefinedPermissionsSet> permissions = new List<ITisDefinedPermissionsSet>();

			ITisRole[] oCurrentRoles = CurrentRoles;

            bIsByPassMode = IsByPassMode(oCurrentRoles);

            if (!bIsByPassMode)
            {
                foreach (ITisRole currentRole in oCurrentRoles)
                {
                    ITisDefinedPermissionsSet rolePermissions =
                        currentRole.Permissions.ObtainPermissionsSet(oSecuredEntity);

                    if (rolePermissions != null)
                    {
                        permissions.Add(rolePermissions);
                    }
                }
            }

            return permissions;
		}

        private bool IsByPassMode(ITisRole[] oCurrentRoles)
		{
            foreach (ITisRole oRole in oCurrentRoles)
            {
                if (IsBuiltinAdministrators(oRole))
                {
                    return true;
                }
            }

            foreach (ITisRole oRole in Roles.All)
			{
				if (oRole.SystemUsers.Count > 0)
				{
					return false;
				}
			}

			// Bypass permissions check for a system user which has no associated role.
			return true;
		}

        private bool IsBuiltinAdministrators(ITisRole oCurrentRole)
        {
            return oCurrentRole != null && oCurrentRole.IsBuiltinAdministrators;
        }

        private void Load()
		{
			try
			{
				m_oSupportedPermissionsMngr = 
					(TisSupportedPermissionsMngr) m_permissionsStorage.LoadObject (SUPPORTED_PERMISSIONS_BLOB);
			}
			catch(Exception oExc)
			{
				Log.Write(
					Log.Severity.INFO,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					oExc.Message
					);

				m_oSupportedPermissionsMngr = new TisSupportedPermissionsMngr ();
			}

			m_oSupportedPermissionsMngr.OnPermissionsSetRemoved += new PermissionsSetDelegate (OnPermissionsSetRemovedHandler);
			m_oSupportedPermissionsMngr.OnPermissionsSetChanged += new PermissionsSetDelegate (OnPermissionsSetChangedHandler);
			m_oSupportedPermissionsMngr.OnCleared               += new EventHandler<EventArgs> (OnSupportedPermissionsClearedHandler);

			try
			{
                m_oRoleMngr = (TisRolesMngr)m_rolesStorage.LoadObject(LOGINS_BLOB);

				OnRolesDeserialization ();
			}
			catch(Exception oExc)
			{
				Log.Write(
					Log.Severity.INFO,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					oExc.Message
					);

				m_oRoleMngr = new TisRolesMngr (new ObtainSupportedPermissionsDelegate (m_oSupportedPermissionsMngr.GetByPersistKey)); 
			}

			m_bLoaded = true;


			if (OnSecurityDataLoaded != null)
			{
				OnSecurityDataLoaded (new EventArgs ());
			}
		}

		private void Save ()
		{
			LoadIfNeeded ();

 
			m_rolesStorage.StoreObject (m_oRoleMngr, LOGINS_BLOB);
			m_permissionsStorage.StoreObject (m_oSupportedPermissionsMngr, SUPPORTED_PERMISSIONS_BLOB);

            if (m_enTransactionType == TRANSACTION_TYPE.SECURED && OnSecurityDataSaved != null)
			{
				OnSecurityDataSaved (new EventArgs ());
			}
		}

		private void LoadIfNeeded ()
		{
			if (!m_bLoaded)
			{
				Load ();
			}
		}

		private TisRolesMngr oRoleMngr
		{
			get
			{
				LoadIfNeeded ();

				return m_oRoleMngr;
			}
		}

		private TisSupportedPermissionsMngr oSupportedPermissionsMngr
		{
			get
			{
				LoadIfNeeded ();

				return m_oSupportedPermissionsMngr;
			}
		}

		private void InitTransactionManager()
		{
			m_oTransactionManager = new TisTransactionManager() ; //"Security", sAppName);

			m_oTransactionManager.OnExecuteTransaction   += new TransactionManagerEvent (OnCommitTransaction);
			m_oTransactionManager.OnRollbackTransaction += new TransactionManagerEvent (OnRollbackTransaction);

			m_oTransactionManager.AddTransactionMember (m_oApplicationResources);
        }

		private void OnRolesDeserialization ()
	    {
			m_oRoleMngr.SupportedPermissionsDelegate = new ObtainSupportedPermissionsDelegate (m_oSupportedPermissionsMngr.GetByPersistKey); 

			foreach (TisRole oRole in m_oRoleMngr.Roles)
			{
				oRole.SupportedPermissionsDelegate = m_oRoleMngr.SupportedPermissionsDelegate;
			}
		}

		private void OnPermissionsSetRemovedHandler (PermissionsSetArgs e)
		{
			m_oRoleMngr.SynchronizePermissions (e.TypedPersistKey);
		}

		private void OnPermissionsSetChangedHandler (PermissionsSetArgs e)
		{
			m_oRoleMngr.SynchronizePermissions (e.TypedPersistKey);
		}

		private void OnSupportedPermissionsClearedHandler(object sender, EventArgs e)
		{
			m_oRoleMngr.SynchronizePermissions ();
		}

		private void OnCommitTransaction(object sender, EventArgs oArgs)
		{
			Save();
		}

		private void OnRollbackTransaction(object sender, EventArgs oArgs)
		{
			Load ();
		}

        #region ITransactable Members

        public void PrepareTransaction()
        {
            m_oTransactionManager.PrepareTransaction();
        }

        public void ExecuteTransaction()
        {
            m_oTransactionManager.ExecuteTransaction();
        }

        public void RollbackTransaction()
        {
            m_oTransactionManager.RollbackTransaction();
        }

        public bool InTransaction
        {
            get
            {
                return m_oTransactionManager.InTransaction;
            }
        }

        #endregion
    }

	internal class TisServicesSecurityCheck : ITisServiceSecurityCheck
	{
		private ITisServiceInfo m_oServiceInfo;
		private ITisSecurityCheck m_oSecurityCheck;

		public TisServicesSecurityCheck (ITisServiceInfo oServiceInfo, ITisSecurityCheck oSecurityCheck)
		{
			m_oServiceInfo   = oServiceInfo;
			m_oSecurityCheck = oSecurityCheck;
		}

		#region ITisServiceSecurityCheck Members

		public string[] AllowedPermissions
		{
			get
			{
				return m_oSecurityCheck.ObtainAllowedPermissions (m_oServiceInfo);
			}
		}

		public int AllowedPermissionsCount 
		{
			get
			{
				return AllowedPermissions.Length; 
			}
		}

		public bool HasPermission(string sPermissionName)
		{
			return (m_oSecurityCheck.HasPermission (m_oServiceInfo, sPermissionName));
		}

		public bool HasPermissions (string[] PermissionsName, 
			                        out string sFailedPermissionName)
		{
			return (m_oSecurityCheck.HasPermissions (m_oServiceInfo, PermissionsName, out sFailedPermissionName));
		}

		public void ValidatePermission (string sPermissionName)
		{
			ValidatePermissions (new string[] {sPermissionName}, out sPermissionName);
		}

		public void ValidatePermissions (string[] PermissionsName, out string sFailedPermissionName)
		{
			sFailedPermissionName = String.Empty;

			try
			{
				m_oSecurityCheck.ValidatePermissions (m_oServiceInfo, PermissionsName, out sFailedPermissionName);
			}
			catch (TisException)
			{
				SecurityAccessException.Raise (m_oServiceInfo.ServiceName, sFailedPermissionName);
			}
			catch (UnauthorizedAccessException)
			{
				SecurityAccessException.Raise (m_oServiceInfo.ServiceName, sFailedPermissionName);
			}
		}

		#endregion
	}
}
