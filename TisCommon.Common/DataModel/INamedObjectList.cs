using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon;
using System.Collections;

namespace TiS.Core.TisCommon.DataModel
{
    public class NamedEventArgs : EventArgs
    {
        public INamedObject NamedObject { get; set; }
        public NamedEventArgs(INamedObject nameObject)
        {
            NamedObject = nameObject;
        }
    }

    public delegate void NamedEventHandler(object sender, NamedEventArgs e);

    [Guid("61CB2FFC-01E5-4706-A0D7-BF0F9CC9E348")]
    public interface INamedObjectList : ICollection, ICloneable
    {
        event NamedEventHandler OnObjectAdd;
        event NamedEventHandler OnObjectRemove;
        INamedObject this[string sName] { get; set; }
        bool Contains(string sName);
        void Remove(string sName);
        void Insert(int index, INamedObject value);
		void Add(INamedObject value);
		string NameByIndex(int nIndex);
        void ObjectRenamed(string sPrevName, string sNewName);
        void Clear();
    }


    [Guid("6B05A3BA-E023-4F1A-8821-9FB4DE3CE349")]
    public interface INamedObjectOwnerList : INamedObjectList
    {
        INamedObject CreateNew(string sName);
    }


    [Guid("9EEAE7FB-115A-47CE-AB01-4DC8C9B434E3")]
    public interface INamedObjectOrder
    {

        int GetOrder(int nIndex);


        void SetOrder(string sObjName, int nNewOrder);
        string GetNameByOrder(int nOrder);

        int GetOrderByName(string sObjName);

        int GetIndexByOrder(int nOrder);

        INamedObject GetByOrder(int nOrder);

        IEnumerator GetOrderedEnumerator();

    }
}
