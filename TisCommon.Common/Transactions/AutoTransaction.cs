using System;
using TiS.Core.TisCommon.Transactions;

namespace TiS.Core.TisCommon
{
    public class AutoTransaction : IDisposable
    {
        private TisTransactionManager m_transactionManager;

        protected bool m_shouldCommit = false;
        protected bool m_inTransaction = false;
        protected bool m_isDisposed = false;

        public enum Mode { NEW, CAN_USE_CURRENT }

      
        public AutoTransaction(TisTransactionManager transactionManager)
        {
            Init(transactionManager, Mode.CAN_USE_CURRENT);
        }

       
        public AutoTransaction(
            TisTransactionManager transactionManager,
            Mode enMode)
        {
            Init(transactionManager, enMode);
        }

        public void SetCommit()
        {
            m_shouldCommit = true;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            if (m_isDisposed)
            {
                return;
            }

            if (!m_inTransaction)
            {
                return;
            }

            bool isCommited = false;

            try
            {
                if (m_shouldCommit)
                {
                    m_transactionManager.ExecuteTransaction();

                    isCommited = true;
                }
            }
            finally
            {
                if (!isCommited)
                {
                    try
                    {
                        m_transactionManager.RollbackTransaction();
                    }
                    catch (Exception)
                    {
                    }
                }

                OnPostFinishTransaction();
            }

            m_isDisposed = true;
        }

        #endregion

        protected virtual void OnPostFinishTransaction()
        {
        }


        private void Init(
            TisTransactionManager transactionManager,
            Mode enMode)
        {
            m_transactionManager = transactionManager;

            if (!m_transactionManager.InTransaction)
            {
                m_transactionManager.PrepareTransaction();

                m_inTransaction = true;
            }
            else
                if (enMode == Mode.NEW)
                {
                    throw new TisException(
                        "Already in transaction (Mode.NEW specified)");
                }
        }

    }
}
