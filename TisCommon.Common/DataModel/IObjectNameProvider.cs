using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public interface IObjectNameProvider
    {
        string GetObjectName(object oObj);
    }
}
