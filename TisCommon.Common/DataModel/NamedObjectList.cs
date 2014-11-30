using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections;
using System.Reflection;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.DataModel
{
    public class NamedEventArgs<T> : EventArgs where T : INamedObject
    {
        public T NamedObject { get; set; }
        public NamedEventArgs(T nameObject)
        {
            NamedObject = nameObject;
        }
    }



    [Serializable]
    [ComVisible(false)]
	public class NamedObjectList<T> : System.Collections.ObjectModel.KeyedCollection<string, T>,
        INamedObjectList,
        IDeserializationCallback where T : INamedObject
    {
        public delegate void NamedEventHandlerT(object sender, NamedEventArgs<T> e);

        [field: IgnoreDataMember]
        public event NamedEventHandler OnObjectAdd;
        [field: IgnoreDataMember]
        public event NamedEventHandler OnObjectRemove;

        [field: IgnoreDataMember]
        public event NamedEventHandlerT OnAdd;
        [field: IgnoreDataMember]
        public event NamedEventHandlerT OnRemove;

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        protected NamedObjectList(SerializationInfo info, StreamingContext context)
        {
        }


        #region Member Variables

        [IgnoreDataMember]
        bool m_bEventsEnabled = true;

        [IgnoreDataMember]
        [NonSerialized]
        private IObjectNameProvider m_oNameProvider;

        #endregion

        #region Constructors

        public NamedObjectList()
            : base()
        {
            PreActivateTasks();
        }

        public NamedObjectList(int capacity)
            : this()
        {
            if (capacity < 0)
            {
                ExceptionUtil.RaiseArgumentOutOfRangeException(
                        "capacity",
                        capacity,
                        "Initial capacity cannot be less than zero.",
                        MethodInfo.GetCurrentMethod());
            }

            this.Capacity = capacity;

            PreActivateTasks();
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
        }

        #endregion

        #region Properties
        public object this[object key]
        {
            get
            {
                return this[GetStringKey(key, null)];
            }
            set
            {
                ValidateValue(value, null);

                this[GetStringKey(key, null)] =
                        (T)value;
            }
        }

        public new INamedObject this[string key]
        {
            get
            {

                if (base.Contains(key) == false)
                {
                    return default(T);
                }

                return base[key];
            }
            set
            {
                throw new TisException("NOT IMPLEMENTED, WILL BE REMOVED FROM INTERFACE");
            }
        }

        public virtual int Capacity
        {
            get
            {
                return Count;
            }
            set
            {
                return;
            }
        }

        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        public virtual object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region Public Methods

        public void SetObjectNameProvider(
                IObjectNameProvider oObjNameProvider)
        {
            m_oNameProvider = oObjNameProvider;
        }

        public virtual void Add(INamedObject value)
        {
            Add((T)value);
        }

        public new void Add(T value)
        {
            // NOTE: We cannot override InsertItem method!!!!

            string key = GetObjectName(value);

			if (Dictionary != null)
			{
				if ( Dictionary.ContainsKey(key) )
					ExceptionUtil.RaiseArgumentException(
						"value",
						"Object with key specified already exists",
						key,
						MethodInfo.GetCurrentMethod());
			}

			FireOnObjectAdd(value);

			base.Add(value);

			OnInsert(Count-1);
        }

        public virtual object Clone()
        {
            NamedObjectList<T> newList = new NamedObjectList<T>();

            CopyDataTo(newList);

            return newList;
        }

        public virtual bool ContainsKey(string key)
        {
			return base.Contains(key);
        }

        public virtual bool ContainsValue(T value)
        {
			return base.Contains(value);
        }

        public virtual void CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                ExceptionUtil.RaiseArgumentNullException(
                        "array",
                        "The destination array cannot be null.",
                        MethodInfo.GetCurrentMethod());
            }

            if (arrayIndex < 0)
            {
                ExceptionUtil.RaiseArgumentOutOfRangeException(
                        "arrayIndex",
                        arrayIndex,
                        "Destination index cannot be less than zero.",
                        MethodInfo.GetCurrentMethod());
            }

            if (array.Rank != 1)
            {
                ExceptionUtil.RaiseArgumentException(
                        "arrayIndex",
                        "Multidimensional arrays are not supported.",
                        "array.Rank",
                        MethodInfo.GetCurrentMethod());
            }

            if (arrayIndex >= array.Length)
            {
                ExceptionUtil.RaiseArgumentException(
                        "arrayIndex",
                        "Destination index cannot be greater than the size of the destination array.",
                        "array",
                        MethodInfo.GetCurrentMethod());
            }

            if (Count > (array.Length - arrayIndex))
            {
                ExceptionUtil.RaiseArgumentException(
                        "arrayIndex",
                        "Not enough available space in the destination array.",
                        "array",
                        MethodInfo.GetCurrentMethod());
            }

            Array.Copy(base.Items.ToArray(), arrayIndex, array, 0, Count);
        }

        public ICollection<string> Keys
        {
            get
            {
                return Dictionary.Keys;
            }
        }

        public virtual T GetByIndex(int index)
        {
            ValidateIndex(index, null);

            return base[index];
        }

        public virtual string NameByIndex(int index)
        {
            return GetKey(index);
        }

        public virtual string GetKey(int index)
        {
            ValidateIndex(index, null);

            return base[index].Name;
        }

        public virtual int IndexOfKey(string key)
        {
            if (Object.ReferenceEquals(key, null)) // avoids compiler error for null check on value type
            {
                ExceptionUtil.RaiseArgumentNullException(
                        "key",
                        "The key cannot be null.",
                        MethodInfo.GetCurrentMethod());
            }

            T item = default(T);
            if (Dictionary != null)
            {
                Dictionary.TryGetValue(key, out item);
            }

            int index = base.IndexOf(item);

            return index;
        }


        public virtual int IndexOfValue(T item)
        {
            int index = IndexOf(item);
            return index;
        }
        public virtual void RemoveObject(T oObj)
        {
            string sKey = GetObjectName(oObj);

            Remove(sKey);
        }

        public virtual void SetByIndex(int index, T value)
        {
            ValidateIndex(index, null);

            if (m_bEventsEnabled)
            {
                FireOnObjectRemove(base[index]);
                FireOnObjectAdd(value);
            }

            base[index] = value;
        }

        public virtual void TrimToSize()
        {
            this.Capacity = Count;
        }

        public virtual void ObjectRenamed(string prevName, string newName)
        {
            try
            {
                // Disable events
                EnableEvents(false);

                T obj = (T)this[prevName];

                if (obj == null)
                {
                    throw new TisException(
                            "Object [{0}] (New name: [{1}]) not in list", prevName, newName);
                }

                // NOTE1: We must remove/add the item, because the Remove/Add fires events that are required...
                // NOTE2: The ChangeItemKey uses GetKeyForItem, and because the GetKeyForItem is implemented to return the Item.Name,
                // and specifically in this scenario the GetKeyForItem should return the prevName, we will set the m_CurrentKeyForItem
                // the the GetKeyForItem method will return it.
                m_CurrentKeyForItem = prevName;
                this.ChangeItemKey(obj, newName);
                m_CurrentKeyForItem = null;

				int saveIndex = IndexOf(obj);
                this.Remove(newName);
				this.Insert(saveIndex, obj);
            }
            finally
            {
                EnableEvents(true);
            }
        }

        #endregion

        #region Private Methods

		public virtual void Insert(int index, INamedObject value)
		{
			Insert(index, (T)value);
		}
		public new void Insert(int index, T value)
        {
            FireOnObjectAdd(value);

            base.Insert(index, value);

            OnInsert(index);
        }

        private void EnsureCapacity(int min)
        {

        }

        public void EnableEvents(bool bEnabled)
        {
            m_bEventsEnabled = bEnabled;
        }

        protected bool EventsEnabled
        {
            get { return m_bEventsEnabled; }
        }

        protected virtual void FireOnObjectAdd(T oValue)
        {
            if (!EventsEnabled) return;

            if (OnObjectAdd != null)
            {
                OnObjectAdd(this, new NamedEventArgs(oValue));

            }

            if (OnAdd != null)
            {
                OnAdd(this, new NamedEventArgs<T>(oValue));
            }
        }

        protected virtual void FireOnObjectRemove(T oValue)
        {
            if (!EventsEnabled) return;

            if (OnObjectRemove != null)
            {
                OnObjectRemove(this, new NamedEventArgs(oValue));
            }

            if (OnRemove != null)
            {
                OnRemove(this, new NamedEventArgs<T>(oValue));
            }
        }

        protected virtual void OnInsert(int nIndex)
        {
            //OnAdd(this, new NameEventArgs() { NamedObject = Items[nIndex] });
        }

        protected virtual void OnDelete(int nIndex)
        {
        }

        protected void CopyDataTo(NamedObjectList<T> oOtherList)
        {
            foreach (var item in this)
            {
                oOtherList.Add((T)item);
            }
        }

        private void PreActivateTasks()
        {
            m_bEventsEnabled = true;
        }

        public string GetObjectName(T oObj)
        {
            return oObj.Name;
        }

        #endregion

        protected virtual void CapacityChanged()
        {
        }

        protected string GetStringKey(object key, MethodBase oMethod)
        {
            if (key == null)
            {
                return String.Empty;
            }

            if (!(key is string))
            {
                ExceptionUtil.RaiseArgumentException(
                        "key",
                        "key must be of 'string' type, provided type:" + key.GetType(),
                        key,
                        oMethod);
            }

            return (string)key;
        }

        protected void ValidateIndex(int index, MethodBase oMethod)
        {
            if (index < 0 || index >= Count)
            {
                ExceptionUtil.RaiseArgumentException(
                        "index",
                        "index must be between 0 and " + (Count - 1).ToString(),
                        index,
                        oMethod);
            }
        }

        protected void ValidateValue(object oValue, MethodBase oMethod)
        {
            if (!(oValue is INamedObject))
            {
                ExceptionUtil.RaiseArgumentException(
                        "value",
                        "value must implement 'INamedObject' interface",
                        oValue,
                        oMethod);
            }
        }


        private string m_CurrentKeyForItem = null;
        protected override string GetKeyForItem(T item)
        {
            if (m_CurrentKeyForItem != null)
            {
                return m_CurrentKeyForItem;
            }

            return item.Name;
        }

        #region INamedObjectList Members

        public new IEnumerator GetEnumerator()
        {
            var en = base.GetEnumerator();
            while (en.MoveNext() == true)
                yield return en.Current;
        }

        public new void Clear()
        {
            while (this.Count > 0)
            {
                RemoveAt(0);
            }
        }

        public new void Remove(string sName)
        {
            T item = Dictionary[sName];
            Remove(item);
        }

        protected override void RemoveItem(int index)
        {
            ValidateIndex(index, null);

            T item = base[index];

            FireOnObjectRemove(item);

            OnDelete(index);

            base.RemoveItem(index);
        }

        #endregion
    }
}
