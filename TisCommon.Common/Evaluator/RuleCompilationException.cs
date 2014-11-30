using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    // This excpetion will be thrown once the compilation of the rule returned
    //	error, so the rule as an argument is not valid
    [ComVisible(false)]
    public class RuleCompilationException : ArgumentException, ISerializable
    {
        // Holds CompilerErrorInfo objects
        private ArrayList m_oErrorInfoList = new ArrayList();
        private string[] m_NativeCompilerOutput = EmptyArrays.StringArray;
        private string m_sCompiledCode = String.Empty;

        public RuleCompilationException(
            StringCollection oNativeCompilerOutput,
            string sCompiledCode)
        {
            m_sCompiledCode = sCompiledCode;
            SetNativeCompilerOutput(oNativeCompilerOutput);
        }

        public RuleCompilationException(
            Exception InnerException,
            StringCollection oNativeCompilerOutput,
            string sCompiledCode) :
            base(String.Empty, InnerException)
        {
            m_sCompiledCode = sCompiledCode;
            SetNativeCompilerOutput(oNativeCompilerOutput);
        }

        private RuleCompilationException(
            SerializationInfo info,
            StreamingContext context) :
            base(info, context)
        {
            m_oErrorInfoList = (ArrayList)info.GetValue(
                "m_oErrorInfoList",
                typeof(ArrayList));

            m_NativeCompilerOutput = (string[])info.GetValue(
                "m_NativeCompilerOutput",
                typeof(string[]));

            m_sCompiledCode = info.GetString("m_sCompiledCode");
        }


        public void AddErrorInfo(
            bool bIsWarning,
            string sErrorMessage,
            string sErrorNumber,
            int nSourceColumn,
            string sSourceFile,
            int nSourceLine)
        {
            m_oErrorInfoList.Add(new CompilerErrorInfo(
                bIsWarning,
                sErrorMessage,
                sErrorNumber,
                nSourceColumn,
                sSourceFile,
                nSourceLine));
        }

        public string CompiledCode
        {
            get { return m_sCompiledCode; }
        }

        public CompilerErrorInfo[] CompilerErrors
        {
            get
            {
                return (CompilerErrorInfo[])ArrayBuilder.CreateArray(
                    m_oErrorInfoList,
                    typeof(CompilerErrorInfo),
                    null);
            }
        }

        public string[] NativeCompilerOutput
        {
            get { return m_NativeCompilerOutput; }
        }

        void ISerializable.GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("m_oErrorInfoList", m_oErrorInfoList);
            info.AddValue("m_NativeCompilerOutput", m_NativeCompilerOutput);
            info.AddValue("m_sCompiledCode", m_sCompiledCode);
        }

        public override string Message
        {
            get
            {
                StringBuilder oMsg = new StringBuilder();

                oMsg.AppendFormat("{0} compilation errors found while compiling code [{1}]\n", m_oErrorInfoList.Count, m_sCompiledCode);

                foreach (CompilerErrorInfo oErrInfo in m_oErrorInfoList)
                {
                    oMsg.AppendFormat("{0}\n", oErrInfo.ToString());
                }

                oMsg.Append("\nNative output:\n");

                foreach (string sNativeOutLine in m_NativeCompilerOutput)
                {
                    oMsg.AppendFormat("{0}\n", sNativeOutLine);
                }

                return oMsg.ToString();
            }
        }


        //
        //	Private
        //

        private void SetNativeCompilerOutput(StringCollection oNativeCompilerOutput)
        {
            m_NativeCompilerOutput = new string[oNativeCompilerOutput.Count];
            oNativeCompilerOutput.CopyTo(m_NativeCompilerOutput, 0);
        }

    } // end of public class RuleCompilationErrorsException
}
