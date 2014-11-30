using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.TisCommon.Services;
using TiS.Core.TisCommon.Customizations;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationManager

    public class TisValidationManager
    {
        private readonly string[] NO_ROLES_REQUIRED = EmptyArrays.StringArray;

        private ITisServiceRegistry m_applicationServiceRegistry;
        private TisValidatorManager m_validatorMngr;
        private TisValidationContextManager m_validationContextMngr;

        public TisValidationManager(
            ITisServiceRegistry applicationServiceRegistry,
            TisValidatorManager validatorMngr,
            TisValidationContextManager customValidationContextMngr)
        {
            m_applicationServiceRegistry = applicationServiceRegistry;
            m_validatorMngr = validatorMngr;
            m_validationContextMngr = customValidationContextMngr;
        }

        #region ITisValidationManager Members

        public bool AddValidationService(
            ref string validationServiceName, 
            string validationServiceCreatorFullName, 
            string assemblyName, 
            string typeFullName)
        {
            bool isServiceInstalled = AddService(
                ref validationServiceName,
                validationServiceCreatorFullName,
                assemblyName,
                typeFullName);

            if (isServiceInstalled)
            {
                m_validatorMngr.Refresh();
            }

            return isServiceInstalled ;
        }

        public bool AddValidationContextService(
            ref string validationContextServiceName,
            string validationContextServiceCreatorFullName,
            string assemblyName,
            string typeFullName)
        {
            bool isServiceInstalled = AddService(
                ref validationContextServiceName,
                validationContextServiceCreatorFullName,
                assemblyName,
                typeFullName);

            if (isServiceInstalled)
            {
                m_validationContextMngr.Refresh();
            }

            return isServiceInstalled;
        }

        public void RemoveValidationService(string validationServiceName)
        {
            if (RemoveService(validationServiceName))
            {
                m_validatorMngr.Refresh();
            }
        }

        public void RemoveValidationContextService(string validationContextServiceName)
        {
            if (RemoveService(validationContextServiceName))
            {
                m_validationContextMngr.Refresh();
            }
        }

        public List<TisValidationMethod> SupportedValidations
        {
            get
            {
                return m_validatorMngr.SupportedValidations;
            }
        }


        public List<TisValidationMethod> GetSupportedValidationsByType(string validationTypeName)
        {
            return GetSupportedValidationsByType(Type.GetType(validationTypeName));
        }

        public List<TisValidationMethod> GetSupportedValidationsByType(Type validationType)
        {
            TisValidator validator = 
                m_validatorMngr.GetValidatorByType(validationType);

            if (validator != null)
            {
                return validator.SupportedValidations;
            }
            else
            {
                return new List<TisValidationMethod>();
            }
        }

        #endregion

        private bool AddService(
            ref string serviceName,
            string serviceCreatorFullName,
            string assemblyName,
            string typeFullName)
        {
            if (!StringUtil.IsStringInitialized(assemblyName))
            {
                Log.WriteWarning("Try to install validation service without assembly name.");
                return false;
            }

            if (!StringUtil.IsStringInitialized(typeFullName))
            {
                Log.WriteWarning("Try to install validation service without type name.");
                return false;
            }

            if (!StringUtil.IsStringInitialized(serviceName))
            {
                serviceName = Guid.NewGuid().ToString();

                Log.WriteInfo("Try to install nameless service. Unique name [{0}] will be used.", serviceName);
            }

            bool isServiceInstalled = m_applicationServiceRegistry.IsServiceInstalled(serviceName);

            if (!isServiceInstalled)
            {
                if (!StringUtil.IsStringInitialized(serviceCreatorFullName))
                {
                    serviceCreatorFullName =
                        typeof(TisUniversalServiceCreator).FullName + "," +
                        typeof(TisUniversalServiceCreator).Assembly.GetName().Name;
                }

                try
                {
                    m_applicationServiceRegistry.InstallService(
                        serviceName,
                        serviceCreatorFullName,
                        typeFullName + "," + assemblyName,
                        NO_ROLES_REQUIRED,
                        false);
                }
                catch (Exception exc)
                {
                    Log.WriteWarning("Failed to install service [{0}]. Details : {1}", serviceName, exc.Message);
                }

                isServiceInstalled = m_applicationServiceRegistry.IsServiceInstalled(serviceName);
            }

            return isServiceInstalled;
        }

        private bool RemoveService(string serviceName)
        {
            bool isServiceRemoved = false;

            if (StringUtil.IsStringInitialized(serviceName))
            {
                if (m_applicationServiceRegistry.IsServiceInstalled(serviceName))
                {
                    try
                    {
                        m_applicationServiceRegistry.UninstallService(serviceName);

                        isServiceRemoved = true;
                    }
                    catch (Exception exc)
                    {
                        Log.WriteWarning("Failed to uninstall service [{0}]. Details : {1}", serviceName, exc.Message);
                    }
                }
                else
                {
                    Log.WriteInfo("Try to uninstall not installed service [{0}].", serviceName);
                }
            }
            else
            {
                Log.WriteInfo("Try to uninstall nameless service.");
            }

            return isServiceRemoved;
        }
    }

    #endregion

    #region TisInternalValidationManager

    public class TisInternalValidationManager : TisApplicationServicesProvider
    {
        private Dictionary<Type, TisValidationMethodsList<TisValidationMethod>> m_supportedValidationsCache =
            new Dictionary<Type, TisValidationMethodsList<TisValidationMethod>>();

        private Dictionary<Type, TisValidationsSourceProvider> m_validationSourceProviderCache =
            new Dictionary<Type, TisValidationsSourceProvider>();

        private TisValidationContextManager m_validationContextMngr;

        private object m_locker = new object();

        public TisInternalValidationManager(
            string sApplicationName,
            ITisServicesHost oServicesHost,
            TisValidationContextManager validationContextMngr)
            : base(oServicesHost, sApplicationName)
        {
            m_validationContextMngr = validationContextMngr;
        }

        #region ITisInternalValidationManager Members

        public List<TisValidationMethod> GetSupportedValidations(ITisValidationProvider validationsProvider)
        {
            TisValidationMethodsList<TisValidationMethod> validationMethodsList = null;

            Type validationProviderData = validationsProvider.ValidationProviderData;

            if (validationProviderData != null)
            {
                lock (m_locker)
                {
                    m_supportedValidationsCache.TryGetValue(
                        validationProviderData,
                        out validationMethodsList);

                    if (validationMethodsList == null)
                    {
                        validationMethodsList =
                            new TisValidationMethodsList<TisValidationMethod>(new ITisValidationProvider[] { validationsProvider });

                        m_supportedValidationsCache.Add(validationProviderData, validationMethodsList);
                    }
                }
            }

            return validationMethodsList;
        }

        public TisValidationsSource GetValidationsSource(ITisValidationProvider validationsProvider)
        {
            Type validationSourceProviderData = validationsProvider.ValidationSourceProviderData;

            if (validationSourceProviderData != null)
            {
                TisValidationsSourceProvider validationSourceProvider;

                lock (m_locker)
                {
                    m_validationSourceProviderCache.TryGetValue(
                        validationSourceProviderData,
                        out validationSourceProvider);

                    if (validationSourceProvider == null)
                    {
                        try
                        {
                            validationSourceProvider =
                                (TisValidationsSourceProvider)Activator.CreateInstance(validationSourceProviderData,
                                                                                       new object[] { this });
                        }
                        catch
                        {
                            throw new TisException("Failed to instantiate validation source provider.");
                        }

                        if (validationSourceProvider != null)
                        {
                            m_validationSourceProviderCache[validationSourceProviderData] = validationSourceProvider;
                        }
                        else
                        {
                            throw new TisException("Validation source provider does not exist.");
                        }
                    }
                }

                return validationSourceProvider.GetValidationsSource(validationsProvider);
            }
            else
            {
                new TisException("Validation source provider data does not exist.");
            }

            return null;
        }

        public ValidationStatus Validate(
            List<TisValidator> validators,
            TisValidationPolicy validationPolicy,
            TisValidationsResult validationsResult)
        {
            foreach (TisValidator validator in validators)
            {
                ValidationStatus lastValidationStatus = Validate(
                    validator.ValidationsProvider,
                    validator.ValidationsSource,
                    validator.ValidationTarget,
                    validationPolicy,
                    validationsResult);

                if (ShouldStopValidation(lastValidationStatus, validationPolicy.StopPolicy, validationPolicy.ResultPolicy))
                {
                    return ValidationStatus.Invalid;
                }
            }

            return TisValidationsResult.GetFinalValidationStatus(
                validationsResult,
                validationPolicy.ResultPolicy);
        }

        public ValidationStatus Validate(
            ITisValidationProvider validationsProvider,
            TisValidationsSource validationsSource,
            object validationsTarget,
            TisValidationPolicy validationPolicy,
            TisValidationsResult validationsResult)
        {
            if (validationsSource == null)
            {
                throw new TisException("Validations source does not exist.");
            }

            if (validationsResult == null)
            {
                throw new TisException("Validations result is not allocated.");
            }

            ParallelCall parallelCall = new ParallelCall();

            List<TisValidationMethod> supportedValidations =
                GetSupportedValidations(validationsProvider);

            parallelCall.ThreadsToUse = supportedValidations.Count;

            TisValidationMethodResult validationMethodResult;
            List<TisValidationMethod> performedValidations = new List<TisValidationMethod>();

            List<ITisValidationContext> validationContexts =
                m_validationContextMngr.GetValidationContextsByType(validationsSource.ValidationType);
            
            foreach (TisValidationMethod supportedValidation in supportedValidations)
            {
                if (IsValidationConfirmed(supportedValidation, validationContexts))
                {
                    if (validationPolicy.RunPolicy == ValidationRunPolicy.Sync)
                    {
                        validationMethodResult = ExecuteValidationMethod(
                            supportedValidation, 
                            validationsSource.Source,
                            validationsTarget,
                            validationContexts);

                        validationsResult.Add(supportedValidation, validationMethodResult);

                        if (ShouldStopValidation(
                            validationMethodResult.Status,
                            validationPolicy.StopPolicy,
                            validationPolicy.ResultPolicy))
                        {
                            break;
                        }
                    }
                    else
                    {
                        performedValidations.Add(supportedValidation);

                        parallelCall.Add(
                            new ExecuteValidationMethodDelegate(ExecuteValidationMethod),
                            new object[] { supportedValidation, 
                                           validationsSource.Source,
                                           validationsTarget,
                                           validationContexts});
                    }
                }
                else
                {
                    parallelCall.ThreadsToUse--;
                }
            }

            if (validationPolicy.RunPolicy == ValidationRunPolicy.Async)
            {
                object[] asyncValidationResults = parallelCall.Perform(true);

                for (int i = 0; i < performedValidations.Count; i++)
                {
                    validationsResult.Add(performedValidations[i], (TisValidationMethodResult)asyncValidationResults[i]);
                }
            }

            return TisValidationsResult.GetFinalValidationStatus(
                validationsResult, 
                validationPolicy.ResultPolicy);
        }

        #endregion

        private bool IsValidationConfirmed(
            TisValidationMethod validationMethod, 
            List<ITisValidationContext> validationContexts)
        {
            bool confirmValidationMethod = true;
            bool handled = false;

            foreach (ITisValidationContext validationContext in validationContexts)
            {
                if (validationContext.ValidationMethodConfirmatory != null)
                {
                    confirmValidationMethod =
                        validationContext.ValidationMethodConfirmatory.ConfirmValidationMethod(validationMethod, ref handled);

                    if (handled)
                    {
                        return confirmValidationMethod;
                    }
                }
            }

            return confirmValidationMethod;
        }

        private bool ShouldStopValidation(
            ValidationStatus lastValidationStatus,
            ValidationStopPolicy stopValidationPolicy,
            ValidationResultPolicy resultValidationPolicy)
        {
            if (stopValidationPolicy == ValidationStopPolicy.FirstInvalid)
            {
                if (resultValidationPolicy == ValidationResultPolicy.ConsiderAll)
                {
                    if (lastValidationStatus != ValidationStatus.Valid)
                    {
                        return true;
                    }
                }
                else
                {
                    if (lastValidationStatus == ValidationStatus.Invalid)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private delegate TisValidationMethodResult ExecuteValidationMethodDelegate(
            TisValidationMethod validationMethod,
            object validationsSource,
            object validationsTarget,
            List<ITisValidationContext> validationContexts);

        private TisValidationMethodResult ExecuteValidationMethod(
            TisValidationMethod validationMethod,
            object validationsSource,
            object validationsTarget,
            List<ITisValidationContext> validationContexts)
        {
            TisValidationMethodDetailedInfo detailedInfo = null;
            ValidationStatus validationStatus = ValidationStatus.Unknown;

            object[] validationMethodParameters = null;

            try
            {
                validationMethodParameters =
                    ObtainValidationMethodParameters(validationMethod, validationsTarget, validationContexts);

                if (validationMethodParameters == null)
                {
                    // Default method parameters
                    validationMethodParameters =
                        new object[validationMethod.MethodSignature.Params.Length];
                }

                bool validParams =
                    ValidateMethodParameters(validationMethod.MethodSignature, validationMethodParameters);

                if (validParams)
                {
                    object methodReturn =
                        validationMethod.Method.Invoke(validationsSource, validationMethodParameters);

                    if (methodReturn is ValidationStatus)
                    {
                        validationStatus = (ValidationStatus)methodReturn;
                    }
                    else
                    {
                        if (methodReturn is TisValidationStatusProvider)
                        {
                            validationStatus = (methodReturn as TisValidationStatusProvider).Status;
                            detailedInfo = (methodReturn as TisValidationStatusProvider).DetailedInfo;
                        }
                        else
                        {
                            Log.WriteWarning("Validation method [{0}] does not return validation status ", validationMethod.Name);
                        }
                    }
                }
                else
                {
                    Log.WriteError("Validation method [{0}] has invalid parameter(s)", validationMethod.Name);
                }
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);
            }

            return new TisValidationMethodResult(validationStatus, validationMethodParameters, detailedInfo);
        }

        private object[] ObtainValidationMethodParameters(
            TisValidationMethod validationMethod,
            object validationsTarget,
            List<ITisValidationContext> validationContexts)
        {
            object[] validationMethodParameters = null;
            bool handled = false;

            foreach (ITisValidationContext validationContext in validationContexts)
            {
                if (validationContext.ValidationMethodParametersProvider != null)
                {
                    validationMethodParameters = validationContext.ValidationMethodParametersProvider.ObtainValidationMethodParameters(
                        validationMethod, 
                        validationsTarget,
                        ref handled);

                    if (handled)
                    {
                        return validationMethodParameters;
                    }
                }
            }

            return validationMethodParameters;
        }

        private bool ValidateMethodParameters(
            ITisMethodSignature validationMethodSignature,
            object[] validationMethodParameters)
        {
            bool validParams = true;

            int paramsCount = validationMethodSignature.Params.Length;

            if (paramsCount != validationMethodParameters.Length)
            {
                validParams = false;
            }

            if (validParams)
            {
                for (int i = 0; i < paramsCount; i++)
                {
                    if (validationMethodParameters[i] != null)
                    {
                        Type declaredParameterType =
                            validationMethodSignature.ParamTypes[i];

                        Type providedParameterType = 
                            validationMethodParameters[i].GetType();

                        if (declaredParameterType != providedParameterType &&
                            !declaredParameterType.IsAssignableFrom(providedParameterType))
                        {
                            if (declaredParameterType.IsByRef &&
                                providedParameterType.MakeByRefType() != declaredParameterType)
                            {
                                validParams = false;
                                break;
                            }
                        }

                    }
                }
            }

            return validParams;
        }
    }

    #endregion
}
