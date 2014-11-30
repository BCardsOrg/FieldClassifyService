using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Transactions
{
    #region TMRollbackSpecificEventArgs

    [Serializable]
    [ComVisible(true)]
    public class TransactionFailureReason : EventArgs
    {
        private Exception m_reason;

        public TransactionFailureReason(Exception reason)
            : base()
        {
            m_reason = reason;
        }

        public Exception Reason
        {
            get { return m_reason; }
        }
    }

    #endregion

    public delegate void TMRollbackSpecificEvent(object sender, TransactionFailureReason reason, ref bool done);

    public delegate void TransactionManagerEvent(object sender, EventArgs oArgs);

    public class TisTransactionManager
    {
        private List<ITransactable> m_transactionMembers = new List<ITransactable>();

        [ThreadStatic]
        private TransactionData m_transactionData;

        public event TransactionManagerEvent OnPrepareTransaction;
        public event TransactionManagerEvent OnExecuteTransaction;
        public event TransactionManagerEvent OnRollbackTransaction;
        public event TMRollbackSpecificEvent OnRollbackTransactionSpecific;
        public event TransactionManagerEvent OnClearTransaction;

        public TisTransactionManager()
        {
        }

        public TisTransactionManager(ITransactable member)
        {
            m_transactionMembers.Add(member);
        }

        public void AddTransactionMember(ITransactable member)
        {
            lock (this)
            {
                if (!m_transactionMembers.Contains(member))
                {
                    m_transactionMembers.Add(member);
                }
            }
        }

        public void AddTransactionMember(TisTransactionManager transactionManager)
        {
            lock (this)
            {
                foreach (ITransactable member in transactionManager.m_transactionMembers)
                {
                    AddTransactionMember(member);
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                m_transactionMembers.Clear();
            }
        }

        #region Add Tasks

        public void AddCommitTask(ITask oTask)
        {
            m_transactionData.ExecuteTasks.AddTask(oTask);
        }

        public void AddPostCommitTask(ITask oTask)
        {
            m_transactionData.PostCompleteTasks.AddTask(oTask);
        }

        public void AddRollbackTask(ITask oTask)
        {
            m_transactionData.RollbackTasks.AddTask(oTask);
        }

        private void PerformCommitTasks()
        {
            m_transactionData.ExecuteTasks.Perform();
        }

        private void PerformPostCommitTasks()
        {
            m_transactionData.PostCompleteTasks.Perform();

            m_transactionData.ExecuteTasks.Clear();
        }

        private void PerformRollbackTasks()
        {
            m_transactionData.RollbackTasks.Perform();

            m_transactionData.ExecuteTasks.Clear();
        }

        #endregion

        public void ExecuteTransaction()
        {
            try
            {
                using (TransactionScope scope = TisTransactionScope.Get)
                {
                    Execute();

                    scope.Complete();

                    try
                    {
                        PerformPostCommitTasks();
                    }
                    catch (Exception exc)
                    {
                        throw new TisException(
                            exc,
                            "Fatal error: PostComplete tasks should not throw exceptions");
                    }
                }
            }
            catch (Exception oExc)
            {
                TransactionFailureReason reason = new TransactionFailureReason(oExc);
                bool done = false;

                RollbackTransaction(reason, ref done);

                if (!done)
                {
                    throw;
                }
            }
            finally
            {
                // Clear transaction
                ClearTransaction();
            }
        }

        public void ClearTransaction()
        {
            lock (this)
            {
                m_transactionData = null;

                // Notify
                NotifyClearTransaction();
            }
        }

        private void NotifyRollbackTransaction(TransactionFailureReason reason, ref bool done)
        {
            if (OnRollbackTransactionSpecific != null)
            {
                OnRollbackTransactionSpecific(this, reason, ref done);
            }

            if (OnRollbackTransaction != null)
            {
                OnRollbackTransaction(this, reason);
            }
        }

        private void NotifyClearTransaction()
        {
            if (OnClearTransaction != null)
            {
                OnClearTransaction(this, new EventArgs());
            }
        }

        public void RollbackTransaction()
        {
            TransactionFailureReason reason = new TransactionFailureReason(null);

            bool done = false;

            RollbackTransaction(reason, ref done);
        }

        private void RollbackTransaction(TransactionFailureReason reason, ref bool done)
        {
            lock (this)
            {
                // Call RollbackTransaction for each transaction member
                foreach (ITransactable member in m_transactionMembers)
                {
                    try
                    {
                        member.RollbackTransaction();
                    }
                    catch (Exception exc)
                    {
                        Log.WriteException(exc);
                    }
                }

                NotifyRollbackTransaction(reason, ref done);

                PerformRollbackTasks();
                ClearTransaction();
            }
        }

        private void NotifyRollbackTransaction()
        {
            if (OnRollbackTransaction != null)
            {
                OnRollbackTransaction(this, new EventArgs());
            }
        }

        public void PrepareTransaction()
        {
            m_transactionData = new TransactionData();

            if (OnPrepareTransaction != null)
            {
                OnPrepareTransaction(this, new EventArgs());
            }

            lock (this)
            {
                foreach (ITransactable member in m_transactionMembers)
                {
                    member.PrepareTransaction();
                }
            }
        }

        private void Execute()
        {
            if (OnExecuteTransaction != null)
            {
                OnExecuteTransaction(this, EventArgs.Empty);
            }

            PerformCommitTasks();

            lock (this)
            {
                foreach (ITransactable member in m_transactionMembers)
                {
                    member.ExecuteTransaction();
                }
            }
        }

        public bool InTransaction
        {
            get
            {
                lock (this)
                {
                    return m_transactionData != null;
                }
            }
        }

        public void ClearRollbackTasks()
        {
            m_transactionData.RollbackTasks.Clear();
        }
    }

    #region TransactionData

    public class TransactionData
    {
        private TaskBatch m_executeTasks = new TaskBatch();
        private TaskBatch m_postCompleteTasks = new TaskBatch();
        private TaskBatch m_rollbackTasks = new TaskBatch();


        public TaskBatch ExecuteTasks
        {
            get { return m_executeTasks; }
        }

        public TaskBatch PostCompleteTasks
        {
            get { return m_postCompleteTasks; }
        }

        public TaskBatch RollbackTasks
        {
            get { return m_rollbackTasks; }
        }
    }

    #endregion
}
