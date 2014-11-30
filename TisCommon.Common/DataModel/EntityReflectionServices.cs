using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon;
using System.Collections;
using System.Reflection;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.DataModel
{

	#region EntityReflectionServices

	[ComVisible(false)]
	public class EntityReflectionServices
	{
		// A static instance 
		private static EntityReflectionServices m_oStaticInstance = 
			new EntityReflectionServices();

		private static EntityTypeInfoCache m_oEntityTypeInfoCache = new EntityTypeInfoCache();

		//
		//	Public methods
		//

		public EntityReflectionServices()
		{
		}

		#region Implementation of EntityReflectionServices

		public EntityTypeInfo GetEntityTypeInfo(object oObj)
		{
			return GetEntityTypeInfo(oObj.GetType());		
		}

		public EntityTypeInfo GetEntityTypeInfo(Type oType)
		{
			return m_oEntityTypeInfoCache.Get(oType);		
		}

		public EntityParameterInfo GetEntityParameterInfo(
			Type   oType,
			string sParamName)
		{
			EntityTypeInfo oEntityTypeInfo = GetEntityTypeInfo(oType);

			EntityParameterInfo oParamData = 
				oEntityTypeInfo.GetEntityParameterInfo(sParamName);

			if(oParamData == null)
			{
				throw new TisException(
					"Parameter [{0}] not exists in type [{1}]",
					sParamName,
					oType);
			}

			return oParamData;
		}

		public object GetObjectOwner(object oObj)
		{
			EntityTypeInfo oEntityTypeInfo = 
				GetEntityTypeInfo(oObj.GetType());

			return ReflectionUtil.GetFieldValue(oObj, oEntityTypeInfo.OwnerParent);
		}

		public void SetObjectOwner(object oObj, object oParent)
		{
			EntityTypeInfo oEntityTypeInfo = 
				GetEntityTypeInfo(oObj.GetType());

			ReflectionUtil.SetFieldValue(oObj, oEntityTypeInfo.OwnerParent, oParent);
		}

		public object GetRoot (object oObj)
		{
			object oRoot  = null;
			object oOwner = GetObjectOwner (oObj);

			if (oOwner != null)
			{
				oRoot = oOwner;
			}

			if (oRoot != null)
			{
				oRoot = GetRoot (oRoot);
			}
			else
			{
				oRoot = oObj;
			}

			return oRoot; 
		}

		public object GetObjectOwnerOfType(object oObj, Type oType)
		{
			object oParent = GetObjectOwner(oObj);

			while(oParent != null && 
				!oType.IsAssignableFrom(oParent.GetType()))
			{
				oParent = GetObjectOwner(oParent);
			}

			return oParent;
		}

		public ICollection GetParents(object oObj, bool bMandatoryOnly)
		{
			EntityTypeInfo oEntityTypeInfo = 
				GetEntityTypeInfo(oObj.GetType());
			
			ArrayList oParents = new ArrayList();

			foreach(EntityParentInfo oParentInfo in oEntityTypeInfo.AllDirectParents)
			{
				if(bMandatoryOnly && !oParentInfo.Mandatory)
				{
					continue;
				}
				
				object oParent = ReflectionUtil.GetFieldValue(oObj, oParentInfo.Parent);

				if(oParent != null)
				{
					oParents.Add(oParent);
				}
			}

			return oParents;
		}

		public ICollection<object> GetChildren(object oObj, bool bRecursive)
		{
			return GetChildren(
				oObj, 
				null,
				null,
				null,
				null,
				EntityRelation.All,
				bRecursive);
		}

		public ICollection<object> GetChildrenOfType(
			object oObj, 
			Type oType, 
			bool bRecursive)
		{
			return GetChildren(
				oObj, 
				new ChildrenListFilter(ChildrenListFilter_TypeMatch),
				oType,
				null,
				null,
				EntityRelation.All,
				bRecursive);
		}

		public object FindNamedChild(
			object oObj,
			Type oChildType,
			string sChildName,
			bool bRecursive)
		{
			ICollection<object> oFoundObjects = GetChildren(
				oObj,
				new ChildrenListFilter(ChildrenListFilter_TypeMatch),
				oChildType,
				new ObjectFilter(ObjectFilter_NameMatch),
				sChildName,
				EntityRelation.All,
				bRecursive);

			object oRetObj = null;

			// Trick to get the first element if exists 
			// or any element if all are the same 
			foreach (object oFoundObj in oFoundObjects)
			{
				if (oRetObj != null)
				{
					if (!Object.ReferenceEquals(oRetObj, oFoundObj))
					{
						throw new TisException(
							"More than 1 object of type [{0}] with name [{1}] found",
							oChildType,
							sChildName);
					}
				}

				oRetObj = oFoundObj;
			}

			return oRetObj;
		}

		public object CreateChild(
			object oObj,
			Type   oChildType, 
			string sChildName)
		{
			EntityTypeInfo oTypeInfo = GetEntityTypeInfo(oObj);

			EntityChildrenListInfo[] OwnedChildrenInfo = oTypeInfo.OwnedChildren;

			INamedObjectOwnerList oChildList = null;
			
			// Find the ChildList of specified type - must be one
			// and only one
			foreach(EntityChildrenListInfo oChildInfo in OwnedChildrenInfo)
			{
				if(oChildInfo.ChildType == oChildType)
				{
					if(oChildList != null)
					{
						throw new TisException(
							"More than 1 child list contains entities of type [{0}] " +
							"under entity of type [{1}]. " +
							"The child can't be created", oChildType, oObj.GetType());
					}

					oChildList = oChildInfo.GetValue(oObj) as INamedObjectOwnerList;
				}
			}

			if(oChildList == null)
			{
				throw new TisException(
					"The child of type [{0}] can't be created under node of type [{1}]",
					oChildType,
					oObj.GetType());
			}
			
			// Create a child
			return oChildList.CreateNew(sChildName);
		}

		public ICollection<object> GetOwnedChildrenWithObsoleteMember(object oObj, bool bRecursive)
		{
			return GetChildren(
				oObj, 
				null,
				null,
				new ObjectFilter(ObjectFilter_WithObsoleteMember),
				null,
				EntityRelation.Owner,
				bRecursive);
		}

		public FieldInfo[] GetEntityTypeObsoleteMembers(object oObj)
		{
			return GetEntityTypeObsoleteMembers(oObj.GetType());
		}

		public FieldInfo[] GetEntityTypeObsoleteMembers(Type oType)
		{
			EntityTypeInfo oEntityTypeInfo = GetEntityTypeInfo(oType);

			if (oEntityTypeInfo != null)
			{
				return oEntityTypeInfo.ObsoleteMembers;
			}
			else
			{
				return new FieldInfo[0];
			}
		}

		#endregion


		public static EntityReflectionServices GetInstance()
		{
			return m_oStaticInstance;
		}

		//
		//	Private methods
		//

		#region Children Filters

		private delegate bool ChildrenListFilter(
			EntityChildrenListInfo oChildrenListInfo, 
			object oCriteria);

		private static bool ChildrenListFilter_TypeMatch(
			EntityChildrenListInfo oChildrenListInfo, 
			object oCriteria)
		{
			Type oRequestedType = (Type)oCriteria;

			if(oRequestedType.IsAssignableFrom(oChildrenListInfo.ChildType))
			{
				return true;
			}

			return false;
		}

		private static bool ChildrenListFilter_AllPass(
			EntityChildrenListInfo	oChildrenListInfo, 
			object					oCriteria)
		{
			return true;
		}

		private delegate bool ObjectFilter(
			object  oObj, 
			object  oCriteria);

		private static bool ObjectFilter_NameMatch(
			object  oObj, 
			object  oCriteria)
		{
			string sRequiredName = (string)oCriteria;

			INamedObject oNamedObject = oObj as INamedObject;

			if(oNamedObject != null)
			{
				// Get object name
				string sName = oNamedObject.Name; 
					
				// Check if name equals to requested
				if( StringUtil.CompareIgnoreCase(sRequiredName, sName))
				{
					// Match
					return true;
				}
			}

			return false;
		}

		private static bool ObjectFilter_WithObsoleteMember(
			object  oObj, 
			object  oCriteria)
		{
			EntityReflectionServices oThis = new EntityReflectionServices();

			EntityTypeInfo oEntityTypeInfo = oThis.GetEntityTypeInfo(oObj);

			FieldInfo[] ObsoleteMembers = oEntityTypeInfo.ObsoleteMembers;

			return ObsoleteMembers.Length > 0;
		}

		#endregion

		#region Children

		internal EntityChildrenListInfo[] GetChildrenListsInfo(
			EntityTypeInfo		oEntityTypeInfo,
			EntityRelation		enRelation)
		{
			EntityChildrenListInfo[] ChildrenLists = null;
				
			switch(enRelation)
			{
				case EntityRelation.Link: 
					ChildrenLists = oEntityTypeInfo.LinkedChildren;
					break;
				case EntityRelation.Owner: 
					ChildrenLists = oEntityTypeInfo.OwnedChildren;
					break;
				case EntityRelation.All: 
					ChildrenLists = oEntityTypeInfo.AllChildren;
					break;
				default: 
					throw new TisException("Unknown EntityRelation [{0}]", enRelation);
			}

			return ChildrenLists;
		}

		// Avoids reallocation of returned children ArrayList
		private void GetChildren(
			object				oObj, 
			ChildrenListFilter	pfChildrenListFilter,
			object				oChildrenListFilterCriteria,
			ObjectFilter		pfObjectFilter,
			object				oObjectFilterCriteria,
			EntityRelation		enRelation,
			bool				bRecursive,
			ICollection<object>			oOutChildren)
		{
			// Get TypeInfo of specified object				
			EntityTypeInfo oEntityTypeInfo = 
				GetEntityTypeInfo(oObj);
			
			// Get the ChildrenLists according to EntityRelation filter
			EntityChildrenListInfo[] ChildrenLists = GetChildrenListsInfo(
				oEntityTypeInfo,
				enRelation);

			// Iterate ChildrenLists
			foreach(EntityChildrenListInfo oChildrenList in ChildrenLists)
			{
				// Obtain list object
				INamedObjectList oContainer = oChildrenList.GetValue(oObj) as INamedObjectList;

                if (oContainer != null)
                {
                    // Check if container passes filter
                    bool bContainerPassedFilter = (pfChildrenListFilter == null) ||
                        pfChildrenListFilter(oChildrenList, oChildrenListFilterCriteria);

                    object oEl;
                    for (int i = 0; i < oContainer.Count; i++)
                    {
                        oEl = oContainer[oContainer.NameByIndex(i)];

                        if (bContainerPassedFilter &&
                            (pfObjectFilter == null || pfObjectFilter(oEl, oObjectFilterCriteria)))
                        {
                            // If elements filter passed
                            oOutChildren.Add(oEl);
                        }

                        if (bRecursive)
                        {
                            // Recursive call for each children
                            GetChildren(
                                oEl,
                                pfChildrenListFilter,
                                oChildrenListFilterCriteria,
                                pfObjectFilter,
                                oObjectFilterCriteria,
                                enRelation,
                                bRecursive,
                                oOutChildren);
                        }
                    }
                }
			}
		}

		private ICollection<object> GetChildren(
			object				oObj, 
			ChildrenListFilter	pfChildrenListFilter,
			object				oChildrenListFilterCriteria,
			ObjectFilter		pfObjectFilter,
			object				oObjectFilterCriteria,
			EntityRelation		enRelation,
			bool				bRecursive)
		{
			const int INITIAL_CHILDREN_ARRAY = 100;
            
			ICollection<object> oChildren = new List<object>(INITIAL_CHILDREN_ARRAY);

			GetChildren(
				oObj,
				pfChildrenListFilter,
				oChildrenListFilterCriteria,
				pfObjectFilter,
				oObjectFilterCriteria,
				enRelation,
				bRecursive,
				oChildren	// "Out", will be filled
				);

			return oChildren;
		}

		#endregion
		
		#region Static utility methods

		private static string[] GetFieldsNames(FieldInfo[] Fields)
		{
            IList<string> Names = new List<string>();

			foreach(FieldInfo oPropInfo in Fields)
			{
				Names.Add(oPropInfo.Name);
			}

            return Names.ToArray();
		}

        private static void AddIfNotExists(IList<string> oVector, string sVal)
		{
			// Add to if not exists
			if(!oVector.Contains(sVal))
			{
				oVector.Add(sVal);
			}
		}

		private static ICollection GetFieldValues(
			object oObj, 
			FieldInfo[] Fields)
		{
			ArrayList oValues = new ArrayList();

			foreach(FieldInfo oField in Fields)
			{
				oValues.Add(
					oField.GetValue(oObj)
					);
			}

			return oValues;
		}
		
		#endregion

		#region EntityTypeInfoCache

		private class EntityTypeInfoCache
		{
			private const int DEFAULT_CAPACITY = 100;

			private Dictionary<Type, EntityTypeInfo> m_oTypeToInfoTable = 
				new Dictionary<Type, EntityTypeInfo>(DEFAULT_CAPACITY);

			public EntityTypeInfoCache()
			{
			}
			
			public EntityTypeInfo Get(Type oType)
			{
				EntityTypeInfo oVal = null;

				if (!m_oTypeToInfoTable.TryGetValue(oType, out oVal))
				{
					oVal = new EntityTypeInfo(oType);

					m_oTypeToInfoTable[oType] = oVal;
				}

				return oVal;
			}
		
		}
		
		#endregion

		#region ObjectTypeNameMap

		private class ObjectTypeNameMap
		{
			private IDictionary m_oTypeToNameMap   = null;

			public ObjectTypeNameMap()
			{
				Fill();
			}

			public string GetTypeName(Type oType)
			{
				if(oType == null)
				{
					return String.Empty;
				}

				string sName = (string)m_oTypeToNameMap[oType];
				
				if(sName == null)
				{
					return oType.FullName;
				}

				return sName;
			}

			public Type TypeFromName(string sTypeName)
			{
				foreach(DictionaryEntry oEntry in m_oTypeToNameMap)
				{
					if( ((string)oEntry.Value) == sTypeName )
					{
						return (Type)oEntry.Key;
					}
				}

				Type oType = Type.GetType(sTypeName);

				if(oType == null)
				{
					throw new TisException("Type name [{0}] can't be resolved", sTypeName);
				}

				return oType;
			}

			private void Fill()
			{
			}


		}

		#endregion

	}

	#endregion


	#region EntityParameterInfo

	[ComVisible(false)]
	public class EntityParameterInfo
	{
		// Parameters are properties that declared in the object itself or
		// in [AggregatedChild] subobjects.
		public readonly PropertyInfo Parameter;

		public readonly FieldInfo	 DeclaringAggregatedObject;

		public EntityParameterInfo(
			PropertyInfo Parameter,
			FieldInfo	 DeclaringAggregatedObject)
		{
			this.Parameter				   = Parameter;
			this.DeclaringAggregatedObject = DeclaringAggregatedObject;
		}

		public Type ParameterType
		{
			get { return Parameter.PropertyType; }
		}

		public string ParameterName
		{
			get 
			{
				return Parameter.Name;
				//if (DeclaringAggregatedObject == null)
				//{
				//    return Parameter.Name;
				//}
				
				//AggregatedChildAttribute oAttribute = GetAggregatedChildAttribute();

				//string sParamName = String.Format("{0}{1}{2}",
				//    oAttribute.ParamsPrefix,
				//    this.Parameter.Name,
				//    oAttribute.ParamsPostfix);
 
				//return sParamName; 
			}
		}

		public bool IsInAggregatedObject
		{
			get { return DeclaringAggregatedObject != null; }
		}

		public bool IsReadOnly
		{
			get { return (Parameter.CanWrite==false); }
		}

		public object GetDeclaringObject(object oRootObj)
		{
			if(!IsInAggregatedObject)
			{
				return oRootObj;
			}
								
			return ReflectionUtil.GetFieldValue(oRootObj, DeclaringAggregatedObject);
		}

		public object GetValue(object oObj)
		{
			object oDeclaringObj = GetDeclaringObject(oObj);
            if (oDeclaringObj == null) { return null; }

			return ReflectionUtil.GetPropertyValue(oDeclaringObj, Parameter);
		}

		public void SetValue(object oObj, object oVal)
		{
			object oDeclaringObj = GetDeclaringObject(oObj);
			
			object oConvertedVal = oVal;

			if(Parameter.PropertyType.IsEnum)
			{
				// Ugly conversion from Int to enum since
				// Convert.ChangeType don't works
				oConvertedVal = Enum.Parse(Parameter.PropertyType,
					Enum.GetName(Parameter.PropertyType, oVal),
					false);
			}
			else
			{
				if(oVal == null && Parameter.PropertyType == typeof(string))
				{
					oConvertedVal = String.Empty;
				}
				else
				{
					// Try to adjust type
					oConvertedVal = Convert.ChangeType(
						oVal, 
						Parameter.PropertyType);
				}
			}

			ReflectionUtil.SetPropertyValue(oDeclaringObj, Parameter, oConvertedVal);
		}
	
		//
		//	Private
		//

		//private AggregatedChildAttribute GetAggregatedChildAttribute()
		//{
		//    AggregatedChildAttribute oAttribute = null;

		//    if(DeclaringAggregatedObject != null)
		//    {
		//        oAttribute = (AggregatedChildAttribute)ReflectionUtil.GetAttribute(
		//            this.DeclaringAggregatedObject,
		//            typeof(AggregatedChildAttribute));
		//    }

		//    if(oAttribute == null)
		//    {
		//        throw new TisException(
		//            "Fatal error: can't obtain AggregatedChildAttribute " + 
		//            " of aggregated field {0}", 
		//            this.DeclaringAggregatedObject);
		//    }

		//    return oAttribute;
		//}
	}

	#endregion


}
