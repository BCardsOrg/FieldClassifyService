using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.DataModel
{
    public interface INamedObjectFactory
    {
        INamedObject CreateObject(Type oType, string sName);
    }
}
