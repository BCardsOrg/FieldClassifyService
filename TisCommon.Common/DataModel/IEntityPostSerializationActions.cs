using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.DataModel
{
    public interface IEntityPostSerializationActions
    {
		object GetParent();
        void SetParent(object parent);
		IEnumerable<object> GetAggregatedChildren();
        IEnumerable<INamedObjectList> GetOwnedChildrenLists();
        IEnumerable<INamedObjectList> GetLinkedChildrenLists();
    }
}
