using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Validation
{
    #region TisValidationMathodsContainer

    public  class TisValidationMethodsList<T> : List<TisValidationMethod> where T : TisValidationMethod
    {
        public TisValidationMethodsList(ITisValidationProvider[] validationProviders)
        {
            IEnumerable<Type> validationProvidersData =
                from validationProvider in validationProviders
                where validationProvider.ValidationProviderData != null
                select validationProvider.ValidationProviderData;

            FillContainer(validationProvidersData);
        }

        private void FillContainer(IEnumerable<Type> validationProvidersData)
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
