using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Management;

namespace TiS.Core.TisCommon.Network
{
    public class DirectoryServicesAccess
    {
        private static Dictionary<string, BaseDirectoryServices> m_serverDSCache = 
            new Dictionary<string, BaseDirectoryServices>();

        #region Domains

        public static string[] GetAvailableDomains()
        {
            return GetAvailableDomains(true, false);
        }

        public static string[] GetAvailableDomains(bool includeLocalMachine, bool includeFullName)
        {
            int domainsCount = 1;
            DomainCollection domains = null;

            try
            {
                domains = Forest.GetCurrentForest().Domains;

                domainsCount += domains.Count;
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to obtain available domains. Details : {0}", exc.Message);
            }

            List<string> domainNames = new List<string>(domainsCount);

            if (includeLocalMachine)
            {
                domainNames.Add(Environment.MachineName);

                //Log.WriteInfo("Added local machine name : [{0}]", Environment.MachineName);
            }

            string domainName;

            if (domains != null && domains.Count > 0)
            {
                foreach (Domain domain in domains)
                {
                    if (includeFullName)
                    {
                        domainName = domain.Name;

                        //Log.WriteInfo("Added full domain name : [{0}]", domainName);
                    }
                    else
                    {
                        domainName = domain.GetDirectoryEntry().Properties["name"].Value.ToString();

                        //Log.WriteInfo("Added short domain name : [{0}]", domainName);
                    }

                    domainNames.Add(domainName);
                }
            }
            else
            {
                try
                {
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("SELECT Domain FROM Win32_ComputerSystem");

                    searcher.Options.ReturnImmediately = true;
                    searcher.Options.Rewindable = true;
                    searcher.Options.DirectRead = true;

                    foreach (ManagementObject oManagementObj in searcher.Get())
                    {
                        object domain = oManagementObj.GetPropertyValue("Domain");

                        if (domain != null)
                        {
                            domainName = domain.ToString();

                            domainNames.Add(domainName);

                            //Log.WriteInfo("Added domain name : [{0}]", domainName);
                        }

                        break;
                    }
                }
                catch (Exception exc)
                {
                    Log.WriteWarning("Failed to obtain available domains. Details : {0}", exc.Message);
                }
            }

            return domainNames.ToArray();
        }

        #endregion

        #region Users

        public static string[] GetUsers(string sServerName)
        {
            BaseDirectoryServices serverDS = GetServerDS(sServerName);

            List<string> allUsers = serverDS.GetAllUsers(true);

            return allUsers.ToArray();
        }

        public static string[] ValidateUsers(string sServerName, string[] userNames)
        {
            BaseDirectoryServices serverDS = GetServerDS(sServerName);

            List<string> validUsers = serverDS.ValidateUsers(new List<string>(userNames));

            return validUsers.ToArray();
        }

        #endregion

        #region Groups

        public static string[] GetGroups(string sServerName)
        {
            BaseDirectoryServices serverDS = GetServerDS(sServerName);

            List<string> allGroups = serverDS.GetAllGroups(true);

            return allGroups.ToArray();
        }

        public static string[] ValidateGroups(string sServerName, string[] groupNames)
        {
            BaseDirectoryServices serverDS = GetServerDS(sServerName);

            List<string> validGroups = serverDS.ValidateGroups(new List<string>(groupNames));

            return validGroups.ToArray();
        }

        public static string[] GetUserGroups(string sServerName, string sUserName)
        {
            BaseDirectoryServices serverDS = GetServerDS(sServerName);

            List<string> userGroups = serverDS.GetUserMembership(sUserName);

            return userGroups.ToArray();
        }

        public static string[] GetUserGroups(string fullAccountName)
        {
            string serverName;
            string userName;

            if (DirectoryServicesUtils.ParseAccountName(fullAccountName, out serverName, out userName))
            {
                return GetUserGroups(serverName, userName);
            }
            else
            {
                return new string[] { };
            }
        }

        public static void CancelAction()
        {
            foreach (BaseDirectoryServices ds in m_serverDSCache.Values)
            {
                ds.CancelLongAction = true;
            }
        }

        #endregion

        #region Private methods

        private static BaseDirectoryServices GetServerDS(string serverName)
        {
            BaseDirectoryServices serverDS;

            m_serverDSCache.TryGetValue(serverName, out serverDS);

            if (serverDS == null)
            {
                if (DirectoryServicesUtils.IsLocalMachine(serverName))
                {
                    serverDS = new LocalDirectoryServices(serverName);
                }
                else
                {
                    serverDS = new DomainDirectoryServices(serverName);
                }

                m_serverDSCache.Add(serverName, serverDS);
            }

            return serverDS;
        }

        #endregion
    }
}
