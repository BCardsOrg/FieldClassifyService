using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.Security
{
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisSystemUser : ITisSystemUser, IDeserializationCallback
	{
		[IgnoreDataMember]
		private IIdentity m_oSystemUser;

        [DataMember]
		private string m_sName;
        [DataMember]
        private string m_sAuthenticationType;

		public TisSystemUser ()
		{
		}

		public TisSystemUser (
			string sUserName, 
			AUTHENTICATION_TYPE enAuthType)
		{
			m_sName                         = sUserName;
			m_sAuthenticationType           = enAuthType.ToString ();

			m_oSystemUser = new GenericIdentity (sUserName, m_sAuthenticationType);
		}

		#region ITisSystemUser Members

		public string Name
		{
			get
			{
				return m_oSystemUser.Name;
			}
		}

		public AUTHENTICATION_TYPE AuthenticationType
		{
			get
			{
				return (AUTHENTICATION_TYPE) Enum.Parse (
					typeof(AUTHENTICATION_TYPE), 
					m_oSystemUser.AuthenticationType);
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_oSystemUser.IsAuthenticated;
			}
		}

		#endregion

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
			m_oSystemUser = new GenericIdentity (m_sName, m_sAuthenticationType);
		}

		#endregion
	}

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisSystemUsersMngr
	{
        [DataMember]
        private ListWithEvents<TisSystemUser> m_oSystemUsers;

		public TisSystemUsersMngr ()
		{
            m_oSystemUsers = new ListWithEvents<TisSystemUser>();
		}

		#region ITisSystemUsersMngr Members

		public List<TisSystemUser> SystemUsers
		{
			get
			{
                return new List<TisSystemUser>(m_oSystemUsers);
			}
		}

		public ITisSystemUser[] Add (string[] UsersName)
		{
			return Add (UsersName, AUTHENTICATION_TYPE.KERBEROS);
		}

		public ITisSystemUser[] Add (string[] UsersName, AUTHENTICATION_TYPE enAuthType)
		{
			ArrayList oSystemUsers = new ArrayList ();
			TisSystemUser oSystemUser;

			foreach (string sUserName in UsersName)
			{
				oSystemUser = GetByIdentity (sUserName, enAuthType);

				if (OnSystemUserAdded != null && oSystemUser == null)
				{
					oSystemUser = new TisSystemUser (sUserName, enAuthType);

					SystemUserArgs oSystemUserArgs = new SystemUserArgs (oSystemUser);

					OnSystemUserAdded (oSystemUserArgs);

					if (oSystemUserArgs.IsValid)
					{
						m_oSystemUsers.Add (oSystemUser);
					}
					else
					{
						Log.WriteWarning ("{0} : [{1}]",
							System.Reflection.MethodInfo.GetCurrentMethod(),  
							String.Format ("Attempt to add user [{0}] failed", oSystemUser.Name));

						continue;
					}
				}

				oSystemUsers.Add (oSystemUser);
			}

			return (ITisSystemUser[]) ArrayBuilder.CreateArray (
				oSystemUsers, 
				typeof (ITisSystemUser), 
				new ArrayElementFilter (AddSystemUsersFilter));
		}

		public ITisSystemUser Add (string UsersName)
		{
			return Add (UsersName, AUTHENTICATION_TYPE.KERBEROS);
		}

		public ITisSystemUser Add (string UsersName, AUTHENTICATION_TYPE enAuthType)
		{
			ITisSystemUser[] SystemUsers = Add (new string[] {UsersName}, enAuthType);

			return SystemUsers [0];
		}

		public void Clear ()
		{
			m_oSystemUsers.Clear ();
		}

		public void RemoveAll (AUTHENTICATION_TYPE enAuthType)
		{
			TisSystemUser oSystemUser;

			for (int i = m_oSystemUsers.Count - 1; i >=0; i--)
			{
				oSystemUser = (TisSystemUser) m_oSystemUsers [i];

				if (oSystemUser.AuthenticationType == enAuthType)
				{
					m_oSystemUsers.Remove (oSystemUser);
				}
			}
		}

		public void Remove (string[] UsersName)
		{
			Remove (UsersName, AUTHENTICATION_TYPE.KERBEROS);
		}

		public void Remove (string[] UsersName, AUTHENTICATION_TYPE enAuthType)
		{
			TisSystemUser oSystemUser;

			for (int i = m_oSystemUsers.Count - 1; i >=0; i--)
			{
				oSystemUser = (TisSystemUser) m_oSystemUsers [i];

				if (Array.IndexOf (UsersName, oSystemUser.Name) > -1 && 
					oSystemUser.AuthenticationType == enAuthType)
				{
					m_oSystemUsers.Remove (oSystemUser);
				}
			}
		}

		public void Remove (ITisSystemUser[] SystemUsers)
		{
			foreach (TisSystemUser oSystemUser in SystemUsers)
			{
				m_oSystemUsers.Remove(oSystemUser);
			}
		}

		public bool Contains (string SytemUserName)
		{
			return Contains (new TisSystemUser (SytemUserName, AUTHENTICATION_TYPE.KERBEROS));
		}

		public bool Contains (string SytemUserName, AUTHENTICATION_TYPE enAuthType)
		{
			return Contains (new TisSystemUser (SytemUserName, enAuthType));
		}

		public bool Contains (TisSystemUser oSystemUser)
		{
			return Contains (oSystemUser); 
		}

		public bool Contains (object item)
		{
            if (m_oSystemUsers.Contains((TisSystemUser)item))
			{
				return true;
			}
			else
			{
				TisSystemUser oSystemUser = GetByIdentity (
					((TisSystemUser) item).Name, 
					((TisSystemUser) item).AuthenticationType);

				return oSystemUser != null; 
			}
		}

		public bool ContainsCurrentSystemUser ()
		{
			return CurrentSystemUser != null;
		}

		public bool ContainsCurrentSystemGroup ()
		{
			return CurrentSystemGroup != null;
		}

		public ITisSystemUser CurrentSystemUser
		{
			get
			{
				return GetUserByPrincipal (GetCurrentPrincipal ());
			}
		}

		public ITisSystemUser CurrentSystemGroup
		{
			get
			{
				return GetGroupByPrincipal (GetCurrentPrincipal());
			}
		}

		public TisSystemUser GetByIdentity (string sName, AUTHENTICATION_TYPE enAuthType)
		{
			foreach (TisSystemUser oSystemUser in m_oSystemUsers)
			{
				if (StringUtil.CompareIgnoreCase (oSystemUser.Name, sName))
				{
					return oSystemUser;
				}
			}

			return null;
		}

		public ITisSystemUser GetByName (string sName)
		{
			foreach (ITisSystemUser oUser in m_oSystemUsers)
			{
				if (StringUtil.CompareIgnoreCase (oUser.Name, sName))
				{
					return oUser;
				}
			}

			return null;
		}

		public event SystemUserAddedDelegate OnSystemUserAdded;
		public event SystemUserInNetGroupDelegate OnFindSystemUserInNetGroup;

		#endregion

        internal ITisSystemUser AddCurrentUser()
        {
            IPrincipal currentPrincipal = GetCurrentPrincipal();

            if (currentPrincipal != null)
            {
                return Add(currentPrincipal.Identity.Name);
            }
            else
            {
                return null;
            }
        }

        internal void RemoveCurrentUser()
        {
            IPrincipal currentPrincipal = GetCurrentPrincipal();

            if (currentPrincipal != null)
            {
                Remove(new string[] { currentPrincipal.Identity.Name });
            }
        }

        private IPrincipal GetCurrentPrincipal()
		{
			IPrincipal oCurrentPrincipal = Thread.CurrentPrincipal;

			if (!oCurrentPrincipal.Identity.IsAuthenticated)
			{
				oCurrentPrincipal = new WindowsPrincipal (WindowsIdentity.GetCurrent ());
			}

			if (!oCurrentPrincipal.Identity.IsAuthenticated)
			{
				return null;
			}

			return oCurrentPrincipal;
	    }

		private TisSystemUser GetUserByPrincipal (IPrincipal oPrincipal)
		{
			return GetByIdentity (oPrincipal.Identity.Name, oPrincipal.Identity.AuthenticationType);
		}

		private TisSystemUser GetGroupByPrincipal (IPrincipal oPrincipal)
		{
			AUTHENTICATION_TYPE enPrincipalAuthType = StringToAuthentication (oPrincipal.Identity.AuthenticationType);

			foreach (TisSystemUser oSystemUser in m_oSystemUsers)
			{
				if (oSystemUser.AuthenticationType == enPrincipalAuthType)
                {
					if (OnFindSystemUserInNetGroup != null &&
						OnFindSystemUserInNetGroup (
						new SystemUserInNetGroupArgs (oPrincipal.Identity.Name, oSystemUser.Name)))
					{
						return oSystemUser;
					}
				}
			}

			return null;
		}

		private TisSystemUser GetByIdentity (string sName, string sAuthType)
		{
			return GetByIdentity (sName, StringToAuthentication (sAuthType));
		}

		private AUTHENTICATION_TYPE StringToAuthentication (string sAuthType)
		{
			return (AUTHENTICATION_TYPE) Enum.Parse (typeof (AUTHENTICATION_TYPE), sAuthType.ToUpper());
		}

		private bool AddSystemUsersFilter (object oElement)
		{
			return oElement is ITisSystemUser;
		}
	}

	[ComVisible(false)]
	public class SystemUserArgs : EventArgs
	{
		private ITisSystemUser m_oSystemUser;
		private bool m_bIsValid = true;

		public SystemUserArgs (ITisSystemUser oSystemUser)
		{
			m_oSystemUser = oSystemUser;
		}

		public ITisSystemUser SystemUser
		{
			get
			{
				return m_oSystemUser;
			}
		}

		public bool IsValid
		{
			get
			{
				return m_bIsValid;
			}
			set
			{
				m_bIsValid = value;
			}
		}
	}

	[ComVisible(false)]
	public class SystemUserInNetGroupArgs : EventArgs
	{
		private string m_sSystemUser;
		private string m_sNetGroup;

		public SystemUserInNetGroupArgs (string sSystemUser, string sNetGroup)
		{
			m_sSystemUser = sSystemUser;
			m_sNetGroup   = sNetGroup; 
		}

		public string SystemUser
		{
			get
			{
				return m_sSystemUser;
			}
		}

		public string NetGroup
		{
			get
			{
				return m_sNetGroup;
			}
		}
	}
}
