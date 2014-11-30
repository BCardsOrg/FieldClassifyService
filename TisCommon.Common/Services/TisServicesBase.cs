using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon.Security;

namespace TiS.Core.TisCommon.Services
{
    public class TisServicesBase : IDisposable
    {
        public virtual void Initialize()
        {
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region Services

        protected ITisServicesHost ServicesHost { get; private set; }

        public void SetServicesHost(ITisServicesHost servicesHost)
        {
            ServicesHost = servicesHost;
        }

        #endregion

        #region Security

        protected ITisServiceSecurityCheck ServiceSecurityCheck { get; private set; }

        public void SetServiceSecurity(ITisServiceSecurityCheck securityCheck)
        {
            ServiceSecurityCheck = securityCheck;
        }

        protected void ValidateManagementPermission()
        {
            if (!HasManagementPermission())
            {
                throw new TisException("No sufficient management permission");
            }
        }

        protected bool HasManagementPermission()
        {
            return HasPermission(PermissionsConst.TIS_PERMISSION_WRITE);
        }

        protected void ValidatePermission(string sPermissionName)
        {
            if (!HasPermission(sPermissionName))
            {
                throw new TisException("No required \"{0}\" permission", sPermissionName);
            }
        }

        protected bool HasPermission(string sPermissionName)
        {
            return (ServiceSecurityCheck == null ||
                    ServiceSecurityCheck.HasPermission(sPermissionName));
        }

        #endregion
    }
}
