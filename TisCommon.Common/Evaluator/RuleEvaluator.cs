using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class RuleEvaluator : IRuleEvaluator
    {
        protected Hashtable m_oRulesCache;
        protected VariableDeclarationList m_oGlobalVariablesDeclaration;
        protected object m_oPredefinedFunctions;
        protected bool m_bValidateVariables;
        internal StrongTypedRuleBuilder m_oStrongTypedRuleBuilder;

        private const string STRONG_TYPED_CLASSES_NAMESPACE = "TiSStrongTypedClasses";


        public RuleEvaluator(VariableDeclarationList TagsSchema,
                                object PredefinedFunctions,
                                string sTempFilesPath,
                                bool bKeepTempFiles)
        {
            InitializeRuleEvaluator(
                TagsSchema,
                PredefinedFunctions,
                sTempFilesPath,
                bKeepTempFiles);
        }

        public RuleEvaluator(VariableDeclarationList TagsSchema,
                                object PredefinedFunctions,
                                bool ValidateVariables,
                                string sTempFilesPath,
                                bool bKeepTempFiles)
        {
            InitializeRuleEvaluator(
                TagsSchema,
                PredefinedFunctions,
                sTempFilesPath,
                bKeepTempFiles);

            m_bValidateVariables = ValidateVariables;
        }

        public bool CheckRuleSyntax(Rule RuleToEvaluate, out CompilerErrorInfo[] CompilerErrors)
        {
            CompilerErrors = new CompilerErrorInfo[] { };

            if (RuleToEvaluate == null)
                throw new ArgumentNullException("Error! RuleEvaluator:CheckRuleSyntax can not get null argument");

            StrongTypeRulesBase StrongTypeRule;

            try
            {
                StrongTypeRule = GetStrongTypedRule(RuleToEvaluate);

                return StrongTypeRule != null;
            }
            catch (RuleCompilationException e)
            {
                CompilerErrors = e.CompilerErrors;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public ITisRuleSyntaxCheckResult CheckRuleSyntax(string sRule)
        {
            CompilerErrorInfo[] Errors;

            bool success = CheckRuleSyntax(
                new Rule(sRule),
                out Errors);

            RuleSyntaxCheckResult result =
                new RuleSyntaxCheckResult(success, Errors);

            return result;
        }

        public void PrecompileRules(Rule[] Rules)
        {
            if (Rules == null)
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "Rules",
                    "Can't be null",
                    System.Reflection.MethodInfo.GetCurrentMethod());
            }

            lock (this)
            {
                ArrayBuilder oRulesToCompile = new ArrayBuilder();

                // Filter out rules that are already in cache and
                // redundant rules
                foreach (Rule oRule in Rules)
                {
                    if (GetStrongTypedRuleFromCache(oRule) != null)
                    {
                        // Already compiled
                        continue;
                    }

                    oRulesToCompile.AddIfNotExists(oRule);
                }

                Rule[] RulesToCompile = (Rule[])oRulesToCompile.GetArray(
                    typeof(Rule));

                StrongTypeRulesBase[] StrongTypedRules =
                    m_oStrongTypedRuleBuilder.CreateNewRules(RulesToCompile);

                for (int i = 0; i < RulesToCompile.Length; i++)
                {
                    Rule oRule = RulesToCompile[i];
                    StrongTypeRulesBase oStrongTypedRule = StrongTypedRules[i];

                    AddStrongTypedRuleToCache(
                        oRule,
                        oStrongTypedRule);
                }
            }
        }

        public bool RuleTrue(VariableValueList oVariableValues, Rule RuleToEvaluate)
        {
            bool ReturnedValue = false;

            lock (this)
            {

                if (oVariableValues == null || RuleToEvaluate == null)
                    throw new ArgumentNullException("Error! RuleEvaluator:RuleTrue can not get null argument");

                if (m_bValidateVariables == true)
                    CheckVariableDeclarationListAndValuesConsistency(oVariableValues);

                StrongTypeRulesBase StrongTypeRule;

                StrongTypeRule = GetStrongTypedRule(RuleToEvaluate);

                ReturnedValue = StrongTypeRule.RuleTrue(oVariableValues);
            }

            return ReturnedValue;
        }

        protected void InitializeRuleEvaluator(
            VariableDeclarationList TagsSchema,
            object PredefinedFunctions,
            string sTempFilesPath,
            bool bKeepTempFiles)
        {
            if (TagsSchema == null || PredefinedFunctions == null)
                throw new ArgumentNullException("Error! RuleEvaluator:RuleEvaluator can not get null argument");

            m_oGlobalVariablesDeclaration = TagsSchema;
            m_oPredefinedFunctions = PredefinedFunctions;

            m_oRulesCache = new Hashtable();

            m_oStrongTypedRuleBuilder = new StrongTypedRuleBuilder(
                m_oGlobalVariablesDeclaration,
                m_oPredefinedFunctions,
                sTempFilesPath,
                bKeepTempFiles);

            m_bValidateVariables = true;
        }

        // Check that there are no tags names in run-time (meta-tags values)
        //	which name does not exist in the setup-time (meta-tags schema)
        // Anyway this error will be found when compiling the strong-typed class,
        //	but using this check at this earlier stage can find this problem much
        //	faster
        protected void CheckVariableDeclarationListAndValuesConsistency(VariableValueList oVariableValues)
        {
            foreach (VariableValue oVal in oVariableValues)
            {
                if (!m_oGlobalVariablesDeclaration.Contains(oVal.Name))
                {
                    throw new TisException("Error! RuleEvaluator:CheckVariableDeclarationListAndValuesConsistency Variable:"
                        + oVal.Name + " is not found in the VariableDeclarationList ");
                }
            }

        } 

        private StrongTypeRulesBase GetStrongTypedRule(Rule oRuleToGet)
        {
            // Try to get from cache
            StrongTypeRulesBase oStrongTypedRule =
                GetStrongTypedRuleFromCache(oRuleToGet);

            if (oStrongTypedRule == null)
            {
                // Adds to cache
                PrecompileRules(new Rule[] { oRuleToGet });
            }

            // Get from cache - now it should exist
            oStrongTypedRule =
                GetStrongTypedRuleFromCache(oRuleToGet);

            // Validate
            if (oStrongTypedRule == null)
            {
                throw new TisException(
                    "Fatal error: PrecompileRules called for rule [{0}]," +
                    "but the rule is not in cache",
                    oRuleToGet);
            }

            return oStrongTypedRule;
        }

        // in case such rule is not found, null will be returned
        private StrongTypeRulesBase GetStrongTypedRuleFromCache(Rule RuleToGet)
        {
            if (RuleToGet == null)
                throw new ArgumentNullException("Error! RuleEvaluator:GetRuleFromCache Rule can not be null");

            return (StrongTypeRulesBase)(m_oRulesCache[RuleToGet]);

        }

        private void AddStrongTypedRuleToCache(
            Rule oRule,
            StrongTypeRulesBase oStrongTypedRule)
        {
            m_oRulesCache[oRule] = oStrongTypedRule;
        }
    }
}
