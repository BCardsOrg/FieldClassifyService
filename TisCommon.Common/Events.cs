using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TiS.Core.TisCommon
{
    public class ItemCancelEventArgs : CancelEventArgs
    {
        private object m_Item;

        public ItemCancelEventArgs(object item)
        {
            m_Item = item;
        }

        public object Item
        {
            get
            {
                return m_Item;
            }
        }
    }
}
