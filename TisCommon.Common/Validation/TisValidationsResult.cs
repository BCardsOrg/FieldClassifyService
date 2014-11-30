using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationsResult

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisValidationsResult : IDictionary<TisValidationMethod, TisValidationMethodResult>, IDeserializationCallback
    {
        [DataMember]
        private IDictionary<TisValidationMethod, TisValidationMethodResult> m_dict = new Dictionary<TisValidationMethod, TisValidationMethodResult>();

        [DataMember]
        private ValidationStatus m_finalValidationsStatus = ValidationStatus.Unknown;

        private ReaderWriterLock m_dictLocker = new ReaderWriterLock();

        public TisValidationsResult()
        {
        }

        private TisValidationsResult(TisValidationsResult me)
        {
            m_dict = new Dictionary<TisValidationMethod, TisValidationMethodResult>(me.m_dict);
        }

        public ValidationStatus FinalValidationsStatus
        {
            get
            {
                lock (this)
                {
                    return m_finalValidationsStatus;
                }
            }
        }

        public void Add(TisValidationsResult validationResult)
        {
            foreach (KeyValuePair<TisValidationMethod, TisValidationMethodResult> kpv in validationResult)
            {
                Add(kpv.Key, kpv.Value);
            }
        }

        #region Filters

        public TisValidationsResult FilterBy(TisValidationsResultFilter filter)
        {
            TisValidationsResult finalValidationsResult = new TisValidationsResult();

            foreach (KeyValuePair<TisValidationMethod, TisValidationMethodResult> kpv in this)
            {
                if (filter.Accepts(kpv.Key, kpv.Value))
                {
                    finalValidationsResult.Add(kpv.Key, kpv.Value);
                }
            }

            return finalValidationsResult;
        }

        #region FilterByMethodName

        public TisValidationsResult FilterByMethodName(string methodName)
        {
            TisValidationsResultFilter methodNameFilter =
                new TisValidationsResultMethodNameFilter(methodName);

            return this.FilterBy(methodNameFilter);
        }

        #endregion

        #region FilterByMethodFullName

        public TisValidationsResult FilterByMethodFullName(string methodFullName)
        {
            TisValidationsResultFilter methodFullNameFilter =
                new TisValidationsResultMethodFullNameFilter(methodFullName);

            return this.FilterBy(methodFullNameFilter);
        }

        #endregion

        #region FilterByCustomCodeProviders

        public TisValidationsResult FilterByCustomCodeProviders()
        {
            TisValidationsResultFilter methodCustomCodeProviderFilter =
                new TisValidationsResultCustomCodeProviderFilter();

            return this.FilterBy(methodCustomCodeProviderFilter);
        }

        #endregion

        #region FilterByMethodStatus

        public TisValidationsResult FilterByMethodStatus(ValidationStatus[] statusToFilter)
        {
            TisValidationsResultFilter methodStatusFilter =
                new TisValidationsResultMethodStatusFilter(statusToFilter);

            return this.FilterBy(methodStatusFilter);
        }

        #endregion

        #region FilterByMethodParametersNames

        public TisValidationsResult FilterByMethodParametersNames(
            string[] parameterNames)
        {
            TisValidationsResultFilter methodParametersNamesFilter =
                new TisValidationsResultMethodParametersNamesFilter(parameterNames);

            return this.FilterBy(methodParametersNamesFilter);
        }

        #endregion

        #region FilterByMethodParametersTypes

        public TisValidationsResult FilterByMethodParametersTypes(
            Type[] parameterTypes,
            bool considerAssignableTypes)
        {
            TisValidationsResultFilter methodParametersTypesFilter =
                new TisValidationsResultMethodParametersTypesFilter(parameterTypes, considerAssignableTypes);

            return this.FilterBy(methodParametersTypesFilter);
        }

        #endregion

        #region FilterByMethodParameters

        public TisValidationsResult FilterByMethodParameters(
            string[] parameterNames,
            Type[] parameterTypes)
        {
            TisValidationsResultFilter methodParametersFilter =
                new TisValidationsResultMethodParametersFilter(parameterNames,
                                                               parameterTypes);

            return this.FilterBy(methodParametersFilter);
        }

        #endregion

        #region FilterByMethodParametersValues

        public TisValidationsResult FilterByMethodParametersValues(
            string[] parameterNames,
            Type[] parameterTypes,
            object[] parameterValues)
        {
            TisValidationsResultFilter methodParametersValuesFilter =
                new TisValidationsResultMethodParametersValuesFilter(parameterNames,
                                                                     parameterTypes,
                                                                     parameterValues);

            return this.FilterBy(methodParametersValuesFilter);
        }

        #endregion

        #endregion

        public static ValidationStatus GetFinalValidationStatus(
            TisValidationsResult results,
            ValidationResultPolicy resultPolicy)
        {
            ValidationStatus finalValidationsStatus =
                GetFinalValidationResult(results.Values, resultPolicy);

            ((TisValidationsResult)results).SetFinalValidationsStatus(finalValidationsStatus);

            return finalValidationsStatus;
        }

        private static ValidationStatus GetFinalValidationResult(
            ICollection<TisValidationMethodResult> results,
            ValidationResultPolicy resultPolicy)
        {
            if (results.Count == 0 && resultPolicy == ValidationResultPolicy.ConsiderAll)
            {
                return ValidationStatus.Unknown;
            }

            foreach (TisValidationMethodResult result in results)
            {
                switch ((ValidationStatus)result.Status)
                {
                    case ValidationStatus.Invalid:
                        return ValidationStatus.Invalid;

                    case ValidationStatus.Unknown:
                        if (resultPolicy == ValidationResultPolicy.ConsiderAll)
                        {
                            return ValidationStatus.Unknown;
                        }
                        break;

                    case ValidationStatus.Valid:
                        break;
                }
            }

            return ValidationStatus.Valid;
        }

        internal void SetFinalValidationsStatus(ValidationStatus finalValidationsStatus)
        {
            lock (this)
            {
                m_finalValidationsStatus = finalValidationsStatus;
            }
        }

        #region IDictionary
        public void Add(TisValidationMethod key, TisValidationMethodResult value)
        {
            using (RWLockWriteSession lockSession = new RWLockWriteSession(m_dictLocker))
            {
                m_dict.Add(key, value);
            }
        }

        public bool ContainsKey(TisValidationMethod key)
        {
            using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
            {
                return m_dict.ContainsKey(key);
            }
        }

        public ICollection<TisValidationMethod> Keys
        {
            get 
            {
                using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
                {
                    return m_dict.Keys;
                }
            }
        }

        public bool Remove(TisValidationMethod key)
        {
            return m_dict.Remove(key);
        }

        public bool TryGetValue(TisValidationMethod key, out TisValidationMethodResult value)
        {
            using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
            {
                return m_dict.TryGetValue(key, out value);
            }
        }

        public ICollection<TisValidationMethodResult> Values
        {
            get 
            {
                using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
                {
                    return m_dict.Values;
                }
            }
        }

        public TisValidationMethodResult this[TisValidationMethod key]
        {
            get
            {
                using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
                {
                    return m_dict[key];
                }
            }
            set
            {
                using (RWLockWriteSession lockSession = new RWLockWriteSession(m_dictLocker))
                {
                    m_dict[key] = value;
                }
            }
        }

        public void Add(KeyValuePair<TisValidationMethod, TisValidationMethodResult> item)
        {
            using (RWLockWriteSession lockSession = new RWLockWriteSession(m_dictLocker))
            {
                m_dict.Add(item);
            }
        }

        public void Clear()
        {
            using (RWLockWriteSession lockSession = new RWLockWriteSession(m_dictLocker))
            {
                m_dict.Clear();
            }
        }

        public bool Contains(KeyValuePair<TisValidationMethod, TisValidationMethodResult> item)
        {
            using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
            {
                return m_dict.Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<TisValidationMethod, TisValidationMethodResult>[] array, int arrayIndex)
        {
            using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
            {
                m_dict.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get 
            {
                using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
                {
                    return m_dict.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
                {
                    return m_dict.IsReadOnly;
                }
            }
        }

        public bool Remove(KeyValuePair<TisValidationMethod, TisValidationMethodResult> item)
        {
            using (RWLockWriteSession lockSession = new RWLockWriteSession(m_dictLocker))
            {
                return m_dict.Remove(item);
            }
        }

        public IEnumerator<KeyValuePair<TisValidationMethod, TisValidationMethodResult>> GetEnumerator()
        {
            using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
            {
                return m_dict.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (RWLockReadSession lockSession = new RWLockReadSession(m_dictLocker))
            {
                return m_dict.GetEnumerator();
            }
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            if (this.m_dictLocker == null)
            {
                m_dictLocker = new ReaderWriterLock();
            }
        }

        #endregion
    }

    #endregion

    #region TisValidationMethodResult

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisValidationMethodResult
    {
        [IgnoreDataMember]
        private object[] m_parameters;
        [DataMember]
        private ValidationStatus m_status;
        [DataMember]
        private TisValidationMethodDetailedInfo m_detailedInfo;

        public TisValidationMethodResult()
        {
        }

        public TisValidationMethodResult(
            ValidationStatus status,
            object[] parameters,
            TisValidationMethodDetailedInfo detailedInfo)
        {
            m_status = status;
            m_parameters = parameters;
            m_detailedInfo = detailedInfo;
        }

        public ValidationStatus Status
        {
            get
            {
                return m_status;
            }
        }

        public object[] Parameters
        {
            get
            {
                return m_parameters;
            }
        }

        public TisValidationMethodDetailedInfo DetailedInfo
        {
            get
            {
                return m_detailedInfo;
            }
        }
    }

    #endregion

    #region TisValidationMethodDetailedInfo

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisValidationMethodDetailedInfo
    {
        [DataMember]
        private string m_message;
        [DataMember]
        private string m_details;
        [DataMember]
        private ValidationShowDetailedInfo m_showDetailedInfo = ValidationShowDetailedInfo.WHEN_FAILED;

        public TisValidationMethodDetailedInfo()
        {
        }

        public TisValidationMethodDetailedInfo(string message, string details, ValidationShowDetailedInfo showDetailedInfo)
        {
            m_message = message;
            m_details = details;
            m_showDetailedInfo = showDetailedInfo;
        }

        #region ITisValidationMethodDetailedInfo Members

        public string Message
        {
            get
            {
                return m_message;
            }
        }

        public string Details
        {
            get
            {
                return m_details;
            }
        }

        public ValidationShowDetailedInfo ShowDetailedInfo
        {
            get
            {
                return m_showDetailedInfo;
            }
        }

        #endregion
    }

    #endregion
}
