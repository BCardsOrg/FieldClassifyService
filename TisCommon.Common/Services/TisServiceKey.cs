using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Services
{
    [ComVisible(false)]
    public class TisServiceKey
    {
        public TisServiceKey(
            string sServiceName, 
            string sInstanceName,
            ITisServiceInfo serviceInfo = null,
            object arbitraryInfo = null)
        {
            if (sServiceName == null)
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sServiceName",
                    "can't be null",
                    System.Reflection.MethodInfo.GetCurrentMethod());
            }

            if (sInstanceName == null)
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sInstanceName",
                    "can't be null",
                    System.Reflection.MethodInfo.GetCurrentMethod());
            }

            ServiceName = sServiceName;
            InstanceName = sInstanceName;
            ServiceInfo = serviceInfo;
            ArbitraryInfo = arbitraryInfo;
        }

        public string InstanceName {get; private set;}
        public string ServiceName { get; private set; }
        public ITisServiceInfo ServiceInfo { get; private set; }
        public object ArbitraryInfo { get; private set; }

        public override bool Equals(Object obj)
        {
            TisServiceKey oOther = obj as TisServiceKey;

            if (Object.ReferenceEquals(oOther, null))
            {
                return false;
            }

            if (!StringUtil.CompareIgnoreCase(
                ServiceName,
                oOther.ServiceName))
            {
                return false;
            }

            if (!StringUtil.CompareIgnoreCase(
                InstanceName,
                oOther.InstanceName))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return ServiceName.GetHashCode();
        }

        public static bool operator ==(TisServiceKey o1, TisServiceKey o2)
        {
            if (Object.ReferenceEquals(o1, null))
            {
                return false;
            }

            return o1.Equals(o2);
        }

        public static bool operator !=(TisServiceKey o1, TisServiceKey o2)
        {
            return !(o1 == o2);
        }

        public int CompareTo(
            object oObj)
        {
            if (Object.ReferenceEquals(oObj, null))
            {
                return -1;
            }

            if (this.Equals(oObj))
            {
                return 0;
            }

            return this.GetHashCode() - oObj.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "(ServiceName=[{0}], InstanceName=[{1}])",
                ServiceName,
                InstanceName);
        }

    }
}
