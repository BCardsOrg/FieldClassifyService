using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Validation
{
    public static class TisValidationUtils
    {
        public static string ValidationErrorToString(this TIS_VALIDATION_ERR validationError)
        {
            string validationErrorName = String.Empty;

            try
            {
                validationErrorName = Enum.GetName(typeof(TIS_VALIDATION_ERR), validationError);
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);
            }

            return validationErrorName;
        }

    }
}
