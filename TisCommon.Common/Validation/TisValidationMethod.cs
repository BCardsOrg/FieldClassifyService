using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using TiS.Core.TisCommon.Customizations;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationMethod

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisValidationMethod : IComparable
    {
        [DataMember]
        private string m_name;
        [DataMember]
        private string m_fullName;
        [DataMember]
        private bool m_isCustomCodeProvider;
        [DataMember]
        private int m_methodOrder;
        [IgnoreDataMember]
        private MethodInfo m_method;
        [IgnoreDataMember]
        private ITisMethodSignature m_oMethodSignature;

        public TisValidationMethod()
        {
        }

        public TisValidationMethod(MethodInfo method, int methodOrder, bool isCustomCodeProvider)
        {
            m_name = method.Name;

            if (method.DeclaringType != null)
            {
                m_fullName = method.DeclaringType.FullName + Type.Delimiter + m_name;
            }
            else
            {
                m_fullName = m_name;
            }

            m_isCustomCodeProvider = isCustomCodeProvider;

            m_method = method;
            m_methodOrder = methodOrder;

            m_oMethodSignature = 
                new MethodSignature(m_method.GetParameters(), m_method.ReturnType);
        }

        #region ITisValidationParams Members

        public string Name
        {
            get 
            {
                return m_name;
            }
        }

        public string FullName
        {
            get
            {
                return m_fullName;
            }
        }

        public int Order
        {
            get 
            {
                return m_methodOrder;
            }
        }

        public bool IsCustomCodeProvider
        {
            get
            {
                return m_isCustomCodeProvider;
            }
        }

        public MethodInfo Method
        {
            get 
            {
                return m_method;
            }
            set
            {
                m_method = value;
            }
        }

        public ITisMethodSignature MethodSignature
        {
            get 
            {
                return m_oMethodSignature;
            }
            set
            {
                m_oMethodSignature = value;
            }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (this.Order > ((TisValidationMethod)obj).Order)
            {
                return 1;
            }
            else
            {
                if (this.Order < ((TisValidationMethod)obj).Order)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion
    }

    #endregion
}
