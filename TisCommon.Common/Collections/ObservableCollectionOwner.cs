using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Collections
{
    public class ObservableCollectionOwner<ItemType, OwnerType> : ObservableCollectionEx<ItemType>
    {
        // NOTE: The m_Owner should be public so the owner object will set it during deserialization.
        // The owner object should not call Owner property set, because it might cause duplicate keyes if the items of the collections
        // has initialization code in the SetParent methods.
        public OwnerType m_Owner;

        public ObservableCollectionOwner() : base()
        {
        }

        public ObservableCollectionOwner(OwnerType owner) : this()
        {
            m_Owner = owner;
        }

        public OwnerType Owner
        {
            get
            {
                return m_Owner;
            }
            internal set
            {
                m_Owner = value;

                foreach (var item in this)
                {
                    (item as IChildItemInternal<OwnerType>).SetParent(m_Owner);
                }
            }
        }
        
        protected override void RemoveItem(int index)
        {
            bool isCancel;
            base.BeginRemoveItem(index, out isCancel);
            if (isCancel) { return; }

            IChildItemInternal<OwnerType> childItem = this[index] as IChildItemInternal<OwnerType>;
            childItem.SetParent(default(OwnerType));
            childItem.SetIndexInParent(-1);
            SetIndexes(index + 1, index);

            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, ItemType item)
        {
            bool isCancel;
            base.BeginInsertItem(index, item, out isCancel);
            if (isCancel) { return; }

            IChildItemInternal<OwnerType> childItem = item as IChildItemInternal<OwnerType>;

            childItem.SetIndexInParent(index);
            SetIndexes(index, index + 1);            

            // NOTE: The m_Owner will be null only in deserialization, and in this case we should not call SetParent...
            if (m_Owner != null)
            {
                childItem.SetParent(m_Owner);               
            }

            base.InsertItem(index, item);
        }

        private void SetIndexes(int fromItemIndex, int startIndex)
        {
            for (int i = fromItemIndex; i < Count; i++)
            {
                IChildItemInternal<OwnerType> item = this[i] as IChildItemInternal<OwnerType>;
                item.SetIndexInParent(startIndex);
                startIndex++;
            }
        }
    }
}
