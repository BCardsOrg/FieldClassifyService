using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

namespace TiS.Core.TisCommon.Evaluator
{
    public interface IRuleEvaluator
    {
        bool RuleTrue(VariableValueList oVariableValues, Rule RuleToEvaluate);
        bool CheckRuleSyntax(Rule RuleToEvaluate, out CompilerErrorInfo[] CompilerErrors);
        ITisRuleSyntaxCheckResult CheckRuleSyntax(string sRule);
    }

    [Guid("9125483C-175E-4C9C-9B55-B0B7DD41F99E")]
    public interface ITisRuleSyntaxCheckResult
    {
        IList CompileErrors { get; }
        bool Succeeded { get; }
    }
}
