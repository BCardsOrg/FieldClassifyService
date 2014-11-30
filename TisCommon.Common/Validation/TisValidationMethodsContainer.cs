using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationMathodsContainer

    public class TisValidationMethodsContainer : List<TisValidationMethod>
    {
        public TisValidationMethodsContainer(ITisValidationProvider[] validationProviders)
        {
            List<Type> validationProvidersData = new List<Type>();

            foreach (ITisValidationProvider validationProvider in validationProviders)
            {
                Type validationProviderData = validationProvider.ValidationProviderData;

                if (validationProviderData != null)
                {
                    validationProvidersData.Add(validationProviderData);
                }
                else
                {
                    Log.WriteInfo(
                        "Validation provider data is not defined for validation provider [{0}]",
                        validationProvider.GetType().FullName);
                }
            }

            FillContainer(validationProvidersData);
        }

        private void FillContainer(List<Type> validationProvidersData)
        {
            foreach (Type validationProviderData in validationProvidersData)
            {
                if (typeof(ITisSupportValidation).IsAssignableFrom(validationProviderData))
                {
                    MethodInfo[] miValidations =
                        validationProviderData.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                    foreach (MethodInfo miValidation in miValidations)
                    {
                        ValidationMethodAttribute validationAttribute =
                            (ValidationMethodAttribute)ReflectionUtil.GetAttribute(miValidation, 
                                                                                   typeof(ValidationMethodAttribute));

                        if (validationAttribute != null)
                        {
                            TisValidationMethod validationMethod =
                                new TisValidationMethod(miValidation, 
                                                        validationAttribute.ValidationOrder, 
                                                        validationAttribute.IsCustomCodeProvider);

                            Add(validationMethod);
                        }
                    }

                    Sort();
                }
                else
                {
                    Log.WriteInfo("Validation provider data does not implement [ITisSupportValidation] interface");
                }
            }
        }
    }

    #endregion
}
