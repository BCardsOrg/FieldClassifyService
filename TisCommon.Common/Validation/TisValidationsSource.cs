using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Validation
{
    public class TisValidationsSource
    {
        private Type m_validationType;
        private object m_source;
        private int m_order = 1;

        public TisValidationsSource(object source)
        {
            m_source = source;

            if (m_source == null)
            {
                throw new TisException("Validations source does not exist");
            }

            ValidationServiceAttribute validationServiceAttribute =
                (ValidationServiceAttribute)ReflectionUtil.GetAttribute(m_source.GetType(),
                                                                        typeof(ValidationServiceAttribute));

            if (validationServiceAttribute != null)
            {
                m_validationType = validationServiceAttribute.ValidationType;
                m_order = validationServiceAttribute.ValidationOrder;
            }
        }

        #region ITisValidationsSource Members

        public Type ValidationType
        {
            get
            {
                return m_validationType;
            }
        }

        public object Source
        {
            get
            {
                return m_source;
            }
        }

        public int Order
        {
            get
            {
                return m_order;
            }
        }

        #endregion
    }
}
