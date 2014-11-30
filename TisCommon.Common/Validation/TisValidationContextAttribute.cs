using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [ComVisible(false)]
    public class TisValidationContextAttribute : Attribute
    {
        private Type m_validationType;

        public TisValidationContextAttribute(string validationTypeName)
            : this(Type.GetType(validationTypeName))
        {
        }

        public TisValidationContextAttribute(Type validationType)
        {
            m_validationType = validationType;
        }

        public Type ValidationType
        {
            get
            {
                return m_validationType;
            }
        }
    }
}
