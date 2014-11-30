using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon
{
    public class TisCustomizationException : TisException
    {
         /// <summary>
         /// Exception that is thrown by integra events.
         /// </summary>
         /// <param name="originalException"></param>
         /// <param name="strFormat"></param>
         /// <param name="args"></param>
        public TisCustomizationException(Exception originalException, string strFormat, params object[] args)
            : base(originalException, strFormat, args)
        {

        }
    }
}
