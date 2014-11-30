using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Collections
{
    public interface IChildItemInternal<ParentType>
    {
        void SetParent(ParentType parent);
        void SetIndexInParent(int indexInParent);
    }
}
