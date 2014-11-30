using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Validation
{
    public class TisValidationProviderMngr : TisApplicationServicesProvider
    {
        public TisValidationProviderMngr(
            string sApplicationName,
            ITisServicesHost oServicesHost)
            : base(oServicesHost, sApplicationName)
        {
        }

        #region ITisValidationProviderMngr Members

        public virtual ITisValidationProvider GetValidationProvider(string sKey)
        {
            return GetServiceInfo(sKey) as ITisValidationProvider;
        }

        #endregion
    }
}
