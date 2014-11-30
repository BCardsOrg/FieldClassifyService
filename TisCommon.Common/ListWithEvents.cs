using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon
{
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class ListWithEvents<T> : IList<T>
    {
        [DataMember]
        private List<T> m_items = null;

        #region Events

        public event EventHandler<ListItemEventArgs<T>> ItemAdded;
        public event EventHandler<EventArgs> ItemRemoved;
        public event EventHandler<EventArgs> ItemsCleared;

        #endregion

        #region Protected Properties

        protected List<T> Items
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

        #region Constructor

        public ListWithEvents()
            : this(0)
        {
        }

        public ListWithEvents(int capacity)
        {
            this.Items = new List<T>(capacity);
        }

        #endregion

        #region IList Methods

        public T this[int index] 
        {
            get 
            { 
                return this.Items[index]; 
            }
            set 
            { 
                this.Items[index] = value; 
            }
        }

        public int IndexOf(T item)
        {
            return this.Items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Items.Insert(index, item);

            OnItemAdded(this, new ListItemEventArgs<T>(item));
        }

        public void RemoveAt(int index)
        {
            this.Items.RemoveAt(index);

            OnItemRemoved(this, new EventArgs());
        }

        #endregion

        #region ICollection Methods and Properties

        public int Count 
        { 
            get 
            { 
                return this.Items.Count; 
            } 
        }

        public bool IsReadOnly 
        { 
            get 
            { return false; 
            } 
        }

        public void Add(T item)
        {
            if (!this.Contains(item))
            {
                this.Items.Add(item);

                OnItemAdded(this, new ListItemEventArgs<T>(item));
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                this.Add(item);
            }
        }

        public void Clear()
        {
            this.Items.Clear();

            OnItemsCleared(this, new EventArgs());
        }

        public bool Contains(T item)
        {
            return this.Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool happened = false;

            if (this.Contains(item))
            {
                happened = this.Items.Remove(item);

                OnItemRemoved(this, new EventArgs());
            }

            return happened;
        }

        #endregion

        #region IEnumerable<T> Methods

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Event Methods

        protected virtual void OnItemAdded(object sender, ListItemEventArgs<T> e)
        {
            if (ItemAdded != null)
                ItemAdded(sender, e);
        }

        protected virtual void OnItemRemoved(object sender, EventArgs e)
        {
            if (ItemRemoved != null)
                ItemRemoved(sender, e);
        }

        protected virtual void OnItemsCleared(object sender, EventArgs e)
        {
            if (ItemsCleared != null)
                ItemsCleared(sender, e);
        }

        #endregion

        public T[] ToArray()
        {
            return m_items.ToArray();
        }
    }

    public class ListItemEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }

        public ListItemEventArgs(T item)
        {
            this.Item = item;
        }
    }
}
