using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TiS.Core.TisCommon.Collections
{
    public abstract class ObservableCollectionKey<ItemType> : ObservableCollectionEx<ItemType>
    {
        private IDictionary<string, ItemType> m_KeyItemMap = new Dictionary<string, ItemType>();

        public ObservableCollectionKey()
            : base()
        {
        }

        protected ItemType GetByKey(string key)
        {
            ItemType result;
            m_KeyItemMap.TryGetValue(key, out result);
            return result;
        }

        protected abstract string GetKeyForItem(ItemType item);

        protected override void InsertItem(int index, ItemType item)
        {
            bool isCancel;
            base.BeginInsertItem(index, item, out isCancel);
            if (isCancel) { return; }

            string key = GetKeyForItem(item);
            m_KeyItemMap.Add(key, item);

            base.InsertItem(index, item);
        }

        public bool RemoveByKey(string key)
        {
            ItemType item;
            if (!m_KeyItemMap.TryGetValue(key, out item))
            {
                return false;
            }

            Remove(item);
            return true;
        }

        protected override void RemoveItem(int index)
        {
            bool isCancel;
            base.BeginRemoveItem(index, out isCancel);
            if (isCancel) { return; }

            string key = GetKeyForItem(this[index]);
            m_KeyItemMap.Remove(key);

            base.RemoveItem(index);
        }
    }
}
