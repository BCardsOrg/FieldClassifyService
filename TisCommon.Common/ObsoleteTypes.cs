using System;
using System.Collections.Generic;
using System.Text;

namespace TiS.Core.TisCommon
{
    internal class ObsoleteTypes
    {
        private Dictionary<string, string> m_obsoleteTypesResolver = new Dictionary<string, string>();

        public ObsoleteTypes()
        {
            m_obsoleteTypesResolver.Add(
                "TiS.Core.PlatformRuntime.Security.SafeArrayList, TiS.Core.PlatformRuntime",
                "TiS.Core.TisCommon.SafeArrayList, TiS.Core.TisCommon");

            m_obsoleteTypesResolver.Add(
                "TiS.Core.PlatformRuntime.Security.SafeHashtable, TiS.Core.PlatformRuntime",
                "TiS.Core.TisCommon.SafeHashtable, TiS.Core.TisCommon");
        }

        public bool ObtainContemporaryTypeName(string obsoleteTypeName, out string contemporaryTypeName)
        {
            return m_obsoleteTypesResolver.TryGetValue(obsoleteTypeName, out contemporaryTypeName);
        }
    }
}
