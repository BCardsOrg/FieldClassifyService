using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Security
{
    public class ImpersonationSesssion : IDisposable
    {
        private WindowsImpersonationContext m_impersonationContext;
        private Impersonator m_impersonator;

        public ImpersonationSesssion()
        {
            m_impersonator = new Impersonator();
        }

        public bool ImpersonateUser(string domain, string userName, string userPassword)
        {
            if (RevertToSelf())
            {
                m_impersonationContext = m_impersonator.ImpersonateUser(domain, userName, userPassword);

                return m_impersonationContext != null;
            }
            else
            {
                return false;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_impersonationContext != null)
            {
                m_impersonationContext.Dispose();

                m_impersonationContext = null;
            }
        }

        #endregion

        #region Impersonator

        private class Impersonator
        {
            private const int LOGON32_LOGON_INTERACTIVE = 2;
            private const int LOGON32_PROVIDER_DEFAULT = 0;

            public WindowsImpersonationContext ImpersonateUser(string domain, string userName, string userPassword)
            {
                WindowsIdentity tempWindowsIdentity;
                IntPtr token = IntPtr.Zero;
                IntPtr tokenDuplicate = IntPtr.Zero;

                try
                {
                    if (!StringUtil.IsStringInitialized(domain))
                    {
                        // local machine
                        domain = "."; 
                    }

                    if (LogonUserA(userName, domain, userPassword, LOGON32_LOGON_INTERACTIVE,
                        LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);

                            return tempWindowsIdentity.Impersonate();
                        }
                    }

                    return null;
                }
                finally
                {
                    if (token != IntPtr.Zero)
                    {
                        CloseHandle(token);
                    }

                    if (tokenDuplicate != IntPtr.Zero)
                    {
                        CloseHandle(tokenDuplicate);
                    }
                }
            }

            #region PInvoke

            [DllImport("advapi32.dll")]
            private static extern int LogonUserA(String lpszUserName,
                String lpszDomain,
                String lpszPassword,
                int dwLogonType,
                int dwLogonProvider,
                ref IntPtr phToken);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern int DuplicateToken(IntPtr hToken,
                int impersonationLevel,
                ref IntPtr hNewToken);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern bool CloseHandle(IntPtr handle);

            #endregion
        }

        #endregion

        #region PInvoke

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        #endregion
    }
}
