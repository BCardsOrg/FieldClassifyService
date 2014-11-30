using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.DataModel
{
    [Guid("7BD763E6-726D-400F-978B-BA00C65ACFE6")]
    public interface INamedObject
    {
        string Name { get; }
    }
}
