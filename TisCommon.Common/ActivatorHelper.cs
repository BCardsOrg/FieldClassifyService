using System;
using System.Collections;

namespace TiS.Core.TisCommon
{
    public class ActivatorHelper
    {
        private static GacAssemblyResolver m_gacAssemblyResolver = new GacAssemblyResolver();
        public static object CreateInstance(
            string className,
            object[] ctorParams)
        {
            Type type;
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(m_gacAssemblyResolver.AssemblyResolveHandler);
                type = Type.GetType(className, true);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(m_gacAssemblyResolver.AssemblyResolveHandler);
            }

            // Create the instance
            return Activator.CreateInstance(type, ctorParams);
        }

        public static object[] CreateInstances(
            IHierarchicalData[] instanceParamsList,
            object[] additionalCtorParams)
        {
            int objectsCount = instanceParamsList.Length;

            object[] objects = new object[objectsCount];

            for (int i = 0; i < objectsCount; i++)
            {
                IHierarchicalData instanceParams = instanceParamsList[i];

                // Get the instance implementation class
                string implClassName = instanceParams.GetMandatoryString("ImplClassName");

                // Create ctor params
                object[] actualCtorParams = CreateCtorParams(instanceParams, additionalCtorParams);

                // Create instance and add to the map
                objects[i] = CreateInstance(implClassName, actualCtorParams);
            }

            return objects;
        }

        private static object[] CreateCtorParams(
            IHierarchicalData InitParams,
            object[] additionalCtorParams)
        {
            int actualParamsSize = 1;

            if (additionalCtorParams != null)
            {
                actualParamsSize += additionalCtorParams.Length;
            }

            object[] actualParams = new object[actualParamsSize];

            if (additionalCtorParams != null)
            {
                additionalCtorParams.CopyTo(actualParams, 0);
            }

            actualParams[actualParamsSize - 1] = InitParams;

            return actualParams;
        }

    }
}
