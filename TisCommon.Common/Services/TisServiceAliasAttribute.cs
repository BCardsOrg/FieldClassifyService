using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Services
{
    #region ServiceAliasAttribute

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [ComVisible(false)]
    public class TisServiceAliasAttribute : Attribute
    {
        private string m_serviceAlias;

        public TisServiceAliasAttribute(string serviceAlias)
        {
            m_serviceAlias = serviceAlias;
        }

        public string ServiceAlias
        {
            get
            {
                return m_serviceAlias;
            }
        }
    }

    #endregion
}
