using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class FunctionsReflector
    {
        protected object m_ObjectToReflect;
        protected Type m_ObjectType;

        // allow safe access to internal members
        protected Type ObjectType
        {
            get
            {
                if (m_ObjectType == null)
                    throw new NullReferenceException("ObjectType must not be null");
                else
                    return m_ObjectType;
            }

            set
            {
                if (value == null)
                    throw new NullReferenceException("ObjectType must not be null");
                else
                    m_ObjectType = value;
            }

        }


        public FunctionsReflector()
        {
            m_ObjectToReflect = null;
            m_ObjectType = null;
        }


        public FunctionsReflector(object ObjectToReflect)
        {
            this.SetObjectToReflect(ObjectToReflect);
        }


        public void SetObjectToReflect(object Obj)
        {
            if (Obj == null)
                throw new ArgumentNullException("SetObjectToReflect can not get null as parameter");

            ObjectType = Obj.GetType();
        }


        public MethodInfo[] GetMethodsInfo(BindingFlags Flags)
        {
            return ObjectType.GetMethods(Flags);
        }


        // Default implementation
        public MethodInfo[] GetMethodsInfo()
        {
            return GetMethodsKeepNonSpecialOnly();
        }

        #region Specific function information getters

        public MethodInfo[] GetMethodsKeepNonSpecialInherited()
        {
            MethodInfo[] Methods = this.GetMethodsInfo(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.NonPublic);

            return FilterKeepNonSpecial(Methods);
        }

        public MethodInfo[] GetMethodsKeepNonSpecialOnly()
        {
            MethodInfo[] Methods = this.GetMethodsInfo(BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly);

            return FilterKeepNonSpecial(Methods);
        }

        private MethodInfo[] FilterKeepNonSpecial(MethodInfo[] Methods)
        {
            ArrayList RelevantMethods = new ArrayList();

            foreach (MethodInfo MI in Methods)
            {
                if (MI.IsSpecialName == false)
                    RelevantMethods.Add(MI);
            }

            return (MethodInfoStackToArray(RelevantMethods));

        }

        #endregion

        #region General helpers functions

        protected MethodInfo[] MethodInfoStackToArray(ArrayList RelevantMethods)
        {
            int nMethods = RelevantMethods.Count;

            MethodInfo[] ReturnedMethods = new MethodInfo[nMethods];

            for (int iMethods = 0; iMethods < nMethods; ++iMethods)
                ReturnedMethods[iMethods] = (MethodInfo)(RelevantMethods[iMethods]);

            return ReturnedMethods;
        }

        public void ConsoleDumpFunctionsData(BindingFlags Flags)
        {
            MethodInfo[] Methods = this.GetMethodsInfo(Flags);
            Console.WriteLine("{0} functions were found in the reflection process", Methods.Length);
            foreach (MethodInfo MethodIn in Methods)
                Console.WriteLine("Method Name = {0} Attr={1}", MethodIn.Name, MethodIn.Attributes);
        }

        public static void ConsoleDumpFunctionsData(MethodInfo[] Methods)
        {
            foreach (MethodInfo MethodIn in Methods)
                Console.WriteLine("Method Name = {0} Attr={1}", MethodIn.Name, MethodIn.Attributes);
        }

        #endregion

    }
}
