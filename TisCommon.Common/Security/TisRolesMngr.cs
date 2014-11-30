using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Network;

namespace TiS.Core.TisCommon.Security
{
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisRolesMngr : IDeserializationCallback
	{
        [DataMember]
        private DictionaryWithEvents<string, ITisRole> m_oRoles;

		[IgnoreDataMember]
        private DictionaryWithEvents<string, List<string>> m_oSystemUserNetGroups = new DictionaryWithEvents<string, List<string>>();

		[IgnoreDataMember]
		private ObtainSupportedPermissionsDelegate  m_oSupportedPermissionsDelegate;

		public TisRolesMngr ()
		{
		}

		public TisRolesMngr (ObtainSupportedPermissionsDelegate oSupportedPermissionsDelegate) : this ()
		{
            m_oRoles = new DictionaryWithEvents<string, ITisRole>();

			m_oSupportedPermissionsDelegate = oSupportedPermissionsDelegate;

			AddRole (TisRole.BUILTIN_ADMINISTRATORS_ROLE);
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
 
		#region ITisRolesMngr Members

		public ITisRole[] Roles
		{
			get
			{
				ITisRole[] _Roles = (ITisRole[]) Array.CreateInstance (typeof (ITisRole), m_oRoles.Count);

				int Index = 0;

				foreach (KeyValuePair<string, ITisRole> kvp in m_oRoles)
				{
                    _Roles[Index++] = (ITisRole)m_oRoles[kvp.Key]; 
				}

				return _Roles;
			}
		}

		public ITisRole[] CurrentRoles
		{
			get
			{
                List<TisRole> currenRoles = new List<TisRole>(ObtainCurrentRoles(USER_TYPE.USER));

                currenRoles.AddRange(ObtainCurrentRoles(USER_TYPE.GROUP));

                return currenRoles.ToArray();
			}
		}

		public ITisRole[] AddRoles (string[] RolesName)
		{
			ITisRole oRole;
			ArrayList oRoles = new ArrayList ();

			foreach (string sRoleName in RolesName)
			{
				oRole = AddRole (sRoleName);

				if (!oRoles.Contains (oRole))
				{
					oRoles.Add (oRole);
				}
			}

			ITisRole[] Roles = new ITisRole [oRoles.Count];

			Roles = (ITisRole[]) oRoles.ToArray (typeof (ITisRole));

			return Roles;
		}

		public void RemoveAllRoles ()
		{
			m_oRoles.Clear ();
		}

		public void RemoveRoles (string[] RolesName)
		{
			foreach (string sRoleName in RolesName)
			{
				m_oRoles.Remove (sRoleName);
			}
		}

		public void RemoveRoles (ITisRole[] Roles)
		{
			foreach (ITisRole oRole in Roles)
			{
				if (oRole != null)
				{
					m_oRoles.Remove (oRole.Name);
				}
			}
		}

		public bool ContainsRole (string sRoleName)
		{
			return m_oRoles.ContainsKey (sRoleName);
		}

		public bool ContainsRole (ITisRole oRole)
		{
			return m_oRoles.ContainsValue (oRole);
		}

		public ITisRole GetByName (string sRoleName)
		{
			return (ITisRole) m_oRoles [sRoleName];
		}

		#endregion

		internal void SynchronizePermissions ()
		{
			foreach (TisRole oRole in Roles)
			{
				oRole.Permissions.Clear ();
			}
		}

		internal void SynchronizePermissions (string sTypedPersistKey)
		{
			ArrayList oTemp = new ArrayList ();

			ITisSupportedPermissionsSet oSupportedPermissionsSet = 
				(ITisSupportedPermissionsSet) m_oSupportedPermissionsDelegate (sTypedPersistKey);

			foreach (TisRole oRole in Roles)
			{
				foreach (TisDefinedPermissionsSet oPermissionsSet in oRole.Permissions.All)
				{
					if (oPermissionsSet.TypedPersistKey == sTypedPersistKey)
					{
						if (oSupportedPermissionsSet != null)
						{
							oTemp.Clear ();

							foreach (string sPermission in oPermissionsSet.DefinedPermissions.Names)
							{
								if (!oSupportedPermissionsSet.ContainsPermission (sPermission))
								{
									oTemp.Add (sPermission);
								}
							}

							oPermissionsSet.DefinedPermissions.RemovePermissions (
								(string[]) oTemp.ToArray (typeof(string)));
						}
						else
						{
							oRole.Permissions.RemoveBySet (oPermissionsSet);
						}
					}
				}
			}
		}

		private TisRole[] ObtainCurrentRoles (USER_TYPE enUserType)
		{
			ArrayBuilder oRoles = new ArrayBuilder(typeof(TisRole));

			foreach (KeyValuePair<string, ITisRole> kvp in m_oRoles)
			{
                TisRole oRole = (TisRole)m_oRoles[kvp.Key];

				if (enUserType == USER_TYPE.USER)
				{
					if (oRole.ContainsCurrentSystemUser ()) 
					{
						oRoles.Add(oRole);
					}
				}
				else
				{
					if (oRole.ContainsCurrentSystemGroup ()) 
					{
                        oRoles.Add(oRole);
                    }
				}
			}

			return (TisRole[])oRoles.GetArray();
		}

		public TisRole AddRole (string sRoleName)
		{
			if (m_oRoles.ContainsKey(sRoleName))
			{
				return (TisRole) m_oRoles [sRoleName];
			}

			TisRole oRole = new TisRole (sRoleName, m_oSupportedPermissionsDelegate);

			oRole.OnRoleNameChanged     += new RoleNameChangedDelegate     (OnRoleNameChangedHandler);
			oRole.OnRoleSystemUserAdded += new RoleSystemUserAddedDelegate (OnRoleSystemUserAddedHandler);
			oRole.OnFindSystemUserInNetGroup +=new SystemUserInNetGroupDelegate(OnFindSystemUserInNetGroupHandler);

			m_oRoles.Add (sRoleName, oRole);

			return oRole;
		}

		private void OnRoleNameChangedHandler(RoleNameChangedArgs e)
		{
			m_oRoles.Remove (e.OldName);

			m_oRoles.Add (e.Role.Name, e.Role);
		}

		private void OnRoleSystemUserAddedHandler(RoleSystemUserArgs e)
		{
			e.IsValid = ValidateSystemUser (e.Role, e.SystemUser);
		}


		private bool OnFindSystemUserInNetGroupHandler(SystemUserInNetGroupArgs e)
		{
			string sSystemUserName = e.SystemUser.ToUpper();
			string sNetGroupName   = e.NetGroup.ToUpper();

			List<string> oNetGroups = default(List<string>);

			if (!m_oSystemUserNetGroups.ContainsKey(sSystemUserName))
			{
                oNetGroups = ObtainUserGroups(sSystemUserName);

                m_oSystemUserNetGroups[sSystemUserName] = oNetGroups;
			}
			else
			{
				oNetGroups = (List<string>) m_oSystemUserNetGroups [sSystemUserName];
			}

            foreach (string netGroup in oNetGroups)
            {
                if (StringUtil.CompareIgnoreCase(netGroup, sNetGroupName))
                {
                    return true;
                }
            }

			return false;
		}

        private List<string> ObtainUserGroups(string sSystemUserName)
        {
            string sServerName = String.Empty;
            string sUserName = String.Empty;

            List<string> oNetGroups = new List<string>();

            DirectoryServicesUtils.ParseAccountName(sSystemUserName, out sServerName, out sUserName);

            oNetGroups.AddRange(ObtainFullUserGroupName(sServerName, sUserName)); // Domain user groups
            oNetGroups.AddRange(ObtainFullUserGroupName(Environment.MachineName, sSystemUserName)); // Local user groups

            return oNetGroups;
        }

        private string[] ObtainFullUserGroupName(string serverName, string userName)
        {
            return DirectoryServicesAccess.GetUserGroups(serverName, userName);
        }

        private bool ValidateSystemUser(ITisRole oRoleToCheck, ITisSystemUser oSystemUser)
		{
			return true;
		}

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
//			m_oRoles.OnDeserialization (sender);

			AddRole (TisRole.BUILTIN_ADMINISTRATORS_ROLE);

			foreach (TisRole oRole in Roles)
			{
				oRole.OnRoleNameChanged     += new RoleNameChangedDelegate     (OnRoleNameChangedHandler);
				oRole.OnRoleSystemUserAdded += new RoleSystemUserAddedDelegate (OnRoleSystemUserAddedHandler);
				oRole.OnFindSystemUserInNetGroup +=new SystemUserInNetGroupDelegate (OnFindSystemUserInNetGroupHandler);
			}
		}

		#endregion
	}

}
