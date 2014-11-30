using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Validation
{
    public class TisValidatorManager : TisApplicationServicesProvider
    {
        ITisServiceRegistry m_serviceRegistry;
        Dictionary<Type, TisValidator> m_allApplicationValidators = new Dictionary<Type, TisValidator>();

        public TisValidatorManager(
            string sApplicationName,
            ITisServicesHost oServicesHost)
            : base(oServicesHost, sApplicationName)
        {
            m_serviceRegistry = 
                oServicesHost.GetServiceRegistry(sApplicationName);

            Refresh();
        }

        public override void Dispose()
        {
            if (m_allApplicationValidators != null)
            {
                m_allApplicationValidators.Clear();
                m_allApplicationValidators = null;
            }

            m_serviceRegistry = null;

            base.Dispose();
        }

        #region ITisValidatorMngr Members

        public List<TisValidator> All
        {
            get
            {
                List<TisValidator> allApplicationValidators = new List<TisValidator>();

                allApplicationValidators.AddRange(m_allApplicationValidators.Values);

                return allApplicationValidators;
            }
        }

        public List<TisValidationMethod> SupportedValidations
        {
            get
            {
                List<TisValidationMethod> supportedValidations = new List<TisValidationMethod>();

                foreach (TisValidator validator in m_allApplicationValidators.Values)
                {
                    supportedValidations.AddRange(validator.SupportedValidations);
                }

                return supportedValidations;
            }
        }

        public TisValidator GetValidatorByType(Type validatorType)
        {
            TisValidator validator;

            m_allApplicationValidators.TryGetValue(validatorType, out validator);

            if(validator == null)
            {
                Log.WriteInfo("Validator for type {0} does not exist", validatorType.FullName);
            }

            return validator;
        }

        public void Refresh()
        {
            ObtainAllValidators();
        }

        #endregion

        private void ObtainAllValidators()
        {
            string[] validationServiceProviderNames = TisServicesUtil.GetServicesOfImplType(
                m_serviceRegistry,
                typeof(ITisSupportValidation));

            foreach (string validationServiceProviderName in validationServiceProviderNames)
            {
                TisValidator validator =
                    (TisValidator)GetService(TisServicesSchema.Validator, validationServiceProviderName);

                if (validator != null)
                {
                    Type validationType = validator.ValidationType;

                    if (validationType != null)
                    {
                        TisValidator parentValidator = null;

                        m_allApplicationValidators.TryGetValue(validationType, out parentValidator);

                        if (parentValidator != null)
                        {
                            parentValidator.AddValidator(validator);
                        }
                        else
                        {
                            m_allApplicationValidators.Add(validationType, validator);
                        }
                    }
                    else
                    {
                        Log.WriteWarning("Installed validation service [{0}] does not provide validation target type", validationServiceProviderName);
                    }
                }
                else
                {
                    Log.WriteWarning("Validation service [{0}] is not installed", validationServiceProviderName);
                }
            }

            foreach (TisValidator validator in m_allApplicationValidators.Values)
            {
                try
                {
                    ((TisValidator)validator).Sort();
                }
                catch (Exception exc)
                {
                    Log.WriteException(Log.Severity.INFO, exc);
                }
            }
        }
    }
}
