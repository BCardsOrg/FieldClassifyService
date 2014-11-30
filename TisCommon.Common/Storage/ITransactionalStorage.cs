using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Transactions;
using TiS.Core.TisCommon.Transactions;

namespace TiS.Core.TisCommon.Storage
{
    [ServiceContract(Namespace = "http://www.topimagesystems.com/Core/TisCommon/Storage/TransactionalStorage")]
    public interface ITransactionalStorage : IStorageService, ITransactable
    {
    }
}
