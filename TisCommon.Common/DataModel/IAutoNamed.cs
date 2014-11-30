using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [Guid("e1d9d409-d7a2-46e3-9d4b-43dd905b5ace")]
    public interface IAutoNamed
    {
        void UpdateName();
    }
}
