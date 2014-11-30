using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class RuleSyntaxCheckResult : ITisRuleSyntaxCheckResult
    {
		public RuleSyntaxCheckResult(
			bool succeeded,
			CompilerErrorInfo[] Errors)
		{
			Succeeded = succeeded;
            CompileErrors = new List<string>(Errors.Select(error => error.ToString()));
		}

        public IList CompileErrors { get; private set; }

        public bool Succeeded { get; private set; }
    }
}
