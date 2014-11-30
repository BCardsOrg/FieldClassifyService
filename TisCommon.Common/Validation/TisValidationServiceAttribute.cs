using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [ComVisible(false)]
    public class ValidationServiceAttribute : Attribute
    {
        private Type m_validationType;
        private int m_validationOrder;

        public ValidationServiceAttribute(string validationTypeName)
            : this(validationTypeName, 1)
        {
        }

        public ValidationServiceAttribute(Type validationType)
            : this(validationType, 1)
        {
        }

        public ValidationServiceAttribute(string validationTypeName, int validationOrder) : 
            this(Type.GetType(validationTypeName), validationOrder)
        {
        }

        public ValidationServiceAttribute(Type validationType, int validationOrder)
        {
            m_validationType = validationType;
            m_validationOrder = validationOrder;
        }

        public Type ValidationType
        {
            get
            {
                return m_validationType;
            }
        }

        public int ValidationOrder
        {
            get
            {
                return m_validationOrder;
            }
        }
    }
}