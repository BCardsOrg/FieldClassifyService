using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Validation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [ComVisible(false)]
    public class ValidationMethodAttribute : Attribute
    {
        private int m_validationOrder;
        private bool m_isCustomCodeProvider;

        public ValidationMethodAttribute():this(1, false)
        {
        }

        public ValidationMethodAttribute(int validationOrder) : this(validationOrder, false)
        {
            m_validationOrder = validationOrder;
        }

        public ValidationMethodAttribute(int validationOrder, bool isCustomCodeProvider)
        {
            m_validationOrder = validationOrder;
            m_isCustomCodeProvider = isCustomCodeProvider;
        }

        public int ValidationOrder
        {
            get
            {
                return m_validationOrder;
            }
        }

        public bool IsCustomCodeProvider
        {
            get
            {
                return m_isCustomCodeProvider;
            }
        }
    }
}
