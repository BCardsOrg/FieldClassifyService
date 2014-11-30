using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.DirectoryServices;
using ActiveDs;

namespace TiS.Core.TisCommon.Network
{
    #region DomainDirectoryServices

    public class DomainDirectoryServices : BaseDirectoryServices
    {
        private const string ALL_USERS_SEARCH_FILTER = 
            "(&(objectclass=person)(objectclass=user)(!(objectclass=computer)))";

        public DomainDirectoryServices(string serverName)
            : base(serverName)
        {
            DirectorySearchSession = 
                new DomainDirectorySearchSession(SearchRoot);
        }

        protected override string AccountNameProperty
        {
            get
            {
                return DirectoryServicesUtils.DOMAIN_ACCOUNT_NAME_PROPERTY;
            }
        }

        #region Users

        protected override List<string> GetSpecificUsers(List<string> userNames, bool includeValidUsersOnly)
        {
            return GetSpecificObjects(
                userNames,
                new StringBuilder(ALL_USERS_SEARCH_FILTER),
                includeValidUsersOnly);
        }

        #endregion

        #region Groups

        public override List<string> GetUserMembership(string userName)
        {
            List<string> userNames = new List<string>();
            List<string> userGroups = new List<string>();

            try
            {
                userNames.Add(userName);

                SearchSpecificObjects(new StringBuilder(ALL_USERS_SEARCH_FILTER), userNames);

                IADsUser user;

                foreach (SearchResult searchResult in DirectorySearchSession.LastResults)
                {
                    user = (IADsUser)searchResult.GetDirectoryEntry().NativeObject;

                    if (user != null)
                    {
                        GetUserMembership(user, AccountNameProperty, userGroups);

                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to obtain user groups. Details : {0}", exc.Message);
            }

            return userGroups;
        }

        protected override List<string> GetSpecificGroups(List<string> groupNames, bool includeValidGroupsOnly)
        {
            return GetSpecificObjects(
                groupNames,
                new StringBuilder(DirectoryServicesUtils.ALL_GROUPS_SEARCH_FILTER), 
                includeValidGroupsOnly);
        }

        #endregion

        private List<string> GetSpecificObjects(
            List<string> objectsNames,
            StringBuilder allObjectsFilter,
            bool includeValidObjectsOnly)
        {
            DirectoryEntry de;
            List<string> specificObjects = new List<string>();

            try
            {
                SearchSpecificObjects(allObjectsFilter, objectsNames);

                if (DirectorySearchSession.LastResults != null)
                {
                    foreach (SearchResult Object in DirectorySearchSession.LastResults)
                    {
                        de = Object.GetDirectoryEntry();

                        if (!includeValidObjectsOnly || IsValidADsObject(de.NativeObject))
                        {
                            specificObjects.Add(
                                DirectoryServicesUtils.GetObjectAccountName(de, AccountNameProperty));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to obtain domain objects. Details : {0}", exc.Message);
            }

            return specificObjects;
        }
    }

    #endregion
}
