using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.TisCommon.Services;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Validation
{
    public class TisValidationContextManager
    {
        private string m_applicationName;
        private ITisServicesHost m_servicesHost;
        private ITisServiceRegistry m_serviceRegistry;
        private Dictionary<Type, List<ITisValidationContext>> m_allValidationContexts =
            new Dictionary<Type, List<ITisValidationContext>>();

        private object m_locker = new object();

        public TisValidationContextManager(
            string sApplicationName,
            ITisServicesHost oServicesHost)
        {
            m_applicationName=sApplicationName;

            m_servicesHost = oServicesHost;

            m_serviceRegistry =
                m_servicesHost.GetServiceRegistry(sApplicationName);

            Refresh();
        }

        #region ITisValidationContextMngr Members

        public int Count
        {
            get
            {
                return m_allValidationContexts.Count;
            }
        }

        public List<ITisValidationContext> All
        {
            get
            {
                List<ITisValidationContext> allValidationContexts = new List<ITisValidationContext>();

                foreach (List<ITisValidationContext> validationContexts in m_allValidationContexts.Values)
                {
                    allValidationContexts.AddRange(validationContexts);
                }

                return allValidationContexts;
            }
        }

        public List<ITisValidationMethodParametersProvider> ValidationMethodParametersProviders
        {
            get
            {
                List<ITisValidationMethodParametersProvider> validationMethodParametersProviders = 
                    new List<ITisValidationMethodParametersProvider>();

                foreach (List<ITisValidationContext> validationContexts in m_allValidationContexts.Values)
                {
                    foreach (ITisValidationContext validationContext in validationContexts)
                    {
                        validationMethodParametersProviders.Add(validationContext.ValidationMethodParametersProvider);
                    }
                }

                return validationMethodParametersProviders;
            }
        }

        public List<ITisValidationMethodConfirmatory> ValidationMethodConfirmatories
        {
            get
            {
                List<ITisValidationMethodConfirmatory> validationMethodConfirmatories =
                    new List<ITisValidationMethodConfirmatory>();

                foreach (List<ITisValidationContext> validationContexts in m_allValidationContexts.Values)
                {
                    foreach (ITisValidationContext validationContext in validationContexts)
                    {
                        validationMethodConfirmatories.Add(validationContext.ValidationMethodConfirmatory);
                    }
                }

                return validationMethodConfirmatories;
            }
        }

        public List<ITisValidationContext> GetValidationContextsByType(Type validationType)
        {
            List<ITisValidationContext> typedValidationContexts;

            List < ITisValidationContext > validationContexts = 
                new List<ITisValidationContext>();

            lock (m_locker)
            {
                m_allValidationContexts.TryGetValue(validationType, out typedValidationContexts);

                if (typedValidationContexts != null)
                {
                    validationContexts.AddRange(typedValidationContexts);
                }
                else
                {
                    Log.WriteInfo("Validation contexts for type {0} do not exist", validationType.FullName);
                }

                return validationContexts;
            }
        }

        public void Refresh()
        {
            ObtainAllValidationContexts();
        }

        #endregion

        private void ObtainAllValidationContexts()
        {
            List<ITisValidationContext> typedValidationContexts;
            List<Type> validationTypes;

            lock (m_locker)
            {
                m_allValidationContexts.Clear();

                string[] validationContextServiceNames = TisServicesUtil.GetServicesOfImplType(
                    m_serviceRegistry,
                    typeof(ITisValidationContext));

                ITisValidationContext validationContext;

                foreach (string validationContextServiceName in validationContextServiceNames)
                {
                    try
                    {
                        validationContext =
                            (ITisValidationContext)m_servicesHost.GetService(m_applicationName, validationContextServiceName);
                    }
                    catch
                    {
                        validationContext = null;
                    }

                    if (validationContext != null)
                    {
                        validationTypes = ObtainValidationTypes(validationContext);

                        foreach (Type validationType in validationTypes)
                        {
                            if (validationType != null)
                            {
                                if (!m_allValidationContexts.ContainsKey(validationType))
                                {
                                    m_allValidationContexts.Add(validationType, new List<ITisValidationContext>());
                                }

                                typedValidationContexts = m_allValidationContexts[validationType];

                                typedValidationContexts.Add(validationContext);
                            }
                            else
                            {
                                Log.WriteWarning("Installed validation context service [{0}] does not provide validation target type", validationContextServiceName);
                            }
                        }
                    }
                    else
                    {
                        Log.WriteWarning("Validation service [{0}] is not installed", validationContextServiceName);
                    }
                }
            }
        }

        private List<Type> ObtainValidationTypes(ITisValidationContext validationContext)
        {
            List<Type> validationTypes = new List<Type>();

            Attribute[] validationContextAttributes = ReflectionUtil.GetAttributes(
                validationContext.GetType(),
                typeof(TisValidationContextAttribute));

            if (validationContextAttributes.Length > 0)
            {
                foreach (TisValidationContextAttribute validationContextAttribute in validationContextAttributes)
                {
                    if (!validationTypes.Contains(validationContextAttribute.ValidationType))
                    {
                        validationTypes.Add(validationContextAttribute.ValidationType);
                    }
                }
            }
            else
            {
                foreach (Type validationType in validationContext.ValidationTypes)
                {
                    if (!validationTypes.Contains(validationType))
                    {
                        validationTypes.Add(validationType);
                    }
                }
            }

            return validationTypes;
        }
    }
}
