using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TiS.Core.TisCommon.DataModel
{
    public class NameTypeLookupTable
    {
        private const int TABLE_SIZE = 100;

        private static IObjectNameProvider m_oDefaultNameProvider =
            new DefaultObjectNameProvider();

        private Hashtable m_oTable = new Hashtable(TABLE_SIZE);
        private IObjectNameProvider m_oNameProvider = m_oDefaultNameProvider;

        public NameTypeLookupTable()
        {
        }

        public NameTypeLookupTable(IObjectNameProvider oDefaultNameProvider)
        {
            m_oDefaultNameProvider = oDefaultNameProvider;
        }

        public object Get(Type oType, string sName)
        {
            return m_oTable[GetKey(oType, sName)];
        }

        public void Clear()
        {
            m_oTable.Clear();
        }

        public void AddRange(IEnumerable<object> oObjects)
        {
            foreach (object oObj in oObjects)
            {
                m_oTable[GetKey(oObj.GetType(), GetObjectName(oObj))] = oObj;
            }
        }

        //
        //	Private
        //

        private string GetObjectName(object oObj)
        {
            return m_oNameProvider.GetObjectName(oObj);
        }

        private string GetKey(Type oType, string sName)
        {
            return String.Concat(oType.FullName, "$", sName);
        }
    }
}
