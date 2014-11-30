using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace TiS.Core.TisCommon.Transactions
{
    public class TisTransactionScope
    {
        public static TransactionScope Get
        {
            get
            {
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = IsolationLevel.ReadCommitted;

                return new TransactionScope(TransactionScopeOption.Required, options);
            }
        }
    }
}
