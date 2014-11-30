using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    // All the strong typed rules will be derived from this base class
    [ComVisible(false)]
    public class StrongTypeRulesBase
    {
        protected object m_UserDefinedFunctions;
        protected VariableValueList m_TiSVariableValues;


        // UserDefinedFunctions can be null - no user defined functions
        public StrongTypeRulesBase(object UserDefinedFunctions)
        {
            m_UserDefinedFunctions = UserDefinedFunctions;
            m_TiSVariableValues = null;
        }


        // this method should be overloaded in the derived class, to implement
        //	the specific rule that this class was designed to evaluate
        // The long parameter name is to prevent accidental usage as a valid
        //	parameter name

        public virtual bool RuleTrue(VariableValueList TiS_TagsValueForRuleCalculation_Tis)
        {
            if (TiS_TagsValueForRuleCalculation_Tis == null)
                throw new ArgumentException("Error! StrongTypeRulesBase:RuleTrue can not get null argument");

            m_TiSVariableValues = TiS_TagsValueForRuleCalculation_Tis;

            // not relvant - just to match the function signature
            return true;
        }

        protected object InvokeUserDefinedFunction(
            string sFunction,
            params object[] Params)
        {
            // Get MethodInfo
            MethodInfo oMethod =
                m_UserDefinedFunctions.GetType().GetMethod(sFunction);

            if (oMethod == null)
            {
                throw new TisException(
                    "User-defined method [{0}] not found in class [{1}]",
                    sFunction,
                    m_UserDefinedFunctions.GetType());
            }

            object[] AllParams = new object[Params.Length + 1];

            // First parameter is always VariableValues collection
            AllParams[0] = this.m_TiSVariableValues;

            // Add other params
            Params.CopyTo(AllParams, 1);

            // Dynamically invoke method
            return oMethod.Invoke(m_UserDefinedFunctions, AllParams);
        }

    }
}
