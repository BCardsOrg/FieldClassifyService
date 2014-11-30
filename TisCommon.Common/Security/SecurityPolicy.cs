using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Security
{
    [Serializable]
    public class SecurityPolicy
    {
        private const TisSecurityPolicy DefaultSecurityPolicy = TisSecurityPolicy.Permissive;

        private TisSecurityPolicy m_SecurityPolicy = DefaultSecurityPolicy;

        public TisSecurityPolicy CurrentSecurityPolicy
        {
            get { return m_SecurityPolicy; }
        }

    }
}
