using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon;
using System.Collections;

namespace TiS.Core.TisCommon.DataModel
{
    public class EntityTreeUtil
    {
        public static string[] GetObjectNames(ICollection oNamedObjects)
        {
            ArrayBuilder oNames = new ArrayBuilder(typeof(string));

            foreach (object oObj in oNamedObjects)
            {
                if (oObj is INamedObject)
                {
                    oNames.Add(((INamedObject)oObj).Name);
                }
            }

            return (string[])oNames.GetArray();
        }
    }
}
