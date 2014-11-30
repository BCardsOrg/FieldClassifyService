using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Reflection;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.DataModel
{
    #region TisEntityReflection

    [ComVisible(false)]
    public class TisEntityReflection : ITisEntityReflection
    {
        // TypeName -> Alias
        private Dictionary<string, string> m_oTypeNameAliases;

        public TisEntityReflection(string instanceName)
        {
            Load(instanceName);
        }

        #region ITisEntityReflection Members

        public ITisEntityTypeInfo GetTypeInfo(Type oEntityType)
        {
            return new TisEntityTypeInfoExternal(
                Reflection.GetEntityTypeInfo(oEntityType),
                this);
        }

        public ITisEntityTypeInfo GetTypeInfoByTypeName(string sEntityTypeName)
        {
            Type oType = TypeNameToType(sEntityTypeName);

            return new TisEntityTypeInfoExternal(
                Reflection.GetEntityTypeInfo(oType),
                this);
        }

        public Type TypeNameToType(string sEntityTypeName)
        {
            // Can be type name or alias
            string sTypeName = sEntityTypeName;

            string sAliasedType = GetTypeNameByAlias(sTypeName);

            if (sAliasedType != null)
            {
                // Alias was provided, use real type name
                sTypeName = sAliasedType;
            }

            Type oType = null;

            oType = Type.GetType(sTypeName);

            if (oType == null)
            {
                throw new TisException(
                    "Type for TypeName [{0}] can't be resolved",
                    sEntityTypeName);
            }

            return oType;
        }

        public string TypeToTypeName(Type oEntityType)
        {
            string sFullTypeString =
                ReflectionUtil.GetFullTypeString(oEntityType);

            string sShortTypeString =
                ReflectionUtil.GetShortTypeString(oEntityType);

            // Try short type string
            string sAlias = GetAliasByTypeName(sShortTypeString);

            if (sAlias == null)
            {
                // Try full type string
                sAlias = GetAliasByTypeName(sFullTypeString);
            }

            if (sAlias != null)
            {
                return sAlias;
            }

            return sFullTypeString;
        }

        public IDictionary<string, string> TypeNameAliases
        {
            get
            {
                return m_oTypeNameAliases;
            }
        }

        public ICollection<object> GetChildrenOfType(
            object oObj,
            string sChildrenTypeName,
            bool bRecursive)
        {
            return Reflection.GetChildrenOfType(
                oObj,
                TypeNameToType(sChildrenTypeName),
                bRecursive);
        }

        public object FindNamedChild(
            object oObj,
            string sChildTypeName,
            string sChildName,
            bool bRecursive)
        {
            return Reflection.FindNamedChild(
                oObj,
                TypeNameToType(sChildTypeName),
                sChildName,
                bRecursive);
        }

        public object GetObjectOwnerOfType(object oObj, string sParentTypeName)
        {
            return Reflection.GetObjectOwnerOfType(
                oObj,
                TypeNameToType(sParentTypeName));
        }

        public object GetObjectOwner(object oObj)
        {
            return Reflection.GetObjectOwner(oObj);
        }

        public object GetRoot(object oObj)
        {
            return Reflection.GetRoot(oObj);
        }

        public object CreateChild(
            object oObj,
            string sChildTypeName,
            string sChildName)
        {
            return Reflection.CreateChild(
                oObj,
                TypeNameToType(sChildTypeName),
                sChildName);
        }


        public void SetTypeNameAlias(string sTypeName, string sAlias)
        {
            m_oTypeNameAliases[sTypeName] = sAlias;
        }

        public void SaveTypeNameAliases()
        {
        }

        #endregion

        private void Load(string instanceName)
        {
            string resourceFileName = String.Format("{0}.{1}{2}", "TiS.Core.TisCommon.Resources", instanceName, "TypeNameAliases.xml");

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = thisAssembly.GetManifestResourceStream(resourceFileName);

            if (resourceStream != null)
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, string>));

                m_oTypeNameAliases = new Dictionary<string, string>((Dictionary<string, string>)serializer.ReadObject(resourceStream));
            }
            else
            {
                throw new TisException("Failed to load type aliases for {0}.", instanceName);
            }
        }

        private string GetTypeNameByAlias(string sAlias)
        {
            KeyValuePair<string, string> pair = TypeNameAliases.FirstOrDefault((x) => x.Value == sAlias);

            if (pair.Key != null)
            {
                return pair.Key;
            }

            return null;
        }

        private string GetAliasByTypeName(string sTypeName)
        {
            return TypeNameAliases[sTypeName];
        }


        private string TypeToString(Type oType)
        {
            return ReflectionUtil.GetFullTypeString(oType);
        }

        private static EntityReflectionServices Reflection
        {
            get
            {
                return EntityReflectionServices.GetInstance();
            }
        }
    }

    #endregion

    #region EntityTypeInfoExternal

    [ComVisible(false)]
    public class TisEntityTypeInfoExternal : ITisEntityTypeInfo
    {
        private EntityTypeInfo m_oTypeInfo;
        private ITisEntityReflection m_oReflectionExternal;
        private static IDictionary<string, string> m_TypeDictionary = new Dictionary<string, string>();
        private static object m_locker = new object();

        public TisEntityTypeInfoExternal(
            EntityTypeInfo oTypeInfo,
            ITisEntityReflection oReflectionExternal)
        {
            m_oTypeInfo = oTypeInfo;
            m_oReflectionExternal = oReflectionExternal;
        }

        #region ITisEntityTypeInfo Members

        public ITisEntityParameterInfo GetParameterInfo(string sParameterName)
        {
            EntityParameterInfo oParamInfo =
                TypeInfo.GetEntityParameterInfo(sParameterName);

            if (oParamInfo == null)
            {
                return null;
            }

            return new TisEntityParameterInfoExternal(
                oParamInfo,
                m_oReflectionExternal);
        }

        public bool IsParameterExists(string sParameterName)
        {
            return TypeInfo.GetEntityParameterInfo(sParameterName) != null;
        }

        public IList<string> ChildrenTypeNames
        {
            get
            {
                return GetTypeNames(
                    TypeInfo.GetChildrenTypes());
            }
        }

        public string OwnerParentTypeName
        {
            get
            {
                return m_oReflectionExternal.TypeToTypeName(
                    TypeInfo.OwnerParent.FieldType);
            }
        }

        public string TypeName
        {
            get
            {
                // TODO: Lock dictionary?
                string typeName;
                string theTypeName = TypeInfo.TheType.FullName;

                lock (m_locker)
                {
                    if (m_TypeDictionary.TryGetValue(theTypeName, out typeName) == false)
                    {
                        typeName = m_oReflectionExternal.TypeToTypeName(TypeInfo.TheType);
                        m_TypeDictionary.Add(theTypeName, typeName);
                    }
                }

                return typeName;
            }
        }

        public IList<ITisEntityParameterInfo> Parameters
        {
            get
            {
                IList<ITisEntityParameterInfo> oParams =
                    new List<ITisEntityParameterInfo>();

                foreach (EntityParameterInfo oParamInfo in TypeInfo.Parameters)
                {
                    oParams.Add(
                        new TisEntityParameterInfoExternal(oParamInfo, m_oReflectionExternal));
                }

                return oParams;
            }
        }

        public IList<ITisEntityChildInfo> LinkedChildren
        {
            get
            {
                return GetChildInfoArray(m_oTypeInfo.LinkedChildren);
            }
        }

        public IList<ITisEntityChildInfo> OwnedChildren
        {
            get
            {
                return GetChildInfoArray(m_oTypeInfo.OwnedChildren);
            }
        }

        public IList<ITisEntityChildInfo> AllChildren
        {
            get
            {
                return GetChildInfoArray(m_oTypeInfo.AllChildren);
            }
        }

        #endregion

        // 
        // Private
        //

        private IList<ITisEntityChildInfo> GetChildInfoArray(
            EntityChildrenListInfo[] ChildrenListsInfo)
        {
            IList<ITisEntityChildInfo> oChildren = new List<ITisEntityChildInfo>();

            foreach (EntityChildrenListInfo oChildListInfo in ChildrenListsInfo)
            {
                oChildren.Add(
                    new EntityChildInfoExternal(oChildListInfo, m_oReflectionExternal));
            }

            return oChildren;
        }

        private static EntityReflectionServices Reflection
        {
            get
            {
                return EntityReflectionServices.GetInstance();
            }
        }

        private EntityTypeInfo TypeInfo
        {
            get
            {
                return m_oTypeInfo;
            }
        }

        private IList<string> GetTypeNames(ICollection TypeCollection)
        {
            IList<string> oTypes = new List<string>();

            foreach (Type oType in TypeCollection)
            {
                oTypes.Add(
                    m_oReflectionExternal.TypeToTypeName(oType));
            }

            return oTypes;
        }

    }

    #endregion

    #region EntityChildInfoExternal

    [ComVisible(false)]
    public class EntityChildInfoExternal : ITisEntityChildInfo
    {
        private EntityChildrenListInfo m_oChildrenListInfo;
        private ITisEntityReflection m_oReflectionExternal;

        public EntityChildInfoExternal(
            EntityChildrenListInfo oChildrenListInfo,
            ITisEntityReflection oReflectionExternal)
        {
            m_oChildrenListInfo = oChildrenListInfo;
            m_oReflectionExternal = oReflectionExternal;
        }

        #region ITisEntityChildInfo Members

        public INamedObjectList GetChildList(object oVal)
        {
            return m_oChildrenListInfo.GetValue(oVal);
        }

        public string TypeName
        {
            get
            {
                return m_oReflectionExternal.TypeToTypeName(
                    m_oChildrenListInfo.ChildType);
            }
        }

        #endregion

        // 
        // Private
        //

        private static EntityReflectionServices Reflection
        {
            get
            {
                return EntityReflectionServices.GetInstance();
            }
        }

    }

    #endregion

    #region TisEntityParameterInfoExternal

    [ComVisible(false)]
    public class TisEntityParameterInfoExternal : ITisEntityParameterInfo
    {
        private EntityParameterInfo m_oParameterInfo;
        private ITisEntityReflection m_oReflectionExternal;

        public TisEntityParameterInfoExternal(
            EntityParameterInfo oEntityParameterInfo,
            ITisEntityReflection oReflectionExternal)
        {
            m_oParameterInfo = oEntityParameterInfo;
            m_oReflectionExternal = oReflectionExternal;
        }

        #region ITisEntityParameterInfo Members

        public object GetValue(object oObj)
        {
            return m_oParameterInfo.GetValue(oObj);
        }

        public void SetValue(object oObj, object oVal)
        {
            m_oParameterInfo.SetValue(oObj, oVal);
        }

        public string Name
        {
            get
            {
                return m_oParameterInfo.ParameterName;
            }
        }

        public string TypeName
        {
            get
            {
                return m_oReflectionExternal.TypeToTypeName(
                    m_oParameterInfo.ParameterType);
            }
        }

        public TIS_TYPE_ENUM TypeCode
        {
            get
            {
                TIS_TYPE_ENUM enTypeCode = TIS_TYPE_ENUM.TIS_OBJECT;

                try
                {
                    Type oType = m_oParameterInfo.ParameterType;

                    // Assume TIS_TYPE_ENUM maps directly to TypeCode enum
                    enTypeCode = (TIS_TYPE_ENUM)Type.GetTypeCode(oType);
                }
                catch
                {
                    // Shallow
                }

                return enTypeCode;
            }
        }

        public Type CLRType
        {
            get
            {
                return m_oParameterInfo.ParameterType;
            }
        }

        public bool IsInAggregatedObject
        {
            get
            {
                return m_oParameterInfo.IsInAggregatedObject;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return m_oParameterInfo.IsReadOnly;
            }
        }

        #endregion

        // 
        // Private
        //

        private static EntityReflectionServices Reflection
        {
            get
            {
                return EntityReflectionServices.GetInstance();
            }
        }

    }

    #endregion
}
