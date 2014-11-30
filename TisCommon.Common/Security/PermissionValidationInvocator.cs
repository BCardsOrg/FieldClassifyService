using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Security
{
    [ComVisible(false)]
    public delegate bool PermissionValidatorDelegate(string permissionName);

    #region PermissionValidationInvocationContext

    [ComVisible(false)]
    public class PermissionValidationInvocationContext
    {
        private PermissionValidatorDelegate m_validatorDelegate;
        private string m_permissionName;

        public PermissionValidationInvocationContext(
            PermissionValidatorDelegate validatorDelegate,
            string permissionName)
        {
            m_validatorDelegate = validatorDelegate;
            m_permissionName = permissionName;
        }

        public PermissionValidatorDelegate ValidatorDelegate
        {
            get
            {
                return m_validatorDelegate;
            }
        }

        public string PermissionName
        {
            get
            {
                return m_permissionName;
            }
        }
    }

    #endregion

    #region PermissionValidationInvocator

    [ComVisible(false)]
    public class PermissionValidationInvocator
    {
        private List<PermissionValidationInvocationContext> m_validationContexts = 
            new List<PermissionValidationInvocationContext>();

        public PermissionValidationInvocator(
            PermissionValidatorDelegate ValidatorDelegate,
            string permissionName)
            : this(new PermissionValidationInvocationContext(ValidatorDelegate, permissionName))
        {
        }

        public PermissionValidationInvocator(
            PermissionValidationInvocationContext validationContexts)
            : this(new PermissionValidationInvocationContext[] { validationContexts })
        {
        }
        public PermissionValidationInvocator(PermissionValidationInvocationContext[] validationContexts)
        {
            if (validationContexts != null)
            {
                m_validationContexts.AddRange(validationContexts);
            }
        }

        public bool Validate()
        {
            if (m_validationContexts != null)
            {
                foreach (PermissionValidationInvocationContext validationContext in m_validationContexts)
                {
                    if (validationContext.ValidatorDelegate(validationContext.PermissionName))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }
    }

    #endregion
}
