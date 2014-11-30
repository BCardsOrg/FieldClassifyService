using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Customizations;

namespace TiS.Core.TisCommon.Validation
{
    [ComVisible(false)]
    public enum ValidationShowDetailedInfo { WHEN_FAILED, ALWAYS };

    [ComVisible(false)]
    public enum TIS_VALIDATION_ERR
    {
        TIS_VALIDATION_OK = 0,
        TIS_VALIDATION_TYPE,
        TIS_VALIDATION_ERR_REJECT,
        TIS_VALIDATION_ERR_REQUIRED,
        TIS_VALIDATION_ERR_EDIT_MASK,
        TIS_VALIDATION_ERR_MIN_LENGTH,
        TIS_VALIDATION_ERR_MAX_LENGTH,
        TIS_VALIDATION_ERR_NUM_DIGITS,
        TIS_VALIDATION_ERR_DATE,
        TIS_VALIDATION_ERR_TIME,
        TIS_VALIDATION_ERR_MIN_VALUE,
        TIS_VALIDATION_ERR_MAX_VALUE,
        TIS_VALIDATION_ERR_QUICK_LOOKUP,
        TIS_VALIDATION_ERR_LOOKUP,
        TIS_VALIDATION_ERR_REGEX,
        TIS_VALIDATION_ERR_WRONG_OBJECT,
        TIS_VALIDATION_ERR_NOT_IMPLEMENTED,
        TIS_VALIDATION_FAILED,
        TIS_VALIDATION_DIC_ERROR_IO = 256,
        TIS_VALIDATION_DIC_ERROR_DATA = 512,
        TIS_VALIDATION_DIC_ERROR_MEMORY = 768,
        TIS_VALIDATION_DIC_ITEM_NOT_FOUND = 1024,
        TIS_VALIDATION_DIC_NOT_LOADED = 1280,
        TIS_VALIDATION_DIC_RET_DLL_BAD = 1792
    };

    [ComVisible(false)]
    public enum ValidationStatus { Unknown, Invalid, Valid };
    [ComVisible(false)]
    public enum ValidationRunPolicy { Sync, Async };
    [ComVisible(false)]
    public enum ValidationStopPolicy { FirstInvalid, Never };
    [ComVisible(false)]
    public enum ValidationResultPolicy { ConsiderAll, ConsiderValidatedOnly };

    [ComVisible(false)]
    public delegate bool ConfirmValidationMethodDelegate(TisValidationMethod validationMethod);
    [ComVisible(false)]
    public delegate object[] ValidationMethodParametersMissedDelegate(TisValidationMethod validationMethod);

    [ComVisible(false)]
    public interface ITisSupportValidation
    {
    }

    [ComVisible(false)]
    public interface ITisValidationProvider
    {
        Type ValidationProviderData { get;}
        Type ValidationSourceProviderData { get;}
    }

    [ComVisible(false)]
    public interface ITisValidationMethodParametersProvider
    {
        object[] ObtainValidationMethodParameters(
            TisValidationMethod validationMethod, 
            object validationTarget, 
            ref bool Handled);
    }

    [ComVisible(false)]
    public interface ITisValidationMethodConfirmatory
    {
        bool ConfirmValidationMethod(
            TisValidationMethod validationMethod, 
            ref bool handled);
    }

    [ComVisible(false)]
    public interface ITisValidationContext
    {
        Type[] ValidationTypes { get; }

        ITisValidationMethodParametersProvider ValidationMethodParametersProvider { get; }
        ITisValidationMethodConfirmatory ValidationMethodConfirmatory { get; }
    }
}
