using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.DataModel
{
    public interface IUsesNamedObjectFactory
    {
        void SetFactory(INamedObjectFactory oFactory);
    }
}
