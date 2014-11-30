using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Customizations
{
    public class TisStationDeclarationInfo
    {
        public TisStationDeclarationInfo()
            : this(String.Empty, String.Empty)
        {
        }

        public TisStationDeclarationInfo(string serviceImplTypeName)
            : this()
        {
            System.Type serviceImplType = StringUtil.IsStringInitialized(serviceImplTypeName) ? System.Type.GetType(serviceImplTypeName) : default(System.Type);

            if (serviceImplType != null)
            {
                TisStationDeclarationAttribute stationDeclarationAttribute =
                    (TisStationDeclarationAttribute)ReflectionUtil.GetAttribute(serviceImplType, typeof(TisStationDeclarationAttribute));

                if (stationDeclarationAttribute != null)
                {
                    Type = stationDeclarationAttribute.Type;
                    SubType = stationDeclarationAttribute.SubType;
                }
            }
        }

        public TisStationDeclarationInfo(TisStationDeclarationAttribute stationDeclarationAttribute)
            : this(stationDeclarationAttribute.Type, stationDeclarationAttribute.SubType)
        {
        }

        public TisStationDeclarationInfo(
            string type, 
            string subType)
        {
            Type = type;
            SubType = subType;
        }

        public string Type { get; private set; }
        public string SubType { get; private set; }
    }
}
