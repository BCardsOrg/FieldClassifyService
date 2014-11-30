using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Runtime.Serialization;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.DataModel
{
    #region NamedObjectListOrderedEnumerator

    internal class NamedObjectListOrderedEnumerator<T> : IEnumerator where T : INamedObject
    {
        private NamedObjectListWithOrder<T> m_oList;
        private int m_nOrder;

        public NamedObjectListOrderedEnumerator(NamedObjectListWithOrder<T> oList)
        {
            m_oList = oList;

            Reset();
        }

        #region IEnumerator Members

        public void Reset()
        {
            m_nOrder = -1;
        }

        public object Current
        {
            get
            {
                ValidatePosition();

                return m_oList.GetByOrder(m_nOrder);
            }
        }

        public bool MoveNext()
        {
            if (!IsEOL)
            {
                m_nOrder++;
            }

            return !IsEOL;
        }

        #endregion

        //
        //	Private
        //

        private bool IsEOL
        {
            get { return (m_nOrder >= m_oList.Count); }
        }

        private void ValidateNotEOL()
        {
            if (IsEOL)
            {
                throw new TisException(
                    "Invalid current position, after end of list");
            }
        }

        private void ValidatePosition()
        {
            if (m_nOrder < 0)
            {
                throw new TisException("Invalid current position, {0}", m_nOrder);
            }

            ValidateNotEOL();
        }
    }

    #endregion

    #region NamedObjectListWithOrder

    [Serializable]
    [ComVisible(false)]
    public class NamedObjectListWithOrder<T> : NamedObjectList<T> where T : INamedObject
    {
        public NamedObjectListWithOrder()
        {
        }

        public int GetOrder(int nIndex)
        {
            return nIndex;
        }

        public void SetOrder(string sObjName, int nNewOrder)
        {
            SetOrder(GetOrderByName(sObjName), nNewOrder);
        }

        public void SetOrder(int nOriginalOrder, int nNewOrder)
        {
			EnableEvents(false);
			T item = GetByIndex(nOriginalOrder);
			base.RemoveAt(nOriginalOrder);
			base.InsertItem(nNewOrder, item);
			EnableEvents(true);
        }

        public virtual INamedObject GetByOrder(int nOrder)
        {
            int nIndex = GetIndexByOrder(nOrder);

            return base.GetByIndex(nIndex);
        }

        public virtual string GetNameByOrder(int nOrder)
        {
            return GetByOrder(nOrder).Name;
        }

        public virtual int GetOrderByName(string sName)
        {
            int nIndex = IndexOfKey(sName);

            if (nIndex < 0)
            {
                throw new ArgumentException(
                    "Object with name [" + sName + "] doesn't exists");
            }

			return nIndex;
        }

        public override object Clone()
        {
            NamedObjectListWithOrder<T> newList = new NamedObjectListWithOrder<T>();
            base.CopyDataTo(newList);

            return newList;
        }

        public int GetIndexByOrder(int nOrder)
        {
			return nOrder;
        }

        public IEnumerator GetOrderedEnumerator()
        {
            return new NamedObjectListOrderedEnumerator<T>(this);
        }

        public override void ObjectRenamed(string sPrevName, string sNewName)
        {
            // Order should not change after rename

            int nOriginalOrder = GetOrderByName(sPrevName);

            base.ObjectRenamed(sPrevName, sNewName);

            SetOrder(sNewName, nOriginalOrder);
        }

        protected int MaxOrder
        {
            get { return Count - 1; }
        }

        private static void ArrayInsertElement(
            Array oArray,
            int nIndex)
        {
            Array.Copy(
                oArray,
                nIndex,				// SrcIndex
                oArray,
                nIndex + 1,			   // DstIndex
                oArray.Length - nIndex - 1	// Length
                );
        }

    }

    #endregion
}
