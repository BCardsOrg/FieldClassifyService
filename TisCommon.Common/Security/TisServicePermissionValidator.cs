using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Security
{
    #region TisServicePermissionValidator

    [ComVisible(false)]
    public class TisServicePermissionValidator : TisPermissionValidator
    {
        private ITisServiceInfo m_callingServiceInfo;

        public TisServicePermissionValidator(
            ITisServiceInfo serviceInfo, 
            TisPermissionValidatorContext validatorContext) :  base(validatorContext)
        {
            m_callingServiceInfo = serviceInfo;
        }

        public override bool Validate(string permissionName)
        {
            if (CanValidate)
            {
                return base.Validate(m_callingServiceInfo, permissionName);
            }
            else
            {
                return false;
            }
        }
    }

    #endregion
}
