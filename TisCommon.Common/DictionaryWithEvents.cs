using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon
{
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class DictionaryWithEvents<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [DataMember]
        private Dictionary<TKey, TValue> m_items = null;

        #region Constructor

        public DictionaryWithEvents()
            : this(0)
        {
        }

        public DictionaryWithEvents(int capacity)
        {
            this.Items = new Dictionary<TKey, TValue>(capacity);
        }

        #endregion

        #region Protected Properties

        protected Dictionary<TKey, TValue> Items 
        {
            get
            {
                return this.m_items;
            }
            private set
            {
                this.m_items = value;
            }
        }

        #endregion

        #region Events 

        public event EventHandler<DictionaryItemEventArgs<TKey, TValue>> ItemAdded;
        public event EventHandler<EventArgs> ItemRemoved;
        public event EventHandler<EventArgs> ItemsCleared;

        #endregion

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                m_items.Add(key, value);

                OnItemAdded(this, new DictionaryItemEventArgs<TKey, TValue>(key, value));
            }
        }

        public bool ContainsKey(TKey key)
        {
            return m_items.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get 
            {
                return m_items.Keys; 
            }
        }

        public bool Remove(TKey key)
        {
            bool isRemoved = m_items.Remove(key);

            if(isRemoved)
            {
                OnItemRemoved(this, new EventArgs());
            }

            return isRemoved;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_items.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get 
            { 
                return m_items.Values; 
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    return m_items[key];
                }

                return default(TValue);
            }
            set
            {
                m_items[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            m_items.Clear();

            OnItemsCleared(this, new EventArgs());
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return m_items.ContainsKey(item.Key) && 
                   m_items.ContainsValue(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int i = -1;

            foreach (KeyValuePair<TKey, TValue> kvp in m_items)
            {
                if (++i >= arrayIndex)
                {
                    array[i] = kvp;
                }
            }
        }

        public int Count
        {
            get 
            { 
                return m_items.Count; 
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (m_items as IEnumerable).GetEnumerator();
        }

        #endregion

        #region Event Methods

        protected virtual void OnItemAdded(object sender, DictionaryItemEventArgs<TKey, TValue> e)
        {
            if (ItemAdded != null)
            {
                ItemAdded(sender, e);
            }
        }

        protected virtual void OnItemRemoved(object sender, EventArgs e)
        {
            if (ItemRemoved != null)
            {
                ItemRemoved(sender, e);
            }
        }

        protected virtual void OnItemsCleared(object sender, EventArgs e)
        {
            if (ItemsCleared != null)
            {
                ItemsCleared(sender, e);
            }
        }

        #endregion

        public bool ContainsValue(TValue value)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> collection =
                from kvp in m_items
                where kvp.Value.Equals(value)
                select kvp;

            return collection.Count() > 0;
        }
    }

    public class DictionaryItemEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }

        public DictionaryItemEventArgs(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
   }

}
