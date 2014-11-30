using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.Security.Principal;
using ActiveDs;

namespace TiS.Core.TisCommon.Network
{
    public class DirectoryServicesUtils
    {
        public const string LOCAL_SEARCH_PROTOCOL = "WinNT://";
        public const string DOMAIN_SEARCH_PROTOCOL = "LDAP://";

        public const string DOMAIN_ACCOUNT_NAME_PROPERTY = "samAccountName";
        public const string LOCAL_ACCOUNT_NAME_PROPERTY = "name";

        public const string ALL_GROUPS_SEARCH_FILTER = "(objectclass=group)";

        private static Dictionary<SecurityIdentifier, string> m_accountsCache =
            new Dictionary<SecurityIdentifier, string>();

        public static bool IsLocalMachine(string machineName)
        {
            return
                !StringUtil.IsStringInitialized(machineName) ||
                StringUtil.CompareIgnoreCase(machineName, Environment.MachineName) ||
                StringUtil.CompareIgnoreCase(machineName, ".");
        }

        public static bool IsLocalMachine(DirectoryEntry de)
        {
            return de.Path.StartsWith(LOCAL_SEARCH_PROTOCOL);
        }

        public static bool ParseAccountName(string fullAccountName, out string serverName, out string userName)
        {
            serverName = String.Empty;
            userName = String.Empty;

            if (StringUtil.IsStringInitialized(fullAccountName))
            {
                string[] parts = fullAccountName.Split(new char[] { '\\' });

                if (parts.Length == 2)
                {
                    serverName = parts[0];
                    userName   = parts[1];
                }
                else
                {
                    serverName = ".";
                    userName   = parts[0];
                }

                return true;
            }
            else
            {
                Log.WriteWarning("Full account name is invalid.");

                return false;
            }
        }

        public static string GetObjectAccountName(DirectoryEntry de, string nameProperty)
        {
            return GetObjectAccountName((IADs)de.NativeObject, nameProperty);
        }

        public static string GetObjectAccountName(IADs nativeObject, string nameProperty)
        {
            string objectAccountName = String.Empty;

            SecurityIdentifier objectSid = null;

            try
            {
                objectSid = new SecurityIdentifier((byte[])nativeObject.Get("objectSid"), 0);

                if (objectSid != null)
                {
                    NTAccount objectAccount;

                    if (!m_accountsCache.TryGetValue(objectSid, out objectAccountName))
                    {
                        objectAccount = (NTAccount)objectSid.Translate(typeof(NTAccount));

                        if (objectAccount != null)
                        {
                            objectAccountName = objectAccount.Value;
                        }

                        m_accountsCache.Add(objectSid, objectAccountName);
                    }
                }
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);

                if (objectSid != null)
                {
                    m_accountsCache.Add(objectSid, objectAccountName);
                }
            }

            if (!StringUtil.IsStringInitialized(objectAccountName))
            {
                if (!StringUtil.IsStringInitialized(nameProperty))
                {
                    nameProperty = LOCAL_ACCOUNT_NAME_PROPERTY;
                }

                objectAccountName = (string)nativeObject.Get(nameProperty);
            }

            return objectAccountName;
        }

        public static IADs GetDyrectoryNativeObject(object searchResult)
        {
            if (searchResult is DirectoryEntry)
            {
                return (IADs)(((DirectoryEntry)searchResult).NativeObject);
            }
            else
            {
                if (searchResult is SearchResult)
                {
                    return (IADs)(((SearchResult)searchResult).GetDirectoryEntry().NativeObject);
                }
            }

            return null;
        }
    }
}
