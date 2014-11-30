using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public class EntityParentInfo
    {
        FieldInfo m_oParent;
        bool m_bMandatory;

        public EntityParentInfo(FieldInfo oParent, bool bMandatory)
        {
            m_oParent = oParent;
            m_bMandatory = bMandatory;
        }

        public bool Mandatory
        {
            get { return m_bMandatory; }
        }

        public FieldInfo Parent
        {
            get { return m_oParent; }
        }
    }
}
