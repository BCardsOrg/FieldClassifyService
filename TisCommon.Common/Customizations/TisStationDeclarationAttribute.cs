using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Customizations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [ComVisible(false)]
    public class TisStationDeclarationAttribute : Attribute
    {
        public TisStationDeclarationAttribute() : this(String.Empty)
        {
        }

        public TisStationDeclarationAttribute(
            string type, 
            string subType = "")
        {
            Type = type;
            SubType = subType;
        }

        public string Type { get; set; }
        public string SubType { get; set; }
    }
}
