using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.Process
{
    #region IProcessProperties

    [Guid("99C6829F-53BD-4642-81CA-FD1990A4F7DA")]
    public interface IProcessProperties
    {
        string MachineName { get; }
        int ProcessId { get; }
        string UniqueProcessId { get; }
    }

    #endregion

    #region ProcessProperties

    [ComVisible(false)]
    [Serializable]
    public class ProcessProperties : IProcessProperties
    {
        private string m_sMachineName;
        private int m_nProcessId;
        private string m_sUniqueProcessId;

        //
        //	Public
        //

        public ProcessProperties(
            string sMachineName,
            int nProcessId,
            string sUniqueProcessId)
        {
            Init(sMachineName, nProcessId, sUniqueProcessId);
        }

        public ProcessProperties()
        {
            Init(
                Environment.MachineName,
                ProcessUtil.CurrentProcessId,
                ProcessUtil.UniqueProcessId);
        }

        public string MachineName
        {
            get
            {
                return m_sMachineName;
            }
        }

        public int ProcessId
        {
            get
            {
                return m_nProcessId;
            }
        }

        public string UniqueProcessId
        {
            get
            {
                return m_sUniqueProcessId;
            }
        }

        public static IProcessProperties Current
        {
            get { return new ProcessProperties(); }
        }

        public override string ToString()
        {
            return m_sUniqueProcessId;
        }


        //
        //	Private
        //

        private void Init(
            string sMachineName,
            int nProcessId,
            string sUniqueProcessId)
        {
            m_sMachineName = sMachineName;
            m_nProcessId = nProcessId;
            m_sUniqueProcessId = sUniqueProcessId;
        }

    }

    #endregion
}
