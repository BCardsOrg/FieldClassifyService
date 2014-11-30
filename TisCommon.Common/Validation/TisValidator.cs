using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Validation
{
    public class TisValidator : IComparable
    {
        private static TisInternalValidationManager m_validationManager;
        private ITisValidationProvider m_validationsProvider;
        private TisValidationsSource m_validationsSource;
        private WeakReference m_validationTarget = new WeakReference(null);
        private List<TisValidator> m_validators = new List<TisValidator>();

        public TisValidator(
            TisInternalValidationManager validationManager,
            ITisValidationProvider validationsProvider)
        {
            m_validationManager = validationManager;
            m_validationsProvider = validationsProvider;

            m_validationsSource = m_validationManager.GetValidationsSource(m_validationsProvider);

            m_validators.Add(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_validationsProvider = null;
            m_validationsSource = null;

            m_validationTarget = null;

            if (m_validators != null)
            {
                foreach (TisValidator validator in m_validators)
                {
                    if (!validator.Equals(this))
                    {
                        validator.Dispose();
                    }
                }

                m_validators.Clear();
                m_validators = null;
            }
        }

        #endregion

        #region ITisValidator Members

        public Type ValidationType
        {
            get
            {
                if (m_validationsSource != null)
                {
                    if (m_validationsSource.Source is ITisSupportValidation)
                    {
                        return m_validationsSource.ValidationType;
                    }
                    else
                    {
                        Log.WriteInfo("Validation source for validation provider {0} does not implement [ITisSupportValidation] interface",
                            m_validationsProvider.GetType().FullName);

                        return null;
                    }
                }
                else
                {
                    Log.WriteInfo("Validation source for validation provider {0} does not exist", m_validationsProvider.GetType().FullName);

                    return null;
                }
            }
        }

        public object ValidationTarget
        {
            get
            {
                return m_validationTarget.Target;
            }
            set
            {
                WeakReference reff = new WeakReference(value);
                foreach (TisValidator childValidator in m_validators)
                {
                    childValidator.m_validationTarget = reff;
                }
            }
        }

        public ITisValidationProvider ValidationsProvider
        {
            get
            {
                return m_validationsProvider;
            }
        }

        public TisValidationsSource ValidationsSource
        {
            get
            {
                return m_validationsSource;
            }
        }

        public ValidationStatus Validate(
            TisValidationPolicy validationPolicy,
            TisValidationsResult validationsResult)
        {
            ((TisValidationsResult)validationsResult).Clear();

            ValidationStatus validationStatus = m_validationManager.Validate(
                m_validators,
                validationPolicy,
                validationsResult);

            return validationStatus;
        }

        public void AddValidator(TisValidator validator)
        {
            if (!m_validators.Contains(validator))
            {
                m_validators.Add(validator);
            }
        }

        public List<TisValidationMethod> SupportedValidations
        {
            get
            {
                List<TisValidationMethod> supportedValidations = new List<TisValidationMethod>();

                foreach (TisValidator validator in m_validators)
                {
                    supportedValidations.AddRange(m_validationManager.GetSupportedValidations(validator.ValidationsProvider));
                }

                return supportedValidations;
            }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            TisValidator validator = (TisValidator)obj;

            int this_fromSchema = 0;
            int validator_fromSchema = 0;

            if (this.ValidationsProvider is ITisServiceInfo && validator.ValidationsProvider is ITisServiceInfo)
            {
                this_fromSchema = Convert.ToInt32((this.ValidationsProvider as ITisServiceInfo).FromSchema);
                validator_fromSchema = Convert.ToInt32((validator.ValidationsProvider as ITisServiceInfo).FromSchema);
            }

            if (this_fromSchema > validator_fromSchema)
            {
                // Build-in service will validate first
                return -1;
            }
            else
            {
                if (this_fromSchema < validator_fromSchema)
                {
                    // Build-in service will validate first
                    return 1;
                }
                else
                {
                    if (this.ValidationsSource.Order > validator.ValidationsSource.Order)
                    {
                        return 1;
                    }
                    else
                    {
                        if (this.ValidationsSource.Order < validator.ValidationsSource.Order)
                        {
                            return -1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }

        #endregion

        internal void Sort()
        {
            m_validators.Sort();
        }
    }
}
