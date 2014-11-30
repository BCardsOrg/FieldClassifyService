using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Transactions;

namespace TiS.Core.TisCommon.Transactions
{
    [Guid("75441BE5-657B-438A-81E5-8A04CE644532")]
    [ServiceContract(Namespace = "http://www.topimagesystems.com/Core/TisCommon/Transactions/Transactable")]
    public interface ITransactable
    {
        [OperationContract]
        void PrepareTransaction();

        [OperationContract]
        void ExecuteTransaction();

        [OperationContract]
        void RollbackTransaction();

        bool InTransaction 
        {
            [OperationContract]
            get; 
        }
    }
}
