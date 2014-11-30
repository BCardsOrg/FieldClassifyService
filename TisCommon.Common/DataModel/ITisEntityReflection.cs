using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.DataModel
{
    [Guid("5A21C59D-71D5-415F-B529-61F7676053CE")]
    public interface ITisEntityReflectionCOM
    {
        void SetTypeNameAlias(string sTypeName, string sAlias);
        void SaveTypeNameAliases();
        ITisEntityTypeInfo GetTypeInfo(Type sEntityType);
        ITisEntityTypeInfo GetTypeInfoByTypeName(string sEntityTypeName);
        object FindNamedChild(Object oObj, String sChildTypeName, String sChildName, Boolean bRecursive);
        object GetObjectOwner(Object oObj);
        object GetObjectOwnerOfType(Object oObj, String sParentTypeName);
        object CreateChild(Object oObj, String sChildTypeName, String sChildName);
    }

    [ComVisible(false)]
    public interface ITisEntityReflection : ITisEntityReflectionCOM
    {
        ICollection<object> GetChildrenOfType(Object oObj, String sChildrenTypeName, Boolean bRecursive);
        string TypeToTypeName(Type sEntityType);
        Type TypeNameToType(string sEntityTypeName);
        IDictionary<string, string> TypeNameAliases { get; }
    }

    [Guid("0BF777F5-8733-40A4-B4FB-CF54E97A0C7A")]
    public interface ITisEntityParameterInfo
    {
        string Name { get; }
        string TypeName { get; }
        TIS_TYPE_ENUM TypeCode { get; }
        Type CLRType { get; }
        bool IsInAggregatedObject { get; }
        object GetValue(Object oObj);
        void SetValue(Object oObj, Object oVal);
        bool IsReadOnly { get; }
    }

    [Guid("A013366E-CF20-4193-98D5-15C096420A4F")]
    public interface ITisEntityTypeInfoCOM
    {
        string TypeName { get; }
        bool IsParameterExists(String sParameterName);
        ITisEntityParameterInfo GetParameterInfo(String sParameterName);
        string OwnerParentTypeName { get; }
    }

    [ComVisible(false)]
    public interface ITisEntityTypeInfo : ITisEntityTypeInfoCOM
    {
        IList<ITisEntityParameterInfo> Parameters { get; }
        IList<string> ChildrenTypeNames { get; }
        IList<ITisEntityChildInfo> LinkedChildren { get; }
        IList<ITisEntityChildInfo> OwnedChildren { get; }
        IList<ITisEntityChildInfo> AllChildren { get; }
    }

    [Guid("FCBDA207-DABD-4841-BB94-4DA25EE88D34")]
    public interface ITisEntityChildInfo
    {


        string TypeName { get; }
        INamedObjectList GetChildList(object oVal);

    }
}
