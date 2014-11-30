using System;
using System.Runtime.InteropServices;
using System.Reflection;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public class EntityChildrenListInfo
    {
        public readonly FieldInfo ChildrenList;
        public readonly Type ChildType;
        public readonly bool IsOwner;

        public EntityChildrenListInfo(
            FieldInfo oChildrenListField,
            Type oChildType,
            bool bIsOwner)
        {
            ChildrenList = oChildrenListField;
            ChildType = oChildType;
            IsOwner = bIsOwner;
        }

        public INamedObjectList GetValue(object oObj)
        {
            object oTarget = oObj;

            return (INamedObjectList)ReflectionUtil.GetFieldValue(oTarget, ChildrenList);
        }
    }
}
