using System;
using System.Threading;
using System.Runtime.InteropServices;

//namespace TiS.Core.TisCommon
//{
//    /// <summary>
//    /// Summary description for TransactionContext.
//    /// </summary>
//    [ComVisible(false)]
//    public class TransactionContext
//    {
//        const string TX_DATASLOT_NAME = "TransactionSlot";
//        const string TX_MANAGER_NAME  = "TransactionContext";

//        [ThreadStatic]
//        private static TransactionManager m_oTxManager;

//        [ThreadStatic]
//        private static object m_tx;

//        public static object Id
//        {
//            get
//            {
//                return Get ();
//            }
//            set
//            {
//                Set (value);
//            }
//        }

//        [CLSCompliant(false)]
//        public static void BeginTransaction(string sAppName, ITransactable oTransactable)
//        {
//            CreateTransactionManager (sAppName);

//            m_oTxManager.AddTransactionMember (oTransactable);

//            m_oTxManager.BeginTransaction ();
//        }

//        [CLSCompliant(false)]
//        public static void BeginTransaction(ITransactable oTransactable)
//        {
//            BeginTransaction (String.Empty, oTransactable);
//        }

//        public static void CommitTransaction ()
//        {
//            m_oTxManager.CommitTransaction ();
//        }

//        public static void RollbackTransaction ()
//        {
//            m_oTxManager.RollbackTransaction ();
//        }

//        public static bool InTransaction ()
//        {
//            return (m_oTxManager != null && m_oTxManager.InTransaction);
//        }

//        //
//        //	Private
//        //

//        private static void CreateTransactionManager (string sAppName)
//        {
//            if (m_oTxManager == null)
//            {
//                m_oTxManager = new TransactionManager (TX_MANAGER_NAME);

//                m_oTxManager.AppName = sAppName;

//                m_oTxManager.OnClearTransaction +=new TransactionManagerEvent(m_oTxManager_OnClearTransaction);
//            }
//            else
//            {
//                m_oTxManager.Clear ();
//            }
//        }

//        private static void m_oTxManager_OnClearTransaction(object sender, EventArgs oArgs)
//        {
//            m_oTxManager.Clear ();
//        }

//        private static object Get ()
//        {
//            return m_tx;
//        }

//        private static void Set (object oTx)
//        {
//            m_tx = oTx;
//        }
//    }
//}
