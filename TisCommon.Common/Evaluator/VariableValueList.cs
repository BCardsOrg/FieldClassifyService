using System;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class VariableValueList : NameObjectCollectionBase
    {
        public VariableValueList()
        {
        }

        public void Add(VariableValue oVariableValue)
        {
            BaseAdd(oVariableValue.Name, oVariableValue);
        }

        public void Add(string Name, object oValue)
        {
            this.Add(new VariableValue(Name, oValue));
        }

        // it is possible that the name of the meta tag is the same, but the
        //	type is different - the ContainMetaTag will return false in this case
        public bool Contains(string sName)
        {
            return this[sName] != null;
        }

        // returns null if no entry is found
        public VariableValue this[string Name]
        {
            get
            {
                return (VariableValue)BaseGet(Name);
            }
        }

        public new IEnumerator GetEnumerator()
        {
            return BaseGetAllValues(typeof(VariableValue)).GetEnumerator();
        }

        #region Get the value of specific meta tags

        public object GetValue(string sName)
        {
            VariableValue oVariable = this[sName];

            if (oVariable == null)
                throw new ArgumentNullException("Error! VariableValueList:GetValue oVariable " + sName + " does not exist ");

            return oVariable.Value;
        }

        public int GetInt(string sName)
        {
            return (int)(this.GetValue(sName));
        }

        public bool GetBool(string sName)
        {
            return (bool)(this.GetValue(sName));
        }

        public string GetString(string sName)
        {
            return (string)(this.GetValue(sName));
        }

        public float GetFloat(string sName)
        {
            return (float)(this.GetValue(sName));
        }

        public double GetDouble(string sName)
        {
            return (double)(this.GetValue(sName));
        }

        #endregion
    }
}
