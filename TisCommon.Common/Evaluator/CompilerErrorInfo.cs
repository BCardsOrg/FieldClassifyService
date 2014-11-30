using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class CompilerErrorInfo
    {
        public readonly bool IsWarning;
        public readonly string ErrorMessage;
        public readonly string ErrorNumber;
        public readonly int SourceColumn;
        public readonly string SourceFile;
        public readonly int SourceLine;

        public CompilerErrorInfo(
            bool bIsWarning,
            string sErrorMessage,
            string sErrorNumber,
            int nSourceColumn,
            string sSourceFile,
            int nSourceLine)
        {
            this.IsWarning = bIsWarning;
            this.ErrorMessage = sErrorMessage;
            this.ErrorNumber = sErrorNumber;
            this.SourceColumn = nSourceColumn;
            this.SourceFile = sSourceFile;
            this.SourceLine = nSourceLine;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1} ({2}) at Line {3}, Column {4}, File {5}",
                IsWarning ? "Warning" : "Error",
                ErrorNumber,
                ErrorMessage,
                SourceLine,
                SourceColumn,
                SourceFile);
        }
    }
}
