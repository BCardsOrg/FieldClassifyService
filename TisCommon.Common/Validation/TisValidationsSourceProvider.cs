using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationsSourceProvider

    public abstract class TisValidationsSourceProvider
    {
        protected TisInternalValidationManager m_validationManager;

        public TisValidationsSourceProvider(
            TisInternalValidationManager validationManager)
        {
            m_validationManager = validationManager;
        }

        #region ITisValidationSourceProvider Members

        public abstract TisValidationsSource GetValidationsSource(ITisValidationProvider validationsProvider);

        #endregion
    }

    #endregion

    #region TisValidationsServiceSourceProvider

    internal class TisValidationsServiceSourceProvider : TisValidationsSourceProvider
    {
        public TisValidationsServiceSourceProvider(
            TisInternalValidationManager validationManager) : base(validationManager)
        {
        }

        public override TisValidationsSource GetValidationsSource(ITisValidationProvider validationsProvider)
        {
            if (validationsProvider is ITisServiceInfo)
            {
                return new TisValidationsSource(
                    m_validationManager.GetService((validationsProvider as ITisServiceInfo).ServiceName));
            }
            else
            {
                Log.WriteWarning("Validations provider {0} is not eFlow service", validationsProvider.GetType().FullName);

                return null;
            }
        }
    }

    #endregion
}
