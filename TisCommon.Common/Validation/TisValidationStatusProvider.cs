using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Validation
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationStatusProvider
    {
        private ValidationStatus m_status;
        private TisValidationMethodDetailedInfo m_detailedInfo;

        public TisValidationStatusProvider(
            ValidationStatus validationStatus,
            string message,
            string details,
            ValidationShowDetailedInfo showDetailedInfo)
        {
            m_status = validationStatus;

            m_detailedInfo =
                new TisValidationMethodDetailedInfo(message, details, showDetailedInfo);
        }

        public TisValidationStatusProvider(
            ValidationStatus validationStatus,
            string message,
            string details) : this(validationStatus, message, details, ValidationShowDetailedInfo.WHEN_FAILED)
        {
        }

        public TisValidationStatusProvider(
            ValidationStatus validationStatus,
            string details) : this(validationStatus, String.Empty, details)
        {
        }

        public TisValidationStatusProvider(
            ValidationStatus validationStatus)
            : this(validationStatus, String.Empty, null)
        {
        }

        #region ITisValidationStatusProvider Members

        public TisValidationMethodDetailedInfo DetailedInfo
        {
            get
            {
                return m_detailedInfo;
            }
        }

        public ValidationStatus Status
        {
            get
            {
                return m_status;
            }
        }

        #endregion
    }
}
