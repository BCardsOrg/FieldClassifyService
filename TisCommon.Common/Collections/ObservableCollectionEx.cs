using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TiS.Core.TisCommon.Collections
{
    public class ObservableCollectionEx<ItemType> : ObservableCollection<ItemType>
    {
        public event EventHandler<NotifyCollectionChangedCancelEventArgs> CollectionChangingCancel;
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanging;
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChangedState;

        private bool m_IsBeginOperationExecuted = false;

        public ObservableCollectionEx()
        {
        }

        protected override void ClearItems()
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                Remove(this[0]);
            }
        }

        protected void BeginRemoveItem(int index, out bool isCancel)
        {
            isCancel = false;

            NotifyCollectionChangedCancelEventArgs e = new NotifyCollectionChangedCancelEventArgs(NotifyCollectionChangedAction.Remove, this[index], index);
            OnCollectionChangingCancel(e);
            if (e.IsCancel) 
            {
                isCancel = true;
                return; 
            }

            OnCollectionChanging(e);

            m_IsBeginOperationExecuted = true;            
        }

        protected override void RemoveItem(int index)
        {
            try
            {
                if (!m_IsBeginOperationExecuted)
                {
                    bool isCancel;
                    BeginRemoveItem(index, out isCancel);
                    if (isCancel) { return; }
                }

                base.RemoveItem(index);
            }
            finally 
            {
                m_IsBeginOperationExecuted = false;
            }
        }

        protected void BeginInsertItem(int index, ItemType item, out bool isCancel)
        {
            isCancel = false;

            NotifyCollectionChangedCancelEventArgs e = new NotifyCollectionChangedCancelEventArgs(NotifyCollectionChangedAction.Add, item, index);
            OnCollectionChangingCancel(e);
            if (e.IsCancel) 
            { 
                isCancel = true;
                return; 
            }

            OnCollectionChanging(e);

            m_IsBeginOperationExecuted = true;
        }

        protected override void InsertItem(int index, ItemType item)
        {
            try
            {
                if (!m_IsBeginOperationExecuted)
                {
                    bool isCancel;
                    BeginInsertItem(index, item, out isCancel);
                    if (isCancel) { return; }
                }

                base.InsertItem(index, item);
            }
            finally
            {
                m_IsBeginOperationExecuted = false;
            }
        }

        protected override void SetItem(int index, ItemType item)
        {
            Remove(this[index]);
            Insert(index, item);
        }

        protected virtual void OnCollectionChangingCancel(NotifyCollectionChangedCancelEventArgs e)
        {
            if (CollectionChangingCancel != null)
            {
                CollectionChangingCancel(this, e);
            }
        }

        protected virtual void OnCollectionChanging(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanging != null)
            {
                CollectionChanging(this, e);
            }
        }

        protected virtual void OnCollectionChangedState(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChangedState != null)
            {
                CollectionChangedState(this, e);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChangedState(e);

            base.OnCollectionChanged(e);
        }

    }
}
