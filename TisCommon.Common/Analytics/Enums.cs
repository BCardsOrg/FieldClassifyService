using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TiS.Core.TisCommon.Analytics
{
    [ComVisible(false)]
    public enum ApplicationType
    {
        Station,
        Tool,
        Server,
        Controller,
        Completion
    }
}
