using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    internal class DefaultObjectNameProvider : IObjectNameProvider
    {
        #region IObjectNameProvider Members

        public string GetObjectName(object oObj)
        {
            INamedObject oNamedObj = oObj as INamedObject;

            if (oNamedObj != null)
            {
                return oNamedObj.Name;
            }

            return oObj.GetType().FullName;
        }

        #endregion
    }
}
