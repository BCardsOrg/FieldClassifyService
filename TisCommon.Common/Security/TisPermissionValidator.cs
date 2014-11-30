using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Security
{
    #region TisPermissionValidatorContext

    [ComVisible(false)]
    public class TisPermissionValidatorContext
    {
        public TisPermissionValidatorContext(
            string applicationName,
            ITisServicesHost servicesHost)
        {
            ApplicationName = applicationName;
            ServicesHost = servicesHost;
        }

        public string ApplicationName { get; private set; }
        public ITisServicesHost ServicesHost { get; private set; }

        public ITisSecurityMngr SecurityMngr
        {
            get
            {
                return (ITisSecurityMngr)ServicesHost.GetService(ApplicationName, "SecurityManager");
            }
        }

        public ITisSecurityCheck SecurityCheck
        {
            get
            {
                return SecurityMngr as ITisSecurityCheck;
            }
        }

        public string NodeName
        {
            get
            {
                return ServicesHost.Name;
            }
        }
    }

    #endregion

    #region TisPermissionValidator

    [ComVisible(false)]
    public class TisPermissionValidator
    {
        private TisPermissionValidatorContext m_validatorContext;

        public TisPermissionValidator(TisPermissionValidatorContext validatorContext)
        {
            m_validatorContext = validatorContext;
        }

        public virtual bool CanValidate
        {
            get
            {
                return m_validatorContext != null;
            }
        }

        public virtual bool Validate(string permissionName)
        {
            return false;
        }

        public virtual bool Validate(object securedEntity, string permissionName)
        {
            if (securedEntity == null)
            {
                Log.WriteInfo("Security : Invalid secured entity.");
                return false;
            }

            if (!StringUtil.IsStringInitialized(permissionName))
            {
                Log.WriteWarning("Security : Invalid permission.");
                return false;
            }

            if (!CanValidate)
            {
                return false;
            }

            return m_validatorContext.SecurityCheck.HasPermission(securedEntity, permissionName);
        }

        protected virtual TisPermissionValidatorContext ValidatorContext
        {
            get
            {
                return m_validatorContext;
            }
        }
    }

    #endregion
}
