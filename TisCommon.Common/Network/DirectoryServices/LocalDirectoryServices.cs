using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using ActiveDs;

namespace TiS.Core.TisCommon.Network
{
    public class LocalDirectoryServices : BaseDirectoryServices
    {
        public LocalDirectoryServices(string machineName)
            : base(machineName)
        {
            DirectorySearchSession = new LocalDirectorySearchSession(SearchRoot);
        }

        protected override string AccountNameProperty
        {
            get
            {
                return DirectoryServicesUtils.LOCAL_ACCOUNT_NAME_PROPERTY;
            }
        }

        #region Users

        protected override List<string> GetSpecificUsers(List<string> userNames, bool includeValidUsersOnly)
        {
            return GetSpecificObjects("user", userNames, includeValidUsersOnly);
        }

        #endregion

        #region Groups

        public override List<string> GetUserMembership(string userName)
        {
            List<string> userGroups = new List<string>();
            List<string> groupNames = new List<string>();

            try
            {
                SearchSpecificObjects(
                    new StringBuilder(DirectoryServicesUtils.ALL_GROUPS_SEARCH_FILTER),
                    null);

                IADsGroup group;

                foreach (DirectoryEntry searchResult in DirectorySearchSession.LastResults)
                {
                    group = (IADsGroup)searchResult.NativeObject;

                    if (group != null)
                    {
                        GetUserMembership(group, userName, AccountNameProperty, userGroups);
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
            return GetSpecificObjects("group", groupNames, includeValidGroupsOnly);
        }

        #endregion

        #region Private

        private List<string> GetSpecificObjects(
            string objectSchemaClassName, 
            List<string> objectNames, 
            bool includeValidObjectsOnly)
        {
            List<string> specificObjects = new List<string>();

            SearchSpecificObjects(
                new StringBuilder("objectclass=" + objectSchemaClassName), 
                objectNames);

            if (DirectorySearchSession.LastResults != null)
            {
                foreach (DirectoryEntry Object in DirectorySearchSession.LastResults)
                {
                    if (!includeValidObjectsOnly || IsValidADsObject(Object.NativeObject))
                    {
                        specificObjects.Add(DirectoryServicesUtils.GetObjectAccountName(Object, null));
                    }
                }
            }

            return specificObjects;
        }

        #endregion
    }
}
