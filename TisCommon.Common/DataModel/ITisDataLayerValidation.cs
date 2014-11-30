using TiS.Core.TisCommon.Validation;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public interface ITisDataLayerValidation
    {
        TisValidationPolicy ValidationPolicy { get; }
        ValidationStatus Validate(TisValidationPolicy validationPolicy);

        bool HasValidator { get; }
		TisValidationsResult LastValidationsResult { get; }
    }
}
