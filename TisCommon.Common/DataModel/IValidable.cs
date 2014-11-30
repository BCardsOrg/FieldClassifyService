using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [Guid("05BAC235-CB2E-43D3-8CCA-A9293C5B50E2")]
    public interface IValidable
    {
        bool IsValid { get; }
    }
}
