using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationsResultFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class TisValidationsResultFilter
    {
        private bool m_isInclusive = true;

        #region TisValidationsResultFilter Members

        public virtual bool Accepts(
            TisValidationMethod validationMethod, 
            TisValidationMethodResult validationMethodResult)
        {
            if (m_isInclusive)
            {
                return Matches(validationMethod, validationMethodResult);
            }
            else
            {
                return !Matches(validationMethod, validationMethodResult);
            }
        }

        public virtual bool IsInclusive
        {
            get
            {
                return m_isInclusive;
            }
            set
            {
                m_isInclusive = value;
            }
        }

        #endregion

        protected virtual bool Matches(
            TisValidationMethod validationMethod,
            TisValidationMethodResult validationMethodResult)
        {
            return true;
        }
    }

    #endregion

    #region TisValidationsResultCompositeFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultCompositeFilter : TisValidationsResultFilter
    {
        private List<TisValidationsResultFilter> m_filters;

        public TisValidationsResultCompositeFilter(TisValidationsResultFilter[] filters)
        {
            m_filters = new List<TisValidationsResultFilter>(filters);
        }

        public List<TisValidationsResultFilter> Filters
        {
            get
            {
                return m_filters;
            }
        }

        public override bool Accepts(
            TisValidationMethod validationMethod, 
            TisValidationMethodResult validationMethodResult)
        {
            if (IsInclusive)
            {
                foreach (TisValidationsResultFilter filter in m_filters)
                {
                    if (!filter.Accepts(validationMethod, validationMethodResult))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                foreach (TisValidationsResultFilter filter in m_filters)
                {
                    if (filter.Accepts(validationMethod, validationMethodResult))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    #endregion

    #region TisValidationsResultBaseFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultBaseFilter : TisValidationsResultFilter
    {
        private TisValidationsResultCompositeFilter m_compositeFilter =
            new TisValidationsResultCompositeFilter(new TisValidationsResultFilter[] { });

        protected virtual TisValidationsResultCompositeFilter CompositeFilter
        {
            get
            {
                return m_compositeFilter;
            }
        }
    }

    #endregion

    #region TisValidationsResultMethodNameFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodNameFilter : TisValidationsResultFilter
    {
        protected string m_methodName;

        public TisValidationsResultMethodNameFilter(string methodName)
        {
            m_methodName = methodName;
        }

        protected override bool Matches(
            TisValidationMethod validationMethod, 
            TisValidationMethodResult validationMethodResult)
        {
            return StringUtil.CompareIgnoreCase(validationMethod.Name, m_methodName);
        }
    }

    #endregion

    #region TisValidationsResultMethodFullNameFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodFullNameFilter : TisValidationsResultMethodNameFilter
    {
        public TisValidationsResultMethodFullNameFilter(string methodFullName)
            : base(methodFullName)
        {
        }

        protected override bool Matches(
            TisValidationMethod validationMethod,
            TisValidationMethodResult validationMethodResult)
        {
            return StringUtil.CompareIgnoreCase(validationMethod.FullName, m_methodName);
        }
    }

    #endregion

    #region TisValidationsResultCustomCodeFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultCustomCodeProviderFilter : TisValidationsResultFilter
    {
        protected override bool Matches(
            TisValidationMethod validationMethod, 
            TisValidationMethodResult validationMethodResult)
        {
            return validationMethod.IsCustomCodeProvider;
        }
    }

    #endregion

    #region TisValidationsResultMethodStatusFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodStatusFilter : TisValidationsResultFilter
    {
        private List<ValidationStatus> m_statusToFilter;

        public TisValidationsResultMethodStatusFilter(ValidationStatus[] statusToFilter)
        {
            m_statusToFilter = new List<ValidationStatus>(statusToFilter);
        }

        public ValidationStatus[] StatusToFilter
        {
            get
            {
                return m_statusToFilter.ToArray();
            }
        }

        protected override bool Matches(
            TisValidationMethod validationMethod,
            TisValidationMethodResult validationMethodResult)
        {
            return m_statusToFilter.Contains(validationMethodResult.Status);
        }
    }

    #endregion

    #region TisValidationsResultMethodParametersBaseFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodParametersBaseFilter : TisValidationsResultFilter
    {
        private List<object> m_methodParametersArray;
        private TisValidationMethod m_validationMethodToCheck;

        public TisValidationsResultMethodParametersBaseFilter(object[] methodParametersArray)
        {
            m_methodParametersArray = new List<object>(methodParametersArray);
        }

        public TisValidationsResultMethodParametersBaseFilter(ParameterInfo[] methodParametersArray)
            : this((object[])methodParametersArray)
        {
        }

        public List<object> MethodParametersArray
        {
            get
            {
                return m_methodParametersArray;
            }
        }

        public TisValidationMethod ValidationMethodToCheck
        {
            get 
            { 
                return m_validationMethodToCheck; 
            }
        }

        protected override bool Matches(
            TisValidationMethod validationMethod,
            TisValidationMethodResult validationMethodResult)
        {
            m_validationMethodToCheck = validationMethod;

            if (m_methodParametersArray.Count != validationMethodResult.Parameters.Length)
            {
                return false;
            }

            for (int i = 0; i < m_methodParametersArray.Count; i++)
            {
                object parameterToCheck = m_methodParametersArray[i];

                object parameterData = validationMethodResult.Parameters[i];

                if (parameterData != null)
                {
                    parameterToCheck = parameterData.GetType();
                }

                if (!Match(parameterToCheck, i))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual bool Match(object parameterToCheck, int index)
        {
            return parameterToCheck == null;
        }
    }

    #endregion

    #region TisValidationsResultMethodParametersNamesFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodParametersNamesFilter : TisValidationsResultMethodParametersBaseFilter
    {
        public TisValidationsResultMethodParametersNamesFilter(string[] methodParametersNames)
            : base(methodParametersNames)
        {
        }

        public List<string> MethodParametersNames
        {
            get
            {
                return MethodParametersArray.ConvertAll<string>(Converter);
            }
        }

        protected override bool Match(object parameterToCheck, int index)
        {
            if (base.Match(parameterToCheck, index))
            {
                return true;
            }

            if (index > -1 && index < ValidationMethodToCheck.MethodSignature.ParamNames.Length)
            {
                string paramName = ValidationMethodToCheck.MethodSignature.ParamNames[index];

                if (StringUtil.CompareIgnoreCase(paramName, (string)parameterToCheck))
                {
                    return true;
                }
            }

            return false;
        }

        private string Converter(object obj)
        {
            return (string)obj;
        }
    }

    #endregion

    #region TisValidationsResultMethodParametersTypesFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodParametersTypesFilter : TisValidationsResultMethodParametersBaseFilter
    {
        private bool m_considerAssignableTypes;

        public TisValidationsResultMethodParametersTypesFilter(
            Type[] methodParametersTypes) : this(methodParametersTypes, true)
        {
        }

        public TisValidationsResultMethodParametersTypesFilter(
            ParameterInfo[] methodParametersTypes)
            : this(methodParametersTypes, true)
        {
        }

        public TisValidationsResultMethodParametersTypesFilter(
            Type[] methodParametersTypes,
            bool considerAssignableTypes) : base(methodParametersTypes)
        {
            m_considerAssignableTypes = considerAssignableTypes;
        }

        public TisValidationsResultMethodParametersTypesFilter(
            ParameterInfo[] methodParametersTypes,
            bool considerAssignableTypes)
            : base(methodParametersTypes)
        {
            m_considerAssignableTypes = considerAssignableTypes;
        }

        public List<Type> MethodParametersTypes
        {
            get
            {
                return MethodParametersArray.ConvertAll<Type>(Converter);
            }
        }

        protected override bool Match(object parameterToCheck, int index)
        {
            Type parameterToCheckAsType;

            if (base.Match(parameterToCheck, index))
            {
                return true;
            }

            if (index > -1 && index < ValidationMethodToCheck.MethodSignature.ParamTypes.Length)
            {
                Type type = ValidationMethodToCheck.MethodSignature.ParamTypes[index];

                if (parameterToCheck is ParameterInfo)
                {
                    parameterToCheckAsType = ((ParameterInfo)parameterToCheck).ParameterType;
                }
                else
                {
                    parameterToCheckAsType = (Type)parameterToCheck;
                }

                if (type == parameterToCheckAsType)
                {
                    return true;
                }
                else
                {
                    if (type.IsByRef)
                    {
                        return type == parameterToCheckAsType.MakeByRefType();
                    }
                    else
                    {
                        if (type.IsInterface)
                        {
                            return parameterToCheckAsType.GetInterface(type.Name, true) != null;
                        }
                        else
                        {
                            if (m_considerAssignableTypes)
                            {
                                return type.IsAssignableFrom(parameterToCheckAsType);
                            }
                        }
                    }
                }
            }

            return false;
        }

        private Type Converter(object obj)
        {
            if (obj is ParameterInfo)
            {
                return ((ParameterInfo)obj).ParameterType;
            }
            else
            {
                return (Type)obj;
            }
        }
    }

    #endregion

    #region TisValidationsResultMethodParametersFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodParametersFilter : TisValidationsResultBaseFilter
    {
        private TisValidationsResultMethodParametersNamesFilter m_parameterNamesFilter;
        private TisValidationsResultMethodParametersTypesFilter m_parameterTypesFilter;

        public TisValidationsResultMethodParametersFilter(            
            string[] parameterNames,
            Type[] parameterTypes) : this(new TisValidationsResultMethodParametersNamesFilter(parameterNames),
                                          new TisValidationsResultMethodParametersTypesFilter(parameterTypes))
        {
        }

        public TisValidationsResultMethodParametersFilter(
            TisValidationsResultFilter parameterNamesFilter,
            TisValidationsResultFilter parameterTypesFilter)
        {
            m_parameterNamesFilter = (TisValidationsResultMethodParametersNamesFilter)parameterNamesFilter;
            m_parameterTypesFilter = (TisValidationsResultMethodParametersTypesFilter)parameterTypesFilter;

            CompositeFilter.Filters.AddRange(new TisValidationsResultFilter[] { parameterNamesFilter,
                                                                                 parameterTypesFilter });
        }

        public List<string> ParameterNames
        {
            get
            {
                return m_parameterNamesFilter.MethodParametersNames;
            }
        }

        public List<Type> ParameterTypes
        {
            get
            {
                return m_parameterTypesFilter.MethodParametersTypes;
            }
        }

        protected override bool Matches(
            TisValidationMethod validationMethod,
            TisValidationMethodResult validationMethodResult)
        {
            return CompositeFilter.Accepts(validationMethod, validationMethodResult);
        }
    }

    #endregion

    #region TisValidationsResultMethodParametersValuesFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class TisValidationsResultMethodParametersValuesFilter : TisValidationsResultMethodParametersFilter
    {
        private List<object> m_parameterValues;

        public TisValidationsResultMethodParametersValuesFilter(
            string[] parameterNames,
            Type[] parameterTypes,
            object[] parameterValues) : this(new TisValidationsResultMethodParametersNamesFilter(parameterNames), 
                                             new TisValidationsResultMethodParametersTypesFilter(parameterTypes),
                                             parameterValues)
        {
        }

        public TisValidationsResultMethodParametersValuesFilter(
            TisValidationsResultFilter parameterNamesFilter,
            TisValidationsResultFilter parameterTypesFilter,
            object[] parameterValues)
            : base(parameterNamesFilter, parameterTypesFilter)
        {
            m_parameterValues = new List<object>(parameterValues);
        }

        public List<object> ParameterValues
        {
            get 
            { 
                return m_parameterValues; 
            }
        }

        protected override bool Matches(
            TisValidationMethod validationMethod, 
            TisValidationMethodResult validationMethodResult)
        {
            if (base.Matches(validationMethod, validationMethodResult))
            {
                if (m_parameterValues.Count == validationMethodResult.Parameters.Length)
                {
                    for (int i = 0; i < validationMethodResult.Parameters.Length; i++)
                    {
                        if (m_parameterValues[i] != null && 
                            !validationMethodResult.Parameters[i].Equals(m_parameterValues[i]))
                        {
                            return false;
                        }
                    }
                }

                return true;

            }

            return false;
        }
    }

    #endregion
}
