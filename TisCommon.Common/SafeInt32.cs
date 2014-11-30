using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TiS.Core.TisCommon
{
    public class SafeInt32
    {
        private int m_nValue;

        public SafeInt32()
        {
        }

        public SafeInt32(int nValue)
        {
            Value = nValue;
        }

        public int Value
        {
            get
            {
                int nValue = 0;

                // Thread-safe access
                Interlocked.Exchange(ref nValue, m_nValue);

                return nValue;
            }

            set
            {
                Interlocked.Exchange(ref m_nValue, value);
            }
        }

        public static implicit operator int(SafeInt32 oSafeInt32)
        {
            return oSafeInt32.Value;
        }
    }
}
