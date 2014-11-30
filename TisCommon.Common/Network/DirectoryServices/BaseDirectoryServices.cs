using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using ActiveDs;

namespace TiS.Core.TisCommon.Network
{
    public abstract class BaseDirectoryServices
    {
        private string m_serverName;
        private DirectoryEntry m_directorySearchRoot;
        private BaseDirectorySearchSession m_directorySearchSession;
        private bool doCancelLongAction = false;

        public BaseDirectoryServices(string serverName)
        {
            m_serverName = serverName;

            if (DirectoryServicesUtils.IsLocalMachine(serverName))
            {
                m_directorySearchRoot = 
                    new DirectoryEntry(DirectoryServicesUtils.LOCAL_SEARCH_PROTOCOL + serverName + ",Computer");
            }
            else
            {
                m_directorySearchRoot = 
                    new DirectoryEntry(DirectoryServicesUtils.DOMAIN_SEARCH_PROTOCOL + serverName);
            }
        }

        public string ServerName
        {
            get
            {
                return m_serverName;
            }
        }

        public bool CancelLongAction
        {
            get
            {
                return doCancelLongAction;
            }
            set
            {
                doCancelLongAction = value;
            }
        }

        protected abstract string AccountNameProperty { get;}

        #region Users

        public virtual List<string> GetAllUsers(bool includeValidUsersOnly)
        {
            return GetSpecificUsers(null, includeValidUsersOnly);
        }

        public virtual List<string> ValidateUsers(List<string> userNames)
        {
            return GetSpecificUsers(userNames, true);
        }

        protected abstract List<string> GetSpecificUsers(List<string> userNames, bool includeValidUsersOnly);

        #endregion

        #region Groups

        public virtual List<string> GetAllGroups(bool includeValidGroupsOnly)
        {
            return GetSpecificGroups(null, includeValidGroupsOnly);
        }

        public virtual List<string> ValidateGroups(List<string> groupNames)
        {
            return GetSpecificGroups(groupNames, true);
        }

        public abstract List<string> GetUserMembership(string userName);

        protected abstract List<string> GetSpecificGroups(List<string> groupNames, bool includeValidGroupsOnly);

        #endregion

        internal BaseDirectorySearchSession DirectorySearchSession
        {
            get
            {
                return m_directorySearchSession;
            }
            set
            {
                m_directorySearchSession = value;
            }
        }

        protected DirectoryEntry SearchRoot
        {
            get
            {
                return m_directorySearchRoot;
            }
        }

        protected bool ShouldIncludeItem(string item, List<string> itemNames)
        {
            if (itemNames != null && itemNames.Count > 0 && !itemNames.Contains(item))
            {
                return false;
            }

            return true;
        }

        protected bool IsValidADsObject(object ADsObject)
        {
            if (ADsObject is IADsUser)
            {
                IADsUser user = (IADsUser)ADsObject;

                return !user.AccountDisabled && !user.IsAccountLocked;
            }
            else
            {
                return true;
            }
        }

        protected void GetUserMembership(IADsUser user, string propertyName, List<string> userGroups)
        {
            try
            {
                IADsMembers membership = user.Groups();

                foreach (IADsGroup group in membership)
                {
                    userGroups.Add(DirectoryServicesUtils.GetObjectAccountName(group, propertyName));
                }
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to obtain user membership. Details : {0}", exc.Message);
            }
        }

        protected void GetUserMembership(IADsGroup group, string userName, string propertyName, List<string> userGroups)
        {
            try
            {
                string userAccountName;
                string groupAccountName = String.Empty;

                IADsMembers membership = group.Members();

                foreach (object obj in membership)
                {
                    userAccountName = DirectoryServicesUtils.GetObjectAccountName((IADs)obj, propertyName);

                    if (StringUtil.CompareIgnoreCase(userAccountName, userName))
                    {
                        if (!StringUtil.IsStringInitialized(groupAccountName))
                        {
                            groupAccountName = DirectoryServicesUtils.GetObjectAccountName(group, propertyName);
                        }

                        userGroups.Add(groupAccountName);
                    }
                }
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to obtain user membership. Details : {0}", exc.Message);
            }
        }

        protected void SearchSpecificObjects(StringBuilder allObjectsFilter, List<string> objectNames)
        {
            try
            {
                if (objectNames != null && objectNames.Count > 0)
                {
                    StringBuilder specificObjectsFilter = new StringBuilder("(|");

                    foreach (string objectName in objectNames)
                    {
                        specificObjectsFilter.Append(
                            "(" + DirectoryServicesUtils.DOMAIN_ACCOUNT_NAME_PROPERTY + "=" + objectName + ")");
                    }

                    specificObjectsFilter.Append(")");

                    allObjectsFilter.Insert(0, "(&");

                    allObjectsFilter.Append(specificObjectsFilter.ToString() + ")");
                }

                DirectorySearchSession.Search(allObjectsFilter.ToString());
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to obtain domain objects. Details : {0}", exc.Message);
            }
        }
    }
}
