using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public class EntityTypeInfo
    {
        public readonly Type TheType;
        public readonly FieldInfo OwnerParent;
        public readonly EntityParentInfo[] AllDirectParents;
        public readonly FieldInfo[] AggregatedChildren;
        public readonly EntityChildrenListInfo[] OwnedChildren;
        public readonly EntityChildrenListInfo[] LinkedChildren;
        public readonly EntityChildrenListInfo[] AllChildren;
        public readonly EntityParameterInfo[] Parameters;
        public readonly FieldInfo[] UniqueParentLinks;
        public readonly FieldInfo[] ObsoleteMembers;

        public EntityTypeInfo(Type oType)
        {
            TheType = oType;

            OwnerParent = GetOwnerParent(TheType);

            AllDirectParents = GetDirectParents(TheType);
            AggregatedChildren = GetAggregatedChildren(TheType);
            OwnedChildren = GetOwnedChildrenLists(TheType);
            LinkedChildren = GetLinkedChildrenLists(TheType);

            AllChildren = (EntityChildrenListInfo[])ArrayBuilder.ChangeArrayType(
                SetUtil<EntityChildrenListInfo>.GetUnion(OwnedChildren, LinkedChildren),
                typeof(EntityChildrenListInfo));

            Parameters = GetParameters(TheType, AggregatedChildren);

            UniqueParentLinks = GetUniqueParentLinks(TheType);

            ObsoleteMembers = GetObsoleteMembers(TheType);
        }

        public EntityParameterInfo GetEntityParameterInfo(string sParamName)
        {
            foreach (EntityParameterInfo oParamData in Parameters)
            {
                if (oParamData.Parameter.Name == sParamName)
                {
                    return oParamData;
                }
            }

            return null;
        }

        public FieldInfo GetUniqueParentLinkField(Type oType)
        {
            foreach (FieldInfo oLinkField in UniqueParentLinks)
            {
                if (oLinkField.FieldType.IsAssignableFrom(oType))
                {
                    return oLinkField;
                }
            }

            return null;
        }
        public Type[] GetChildrenTypes()
        {
            ArrayBuilder oTypes = new ArrayBuilder();

            foreach (EntityChildrenListInfo oData in AllChildren)
            {
                oTypes.AddIfNotExists(oData.ChildType);
            }

            return (Type[])oTypes.GetArray(typeof(Type));
        }

        public bool IsOwnerParentNotSerialized
        {
            get
            {
                return (OwnerParent.Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized;
            }
        }

        //
        //	Private
        //

        private static Type[] GetTypes(ICollection FieldInfoCollection)
        {
            ArrayBuilder oTypes = new ArrayBuilder();

            foreach (FieldInfo oInfo in FieldInfoCollection)
            {
                oTypes.AddIfNotExists(oInfo.FieldType);
            }

            return (Type[])oTypes.GetArray(typeof(Type));
        }

        #region Special Fields Accessors

        private static EntityParentInfo[] GetDirectParents(Type oType)
        {
            // Get Fields with ParentAttribute
            FieldInfo[] Fields = ReflectionUtil.GetFieldsWithAttribute(
                oType,
                typeof(ParentAttribute));

            // Prepare array
            EntityParentInfo[] ParentInfoArray = new EntityParentInfo[Fields.Length];

            for (int i = 0; i < Fields.Length; i++)
            {
                FieldInfo oField = Fields[i];

                bool bMandatory = false;

                // Check Mandatory flag
                ParentAttribute oParentAttribute = (ParentAttribute)ReflectionUtil.GetAttribute(
                    oField,
                    typeof(ParentAttribute));

                if (oParentAttribute != null)
                {
                    bMandatory = oParentAttribute.Mandatory;
                }

                ParentInfoArray[i] = new EntityParentInfo(oField, bMandatory);
            }

            return ParentInfoArray;
        }

        private static FieldInfo GetOwnerParent(Type oType)
        {
            // Get Fields with OwnerParentAttribute
            FieldInfo[] Fields = ReflectionUtil.GetFieldsWithAttribute(
                oType,
                typeof(OwnerParentAttribute));

            if (Fields.Length == 0)
            {
                // No parent
                return null;
            }

            // Validate we have only one parent
            if (Fields.Length > 1)
            {
                throw new TisException(
                    "Type [{0}] has more than 1 Fields marked as OwnerParent",
                    oType);
            }

            // Get parent Field
            return Fields[0];
        }

        private static FieldInfo[] GetAggregatedChildren(Type oType)
        {
            return ReflectionUtil.GetFields(
                oType,
                new MemberFilter(FieldFilter_AggregatedChild),
                null);
        }


        private static EntityChildrenListInfo[] GetChildrenListsData(
            FieldInfo[] Fields,
            bool bIsOwner)
        {
            int nListsCount = Fields.Length;

            EntityChildrenListInfo[] ChildrenLists = new EntityChildrenListInfo[nListsCount];

            for (int i = 0; i < nListsCount; i++)
            {
                FieldInfo oField = Fields[i];

                // Obtain ChildrenListAttribute
                ChildrenListAttribute oAttribute = (ChildrenListAttribute)ReflectionUtil.GetAttribute(
                    oField,
                    typeof(ChildrenListAttribute));

                EntityChildrenListInfo oListData = new EntityChildrenListInfo(
                    oField,
                    oAttribute.ElementType,
                    bIsOwner);

                // Put to array
                ChildrenLists[i] = oListData;
            }

            return ChildrenLists;
        }

        private static EntityChildrenListInfo[] GetLinkedChildrenLists(Type oType)
        {
            return GetChildrenListsData(
                GetLinkedChildrenListsFields(oType),
                false // IsOwner
                );

        }

        private static EntityChildrenListInfo[] GetOwnedChildrenLists(Type oType)
        {
            return GetChildrenListsData(
                GetOwnedChildrenListsFields(oType),
                true // IsOwner
                );

        }

        private static FieldInfo[] GetOwnedChildrenListsFields(Type oType)
        {
            return ReflectionUtil.GetFields(
                oType,
                new MemberFilter(FieldFilter_OwnedChildrenList),
                null);
        }

        private static FieldInfo[] GetLinkedChildrenListsFields(Type oType)
        {
            return ReflectionUtil.GetFields(
                oType,
                new MemberFilter(FieldFilter_LinkedChildrenList),
                null);
        }

        private static EntityParameterInfo[] GetParameters(
            Type oType,
            FieldInfo[] AggregatedChildren)
        {
            ArrayBuilder oParamData = new ArrayBuilder();

            // Array of all parameter names - to detect collisions
            IList<string> oParamNames = new List<string>();

            //
            //	Add parameters that belong directly to the provided Type
            //

            PropertyInfo[] ParamProperties = GetParameterProperties(oType);

            foreach (PropertyInfo oParamProperty in ParamProperties)
            {
                EntityParameterInfo oParamInfo = new EntityParameterInfo(
                    oParamProperty,
                    null // DeclaringAggregatedObject
                    );

                oParamData.Add(oParamInfo);

                oParamNames.Add(oParamInfo.ParameterName);
            }

            //
            // Add aggregated object parameters
            //

            foreach (FieldInfo oAggregatedChild in AggregatedChildren)
            {
                PropertyInfo[] ChildParamProperties =
                    GetParameterProperties(oAggregatedChild.FieldType);

                foreach (PropertyInfo oParamProperty in ChildParamProperties)
                {
                    EntityParameterInfo oChildParamInfo = new EntityParameterInfo(
                            oParamProperty,
                            oAggregatedChild // DeclaringAggregatedObject
                            );

                    string sParamName = oChildParamInfo.ParameterName;

                    if (!oParamNames.Contains(sParamName))
                    {
                        // Add only if no parameter with same name already added

                        oParamData.Add(oChildParamInfo);

                        oParamNames.Add(sParamName);
                    }
                }
            }

            return (EntityParameterInfo[])oParamData.GetArray(
                typeof(EntityParameterInfo));
        }

        private static PropertyInfo[] GetParameterProperties(Type oType)
        {
            return ReflectionUtil.GetPublicProperties(
                oType,
                new MemberFilter(PropertyFilter_Parameter),
                null);
        }


        private static FieldInfo[] GetUniqueParentLinks(Type oType)
        {
            return ReflectionUtil.GetFieldsWithAttribute(
                oType,
                typeof(UniqueParentLinkAttribute));
        }

        private static FieldInfo[] GetObsoleteMembers(Type oType)
        {
            return ReflectionUtil.GetFieldsWithAttribute(
                oType,
                typeof(ObsoleteAttribute));
        }

        #endregion

        #region Field/Property Filters

        private static bool FieldFilter_DirectParent(
            MemberInfo oMember,
            object oNull)
        {
            FieldInfo oField = (FieldInfo)oMember;
            Type oFieldType = oField.FieldType;

            // Is compound type
            if (IsCompoundType(oFieldType))
            {
                // All Fields that marked with ParentAttribute pass
                if (ReflectionUtil.HasAttribute(oField, typeof(ParentAttribute)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool FieldFilter_AggregatedChild(
            MemberInfo oMember,
            object oNull)
        {
            if (ReflectionUtil.HasAttribute(
                oMember,
                typeof(AggregatedChildAttribute)))
            {
                return true;
            }

            return false;
        }

        private static bool FieldFilter_OwnedChildrenList(
            MemberInfo oMember,
            object oNull)
        {
            FieldInfo oField = (FieldInfo)oMember;

            return FieldFilter_AttributeAndType(
                oField,
                typeof(OwnedChildrenListAttribute),
                typeof(INamedObjectOwnerList));
        }

        private static bool FieldFilter_LinkedChildrenList(
            MemberInfo oMember,
            object oNull)
        {
            FieldInfo oField = (FieldInfo)oMember;

            return FieldFilter_AttributeAndType(
                oField,
                typeof(LinkedChildrenListAttribute),
                typeof(INamedObjectList));
        }

        private static bool PropertyFilter_Parameter(
            MemberInfo oMember,
            object oNull)
        {
            PropertyInfo oProperty = (PropertyInfo)oMember;
            Type oPropertyType = oProperty.PropertyType;

            // No indexed properties
            if (oProperty.GetIndexParameters().Length > 0)
            {
                return false;
            }

            ParameterAttribute oParamAttribute = (ParameterAttribute)ReflectionUtil.GetAttribute(
                oProperty,
                typeof(ParameterAttribute));

            if ((oParamAttribute != null) && !oParamAttribute.IsParameter)
            {
                return false;
            }

            bool bExplicitParameter =
                (oParamAttribute != null) && oParamAttribute.IsParameter;

            if (IsCompoundType(oPropertyType) && !bExplicitParameter)
            {
                return false;
            }

            return true;
        }

        private static bool PropertyFilter_SpecificParameter(
            MemberInfo oMember,
            object oParamName)
        {
            PropertyInfo oProperty = (PropertyInfo)oMember;

            if (PropertyFilter_Parameter(oMember, null))
            {
                if ((string)oParamName == oProperty.Name)
                {
                    return true;
                }
            }

            return false;
        }

        // Non-standard, called by other filters
        private static bool FieldFilter_AttributeAndType(
            FieldInfo oField,
            Type oAttributeBaseType,
            Type oFieldBaseType)
        {
            // Check all Fields that marked with ChildAttribute
            if (ReflectionUtil.HasAttribute(oField, oAttributeBaseType))
            {
                // Must implement INamedObjectList
                if (oFieldBaseType.IsAssignableFrom(oField.FieldType))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        private static bool IsCompoundType(Type oType)
        {
            bool bRetVal = false;

            // If class (but not string) or interface
            if ((oType.IsClass && !(oType == typeof(string))) || oType.IsInterface)
            {
                bRetVal = true;
            }

            return bRetVal;
        }

    }
}
