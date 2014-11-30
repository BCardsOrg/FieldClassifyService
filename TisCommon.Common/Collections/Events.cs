using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace TiS.Core.TisCommon.Collections
{
    public class NotifyCollectionChangedCancelEventArgs : NotifyCollectionChangedEventArgs
    {
        private bool m_IsCancel = false;

        public NotifyCollectionChangedCancelEventArgs(NotifyCollectionChangedAction action)
            : base(action)
        {
        }

        public NotifyCollectionChangedCancelEventArgs(NotifyCollectionChangedAction action, object item, int index)
            : base(action, item, index)
        {
        }

        public NotifyCollectionChangedCancelEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            : base(action, newItems, oldItems)
        {
        }

        public bool IsCancel
        {
            get
            {
                return m_IsCancel;
            }
            set
            {
                m_IsCancel = value;
            }
        }
    }
}
