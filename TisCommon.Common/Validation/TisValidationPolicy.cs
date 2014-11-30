using System;
using System.Collections.Generic;
using System.Text;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationPolicy

    public class TisValidationPolicy
    {
        private ValidationRunPolicy m_runPolicy = ValidationRunPolicy.Sync;
        private ValidationStopPolicy m_stopPolicy = ValidationStopPolicy.FirstInvalid;
        private ValidationResultPolicy m_resultPolicy = ValidationResultPolicy.ConsiderValidatedOnly;

        #region ITisValidationPolicy Members

        public ValidationRunPolicy RunPolicy
        {
            get
            {
                return m_runPolicy;
            }
            set
            {
                m_runPolicy = value;
            }
        }

        public ValidationStopPolicy StopPolicy
        {
            get
            {
                return m_stopPolicy;
            }
            set
            {
                m_stopPolicy = value;
            }
        }

        public ValidationResultPolicy ResultPolicy
        {
            get
            {
                return m_resultPolicy;
            }
            set
            {
                m_resultPolicy = value;
            }
        }

        #endregion
    }

    #endregion
}
