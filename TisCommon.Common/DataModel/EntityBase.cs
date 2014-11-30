using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using TiS.Core.TisCommon.Reflection;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.DataModel
{
    [Serializable]
    [ComVisible(false)]
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow", IsReference = true)]
    [DebuggerDisplay("Name=[{m_sName}]")]
    public class EntityBase : IEntityBase, IEntityPostSerializationActions
    {

        private const string ENITY_LEGACY_ELEGIBLE_NAME_SEPARATOR = "_";
        protected const string ENITY_NAME_SEPARATOR = ".";
        [DataMember]
        private string m_sName;

        // Currently static to simplify the mechanism
        // TODO: make a context service. This will allow to define
        // default values more dynamically (SetFieldDefaultValue, etc)
        //private static ObjectDataSetterGetter m_oDefaultValuesSetter
        //    = new ObjectDataSetterGetter(null, null);

        [NonSerialized]
        [IgnoreDataMember]
        private TisServicesAccessorBase m_servicesAccessor;
        //        private EntityTreeContext m_oContext;

        // Restored after deserialization - 
        // better solution from data integrity point of view
        // Use system array instead of ArrayList in order to save memory
        [NonSerialized]
        [IgnoreDataMember]
		private List<INamedObjectList> m_ParentContainers = new List<INamedObjectList>();

        // Note: One event is used in order to save memory
        // Each declared event (even without subscribers) = at least 4 bytes.
        // EntityBase should be as 'light' as possible
        [field: NonSerialized]
        [field: IgnoreDataMember]
        public event TreeChangeDelegate OnTreeChange;

        // Cached for faster access. Used very often.
        [NonSerialized]
        [IgnoreDataMember]
        private EntityTypeInfo m_oTypeInfo;

        //
        //	Public
        //

        public EntityBase()
            : this(String.Empty, null)
        {
        }

        public EntityBase(string sName)
            : this(sName, null)
        {
        }

		public EntityBase(
            string sName,
            TisServicesAccessorBase servicesAccessor)
        //            EntityTreeContext oContext)
        {
            m_sName = sName;
            m_servicesAccessor = servicesAccessor;
            //            m_oContext = oContext;

            //if (!SmartSerializer.DeserializeInProcess)
            {
                NewEntityPreActivateTasks();
            }
        }


        [OnDeserialized()]
        public void Deserialized(StreamingContext context)
        {

        }

        #region Context

        public virtual void SetServicesAccessor(TisServicesAccessorBase servicesAccessor)
        {
            m_servicesAccessor = servicesAccessor;

            // "Push" context to clients - to optimize access to context
			IEnumerable<object> oOwnedChildren = GetOwnedChildren(false);

            foreach (object oChild in oOwnedChildren)
            {
                IEntityBase oChildEntity = oChild as IEntityBase;

                if (oChildEntity != null)
                {
                    oChildEntity.SetServicesAccessor(servicesAccessor);
                }
            }
        }

        //public virtual void SetContext(EntityTreeContext oContext)
        //{
        //    m_oContext = oContext;

        //    // "Push" context to clients - to optimize access to context
        //    ICollection<object> oOwnedChildren = GetOwnedChildren(false);

        //    foreach (object oChild in oOwnedChildren)
        //    {
        //        IEntityBase oChildEntity = oChild as IEntityBase;

        //        if (oChildEntity != null)
        //        {
        //            oChildEntity.SetContext(oContext);
        //        }
        //    }
        //}
        #endregion

        #region IDeserializationCallback Members

        public virtual void OnDeserialization(object sender)
        {
            DeserializedEntityPreActivateTasks();
        }

        #endregion

        #region Name/FullContextName/Rename

        public string Name
        {
            get { return m_sName; }
        }

        public string FullContextName
        {
            get
            {
                string sFullContextName = CreateFullContextName(
                    GetOwnersChain(this),
                    this.Name);

                return sFullContextName;
            }
        }

        public virtual void Rename(
            string sNewName)
        {
            // Optimization
            if (this.Name == sNewName)
            {
                return;
            }

            using (AutoTreeChangeNotify oChangeNotify = new AutoTreeChangeNotify(
                      this,
                      EntityTreeChange.Rename,
                      this,
                      null,
                      EntityRelation.Undefined,
                      sNewName))
            {
                Hashtable oNamesMap = new Hashtable();

                foreach (INamedObjectList oParentContainer in ParentContainers)
                {
                    string sPrevName = oParentContainer[this.Name].Name;

                    oNamesMap.Add(oParentContainer, sPrevName);
                }

                PreFullContextNameChangeNotify(sNewName);

                // Set new name
                m_sName = sNewName;

                // Notify all containers that object name was changed
                foreach (INamedObjectList oParentContainer in ParentContainers)
                {
                    // Keep previous name
                    string sPrevName = (string)(oNamesMap[oParentContainer]);
                    string sName = oParentContainer[sPrevName].Name;

                    oParentContainer.ObjectRenamed(sPrevName, sName);
                }

            }
        }

        #endregion

        #region IValidable

        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Reflection

        public EntityTypeInfo TypeInfo
        {
            get
            {
                if (m_oTypeInfo == null)
                {
                    m_oTypeInfo = Reflection.GetEntityTypeInfo(this);
                }

                return m_oTypeInfo;
            }
        }

        public IEntityBase OwnerParentEntity
        {
            get
            {
                return (OwnerParent as IEntityBase);
            }
        }

        public INamedObjectList OwnerContainter
        {
            get
            {
                if (ParentContainers.Length == 0)
                {
                    return null;
                }

                // Owner is always first
                INamedObjectList oOwnerContainer = ParentContainers[0];

                return oOwnerContainer;
            }
        }

        public IEntityBase RootOwner
        {
            get
            {
                IEntityBase oRoot = this;

                IEntityBase oOwner;

                while (true)
                {
                    oOwner = oRoot.OwnerParentEntity;

                    if (oOwner == null)
                    {
                        break;
                    }

                    oRoot = oOwner;
                }

                return oRoot;
            }
        }

        public IEntityBase[] OwnersChain
        {
            get
            {
                ArrayBuilder oOwners = new ArrayBuilder(typeof(IEntityBase));

                IEntityBase oOwner = this.OwnerParentEntity;

                while (oOwner != null)
                {
                    oOwners.Add(oOwner);

                    oOwner = oOwner.OwnerParentEntity;
                }

                IEntityBase[] Owners = (IEntityBase[])oOwners.GetArray();

                Array.Reverse(Owners);

                return Owners;
            }
        }

        public object GetObjectOwnerOfType(Type oType)
        {
            return Reflection.GetObjectOwnerOfType(this, oType);
        }

        public ICollection GetParents(bool bMandatoryOnly)
        {
            return Reflection.GetParents(this, bMandatoryOnly);
        }

        public IEnumerable<object> GetOwnedChildren(bool bRecursive)
        {
			//return Reflection.GetOwnedChildren(this, bRecursive);

			var iepsa = this as IEntityPostSerializationActions;

			foreach (var itemsList in iepsa.GetOwnedChildrenLists())
			{
                if (itemsList != null)
                {
                    foreach (var item in itemsList)
                    {
                        yield return item;

                        if (bRecursive)
                        {
                            IEntityBase entityBase = item as IEntityBase;
                            if (entityBase != null)
                            {
                                foreach (var childItem in entityBase.GetOwnedChildren(bRecursive))
                                {
                                    yield return childItem;
                                }
                            }
                        }
                    }
                }
			}
        }

		public IEnumerable<object> GetLinkedChildren(bool bRecursive)
        {
            //return Reflection.GetLinkedChildren(this, bRecursive);
		
			var iepsa = this as IEntityPostSerializationActions;

			foreach (var itemsList in iepsa.GetLinkedChildrenLists())
			{
				foreach (var item in itemsList)
				{
					yield return item;

					if (bRecursive)
					{
						IEntityBase entityBase = item as IEntityBase;
						if (entityBase != null)
						{
							foreach (var childItem in entityBase.GetLinkedChildren(bRecursive))
							{
								yield return childItem;
							}
						}
					}
				}
			}
		}

		public IEnumerable<object> GetAllChildren(bool bRecursive)
		{
			foreach (var item in GetOwnedChildren(bRecursive))
			{
				yield return item;
			}
			foreach (var item in GetLinkedChildren(bRecursive))
			{
				yield return item;
			}
		}

        public ICollection<object> GetChildrenOfType(
            Type oType,
            bool bRecursive)
        {
            return Reflection.GetChildrenOfType(this, oType, bRecursive);
        }

        public object FindNamedChild(
            Type oChildType,
            string sChildName,
            bool bRecursive)
        {
            return Reflection.FindNamedChild(
                this,
                oChildType,
                sChildName,
                bRecursive);
        }

        public void SetOwnerParent(IEntityBase oParentEntity)
        {
			Reflection.SetObjectOwner(this, oParentEntity);
        }

        public bool IsOwnerParent(IEntityBase oEntity)
        {
			IEntityBase oOwner = this.OwnerParentEntity;

			while (oOwner != null)
			{
				if (oOwner == oEntity)
				{
					return true;
				}

				oOwner = oOwner.OwnerParentEntity;
			}

			return false;
        }

        #endregion


        #region Services access

        public object GetContextService(string sServiceName)
        {
            object oService = CheckedServicesAccessor.GetService(sServiceName);

            return oService;
        }

        public object GetContextService(Type oServiceType)
        {
            object oService = CheckedServicesAccessor.GetService(oServiceType);

            return oService;
        }

        public object GetContextService(string sServiceName, string sInstanceName)
        {
            object oService = CheckedServicesAccessor.GetService(sServiceName, sInstanceName);

            return oService;
        }

        public object GetContextService(Type oServiceType, string sInstanceName)
        {
            object oService = CheckedServicesAccessor.GetService(oServiceType, sInstanceName);

            return oService;
        }

        public object GetContextService(ITisServiceInfo oServiceInfo)
        {
            object oService = CheckedServicesAccessor.GetService(oServiceInfo);

            return oService;
        }

        #endregion

        #region INamedObjectFactory Members

        public virtual INamedObject CreateObject(Type oType, string sName)
        {
            // Create object instance
            INamedObject oObj = (INamedObject)Activator.CreateInstance(
                oType,
                new object[] { sName });

            return oObj;
        }

        #endregion

        //
        //	Protected / Private
        //

        #region FullContextName services

        internal static string GetLegacyElegibleFullContextName(IEntityBase entity)
        {
            return entity.FullContextName.Replace(ENITY_NAME_SEPARATOR, ENITY_LEGACY_ELEGIBLE_NAME_SEPARATOR);
        }

        // Used to create simulated owners chain when this entity has a specified
        // name that can be different from the real name.
        // Used for rename events
        private string GetChildFullContextName(
            IList<string> ownersChain,
            string entityName,
            IEntityBase childEntity)
        {
            IList<string> childEntityOwners = GetOwnersChain(childEntity);

            if (childEntityOwners.Count > 0)
            {
                childEntityOwners[ownersChain.Count] = entityName;
            }

            string childFullContextName = CreateFullContextName(childEntityOwners, childEntity.Name);
            return childFullContextName;
        }

        // Non-recursive method
        private static string CreateFullContextName(
            IList<string> ownersChain,
            string sEntityName)
        {
            // No context name if no parent - backward compatibility
            if (ownersChain.Count == 0)
            {
                return String.Empty;
            }

            StringBuilder oFullContextName = new StringBuilder();

            for (int i = 1; i < ownersChain.Count; i++)
            {
                string sOwner = ownersChain[i];

                oFullContextName.Append(sOwner);
                oFullContextName.Append(ENITY_NAME_SEPARATOR);
            }

            oFullContextName.Append(sEntityName);

            return oFullContextName.ToString();
        }

        private static IList<string> GetOwnersChain(IEntityBase oEntity)
        {
            IList<string> result = new List<string>();

            IEntityBase oOwner = null;

            oOwner = oEntity.OwnerParentEntity;

            while (oOwner != null)
            {
                result.Insert(0, oOwner.Name);
                oOwner = oOwner.OwnerParentEntity;
            }

            return result;
        }

        #endregion

        #region PreFullContextNameChange

        private void PreFullContextNameChangeNotify(string sNewName)
        {
            // Create new FullContextName
            IList<string> ownersChain = GetOwnersChain(this);
            string sNewFullContextName = CreateFullContextName(ownersChain, sNewName);

            //	Notify this instance that FullContextName is goind to be changed
            OnPreFullContextNameChange(sNewFullContextName);

			foreach (object oChild in GetOwnedChildren(true))
            {
                IEntityBase oChildEntity = oChild as IEntityBase;

                if (oChildEntity == null)
                {
                    continue;
                }

                // Create child FullContextName
                string sNewChildFullContextName = GetChildFullContextName(
                    ownersChain,
                    sNewName,
                    oChildEntity);

                //Notify child that FullContextName is goind to be changed
                oChildEntity.OnPreFullContextNameChange(
                    sNewChildFullContextName);
            }

        }

        public virtual void OnPreFullContextNameChange(
            string sNewFullContextName)
        {
        }

        #endregion

        #region Context

        public TisServicesAccessorBase ServicesAccessor
        {
            get
            {
                TisServicesAccessorBase servicesAccessor = m_servicesAccessor;

                if (servicesAccessor == null)
                {
                    IEntityBase oOwnerParent = OwnerParentEntity;

                    if (oOwnerParent != null)
                    {
                        servicesAccessor = oOwnerParent.ServicesAccessor;
                    }
                }

                return servicesAccessor;
            }
        }

        public TisServicesAccessorBase CheckedServicesAccessor
        {
            get
            {
                TisServicesAccessorBase servicesAccessor = ServicesAccessor;

                if (servicesAccessor == null)
                {
					throw new TisException("No services accessor available, entity : Type: [{0}] Value: [{1}]", this.GetType(), this);
                }

                return servicesAccessor;
            }

        }

        protected bool HasServicesAccessor
        {
            get
            {
                return ServicesAccessor != null;
            }
        }

        //public EntityTreeContext Context
        //{
        //    get
        //    {
        //        EntityTreeContext oContext = m_oContext;

        //        if (oContext == null)
        //        {
        //            IEntityBase oOwnerParent = OwnerParentEntity;

        //            if (oOwnerParent != null)
        //            {
        //                oContext = oOwnerParent.Context;
        //            }
        //        }

        //        return oContext;
        //    }
        //}

        //protected EntityTreeContext CheckedServicesAccessor
        //{
        //    get
        //    {
        //        EntityTreeContext oContext = Context;

        //        if (oContext == null)
        //        {
        //            throw new TisException("No context available, entity: [{0}]", this);
        //        }

        //        return oContext;
        //    }

        //}

        //protected bool HasContext
        //{
        //    get
        //    {
        //        return Context != null;
        //    }
        //}

        #endregion

        #region DefaultValues

        //private ObjectDataSetterGetter DefaultValuesSetter
        //{
        //    get
        //    {
        //        return m_oDefaultValuesSetter;
        //    }
        //}

        private void InitEntityFieldsToDefaults(IEntityBase oEntity)
        {
            //object entityTarget = oEntity;

            //// Init object with default values
            //DefaultValuesSetter.InitSerializedFieldsToDefaults(
            //    entityTarget,
            //    true // bExplicitlyDeclaredOnly
            //    );

            //ICollection oAggregatedChildren =
            //    Reflection.GetAggregatedChildren(entityTarget);

            //foreach (object oChildObj in oAggregatedChildren)
            //{
            //    object childTarget = oChildObj;

            //    // Init object with default values
            //    DefaultValuesSetter.InitSerializedFieldsToDefaults(
            //        childTarget,
            //        true // bExplicitlyDeclaredOnly
            //        );
            //}
        }

        #endregion

        #region Validation

        protected bool OwnedChildrenValid
        {
            get
            {
                // Get all directly owned children
				IEnumerable<object> oAllOwnedChildren = GetOwnedChildren(
                    false	// bRecursive
                    );

                // For each child
                foreach (object oChild in oAllOwnedChildren)
                {
                    // Query for IValidable interface
                    IValidable oValidableChild = oChild as IValidable;

                    // If supports
                    if (oValidableChild != null)
                    {
                        // Check if valid
                        if (!oValidableChild.IsValid)
                        {
                            // Not valid - return false
                            return false;
                        }
                    }
                }

                // All valid
                return true;
            }
        }

        #endregion

        #region Activation

        private void NewEntityPreActivateTasks()
        {
            InitEntityFieldsToDefaults(this);

            CommonPreActivateTasks();
        }

        private void DeserializedEntityPreActivateTasks()
        {
            CommonPreActivateTasks();

            RestoreChildParentContainerArrays();
        }

        private void CommonPreActivateTasks()
        {
            SubcribeContainersEvents();

            var iepsa = this as IEntityPostSerializationActions;
            // Set 'this' as factory of owned child object lists in order
            // to intercept object creation event and perform specific
            // tasks like initialization of fields to default values
            foreach (var ownedList in iepsa.GetOwnedChildrenLists())
            {
                if (ownedList != null)
                {
                    IUsesNamedObjectFactory oList = ownedList as IUsesNamedObjectFactory;

                    if (oList != null)
                    {
                        oList.SetFactory(this);
                    }


                    foreach (var ownedChild in ownedList)
                    {
                        IEntityPostSerializationActions ownedChildAction = ownedChild as IEntityPostSerializationActions;

                        if (ownedChildAction != null)
                            ownedChildAction.SetParent(this);
                    }
                }
            }

			foreach (var aggChild in iepsa.GetAggregatedChildren())
			{
                if (aggChild != null)
                {
                    IEntityPostSerializationActions ownedChildAction = aggChild as IEntityPostSerializationActions;

                    if (ownedChildAction != null)
                        ownedChildAction.SetParent(this);
                }
			}
			//// Set 'this' as OwnerParent of all aggregated children
			//foreach (FieldInfo oChildField in this.TypeInfo.AggregatedChildren)
			//{
			//    object oAggregatedChild =
			//        ReflectionUtil.GetFieldValue(this, oChildField);
			//    //                    oChildField.GetValue(this);

			//    if (oAggregatedChild != null)
			//    {
			//        this.SetObjectOwnerParent(oAggregatedChild, this);
			//    }
			//}

        }

        #endregion

        #region Reflection

        protected EntityReflectionServices Reflection
        {
            get
            {
                return EntityReflectionServices.GetInstance();
            }
        }

        protected object OwnerParent
        {
            get
            {
				return GetParent(); // ReflectionUtil.GetFieldValue(this, TypeInfo.OwnerParent);
            }
        }

        #endregion

        #region Logging utils

        private void LogContextServiceRequest(object oServiceKey, object oRetService)
        {
            Log.WriteDetailedDebug(
                "ContextService [{0}] requested, [{1}] returned",
                oServiceKey,
                oRetService);
        }

        #endregion

        #region PreTreeChange

        public EntityTreeChangeInfo PreTreeChange(
            EntityTreeChange enChange,
            IEntityBase oParentEntity,
            IEntityBase oChildEntity,
            EntityRelation enParentChildRelation,
            object oAdditionalInfo)
        {
            EntityTreeChangeInfo oChangeInfo = new EntityTreeChangeInfo(
                    enChange,
                    oParentEntity,
                    oChildEntity,
                    enParentChildRelation,
                    oAdditionalInfo);

            PreTreeChange(oChangeInfo);

            return oChangeInfo;
        }

        public EntityTreeChangeInfo PostTreeChange(
            EntityTreeChange enChange,
            IEntityBase oParentEntity,
            IEntityBase oChildEntity,
            EntityRelation enParentChildRelation,
            object oAdditionalInfo)
        {
            EntityTreeChangeInfo oChangeInfo = new EntityTreeChangeInfo(
                enChange,
                oParentEntity,
                oChildEntity,
                enParentChildRelation,
                oAdditionalInfo);

            PostTreeChange(oChangeInfo);

            return oChangeInfo;
        }

        private void PreTreeChange(EntityTreeChangeInfo oChangeInfo)
        {
            TreeChangeNotify(
                EntityTreeEventTiming.PreChange,
                oChangeInfo
                );
        }

        public void PostTreeChange(EntityTreeChangeInfo oChangeInfo)
        {
            TreeChangeNotify(
                EntityTreeEventTiming.PostChange,
                oChangeInfo
                );
        }

        #endregion

        #region TreeChange events

        public virtual void TreeChangeNotify(
            EntityTreeEventTiming enTiming,
            EntityTreeChangeInfo oChangeInfo)
        {
            // Fire event 
            FireTreeChangeEvent(
                enTiming,
                oChangeInfo);

            // Call 'Template method' event
            OnTreeChangeHandler(
                enTiming,
                oChangeInfo);

            IEntityBase oParent = OwnerParent as IEntityBase;

            if (oParent != null)
            {
                // Notify parent 
                oParent.TreeChangeNotify(
                    enTiming,
                    oChangeInfo);
            }

        }

        private void FireTreeChangeEvent(
            EntityTreeEventTiming enTiming,
            EntityTreeChangeInfo oChangeInfo)
        {
            if (this.OnTreeChange != null)
            {
                OnTreeChange(
                    new TreeChangeEventArgs(enTiming, oChangeInfo)
                    );
            }
        }

        // May be overriden in inherited entities
        protected virtual void OnTreeChangeHandler(
            EntityTreeEventTiming enTiming,
            EntityTreeChangeInfo oChangeInfo)
        {
        }

        #endregion

        #region UniqueParentLinks handling

        private void SetUniqueParentLinkIfNeeded(object oVal)
        {
            FieldInfo oField = GetUniqueParentLinkField(oVal);

            if (oField == null)
            {
                return;
            }

            object oParent = this;

            ReflectionUtil.SetFieldValue(oVal, oField, oParent);
        }

        private void RemoveUniqueParentLinkIfNeeded(object oVal)
        {
            FieldInfo oField = GetUniqueParentLinkField(oVal);

            if (oField == null)
            {
                return;
            }

            object oCurrUniqueParent = ReflectionUtil.GetFieldValue(oVal, oField);

            // Reset UniqueParentLink in case we are the parent
            if (oCurrUniqueParent != null)
            {
                oCurrUniqueParent = oCurrUniqueParent;

                if (Object.ReferenceEquals(this, oCurrUniqueParent))
                {
                    ReflectionUtil.SetFieldValue(oVal, oField, null);
                }
            }
        }

        private FieldInfo GetUniqueParentLinkField(object oVal)
        {
            // Get TypeInfo of the value
            EntityTypeInfo oTypeInfo =
                Reflection.GetEntityTypeInfo(oVal);

            // Get a field that is a unique link to us
            FieldInfo oField =
                oTypeInfo.GetUniqueParentLinkField(this.GetType());

            return oField;
        }

        private void ValidateUniqueParentLinkField(object oVal, FieldInfo oField)
        {
            // Check the case that the type don't have UniqueParentLink to <this> type
            if (oField == null)
            {
                throw new TisException(
                    "Type [{0}] don't have UniqueParentLink to type [{1}]",
                    oVal.GetType(), this.GetType());
            }
        }

        #endregion

        #region ParentContainers

        public INamedObjectList[] ParentContainers
        {
            get
            {
                if (m_ParentContainers == null)
                {
                    m_ParentContainers = new List<INamedObjectList>();
                }

                return m_ParentContainers.ToArray();
            }
        }

        public void AddParentContainer(
            INamedObjectList oParentContainer,
            bool bOwner)
        {
			if (m_ParentContainers == null)
			{
				m_ParentContainers = new List<INamedObjectList>();
			}
			
			if (m_ParentContainers.Count == 0)
            {
                // Owner
                m_ParentContainers.Add(oParentContainer);
            }
            else
            {
				try
				{
					if (bOwner)
					{
						m_ParentContainers.Insert(0, oParentContainer);
					}
					else
					{
						m_ParentContainers.Add(oParentContainer);
					}
				}
				catch (Exception e)
				{
					throw new TisException(e,
						"Entity [{0}] already contains has [{1}] as parent",
						this,
						oParentContainer);
				}
            }
        }

        public void RemoveParentContainer(INamedObjectList oParentContainer)
        {
            int ind;
            if ((ind = Array.IndexOf(ParentContainers, oParentContainer)) < 0)
            {
                throw new TisException(
                    "Entity [{0}] don't contain [{1}] as parent",
                    this,
                    oParentContainer);
            }

            m_ParentContainers.RemoveAt(ind);
        }

        private void RestoreChildParentContainerArrays()
        {
            var iepsa = this as IEntityPostSerializationActions;
            RestoreChildParentContainerArrays(
               iepsa.GetOwnedChildrenLists(),
                true // Owner
                );

            RestoreChildParentContainerArrays(
                iepsa.GetLinkedChildrenLists(),
                false // Owner
                );
        }

        private void RestoreChildParentContainerArrays(IEnumerable<INamedObjectList> lists, bool bOwner)
        {
            // For all child containers of this entity
            foreach (var list in lists)
            {
                if (list != null)
                {
                    // For each element
                    foreach (INamedObject oObj in list)
                    {
                        IEntityBase oEntity = oObj as IEntityBase;

                        if (oEntity != null)
                        {
                            // Add container to ParentContainers list
                            oEntity.AddParentContainer(list, bOwner);
                        }
                    }
                }
            }
        }

        #endregion

        #region Children Containers

        private void SubcribeContainersEvents()
        {
            var iepsa = this as IEntityPostSerializationActions;
            // Subscribe Owned children containers
            foreach (var namedList in iepsa.GetOwnedChildrenLists())
            {
                if (namedList != null)
                {
                    namedList.OnObjectAdd += OwnedChildrenList_OnAdd;
                    namedList.OnObjectRemove += OwnedChildrenList_OnRemove;
                }
            }

            foreach (var namedList in iepsa.GetLinkedChildrenLists())
            {
                if (namedList != null)
                {
                    namedList.OnObjectAdd += LinkedChildrenList_OnAdd;
                    namedList.OnObjectRemove += LinkedChildrenList_OnRemove;
                }
            }
        }

        private void UpdateOwnerContainer(
            INamedObject oObj,
            INamedObjectList oContainer)
        {
            // Set child "OwnerParent" field
            SetObjectOwnerParent(
                oObj,
                this);

            AddChildCommon(
                oContainer,
                oObj,
                true);
        }

        private void UpdateNonOwnerContainer(
            INamedObject oObj,
            INamedObjectList oContainer)
        {
            AddChildCommon(
                oContainer,
                oObj,
                false);
        }

        public void DetachAllChildren()
        {
            //
            //	"Recursive" call that propogates to all owned children and causes
            //	them to remove all links 
            //

            // Remove all children

			var iepsa = this as IEntityPostSerializationActions;

			foreach (var item in iepsa.GetOwnedChildrenLists() )
			{
				item.Clear();
			}

			foreach (var item in iepsa.GetLinkedChildrenLists())
			{
				item.Clear();
			}
        }

        private void AddChildCommon(
            INamedObjectList oContainer,
            INamedObject oChild,
            bool bOwner)
        {
            // Register the container as parent of the entity
            (oChild as IEntityBase).AddParentContainer(oContainer, bOwner);

            SetUniqueParentLinkIfNeeded(oChild);
        }

        private void SetObjectOwnerParent(object oObj, object oParent)
        {
            EntityTypeInfo oChildTypeInfo =
                Reflection.GetEntityTypeInfo(oObj);

            FieldInfo oOwnerParentField = oChildTypeInfo.OwnerParent;

            if (oOwnerParentField != null)
            {
                object oTarget = oObj;

                // Set child "OwnerParent" field

                ReflectionUtil.SetFieldValue(oTarget, oOwnerParentField, oParent);
            }
        }

        #endregion

        #region Child Containers event handlers

        private void OwnedChildrenList_OnAdd(object sender, NamedEventArgs e)
        {
            INamedObjectList oContainer = (INamedObjectList)sender;
            IEntityBase oChildEntity = e.NamedObject as IEntityBase;

            if (oChildEntity == null)
            {
                return;
            }

            using (AutoTreeChangeNotify oChangeNotify = new AutoTreeChangeNotify(
                      this,
                      EntityTreeChange.ChildAdd,
                      this,
                      oChildEntity,
                      EntityRelation.Owner,
                      null))
            {
                UpdateOwnerContainer(oChildEntity, oContainer);
            }
        }

        private void OwnedChildrenList_OnRemove(object sender, NamedEventArgs e)
        {
            INamedObjectList oContainer = (INamedObjectList)sender;
            IEntityBase oChildEntity = e.NamedObject as IEntityBase;

            if (oChildEntity == null)
            {
                return;
            }

            using (AutoTreeChangeNotify oChangeNotify = new AutoTreeChangeNotify(
                      this,
                      EntityTreeChange.ChildRemove,
                      this,
                      oChildEntity,
                      EntityRelation.Owner,
                      null))
            {
                //
                //	The object oChild that we are it's owner is detached from the tree.
                //	1. All links to this object should be removed, i.e it should be removed 
                //     from all containers listed in m_ParentContainers.
                //	2. Reset UniqueParentLink of all linked children of the removed object.
                //     

                // Remove all links to child object

                // TODO: !!! Revert to INamedObjectList when RemoveObject will be part
                // of INamedObjectList interface
                foreach (INamedObjectList oParentContainer in oChildEntity.ParentContainers)
                {
                    try
                    {
                        // We don't want to remove from owner (our) container, since
                        // This operation is already done 
                        if (oParentContainer != oContainer)
                        {
                            oParentContainer.Remove(oChildEntity.Name);
                        }
                    }
                    catch (Exception oExc)
                    {
                        Log.WriteError(
                            "Object [{0}] was not removed from parent container " +
                            " [{1}]", oChildEntity, oParentContainer);

                        Log.WriteException(oExc);
                    }
                }

                // Detach all linked children of the removed object
                oChildEntity.DetachAllChildren();

                // Remove links from child object to parents
                RemoveUniqueParentLinkIfNeeded(oChildEntity);

                // Reset child "OwnerParent" field
                SetObjectOwnerParent(
                    oChildEntity,
                    null);
            }
        }

        private void LinkedChildrenList_OnAdd(object sender, NamedEventArgs e)
        {
            INamedObjectList oContainer = (INamedObjectList)sender;
            IEntityBase oChildEntity = e.NamedObject as IEntityBase;

            if (oChildEntity == null)
            {
                return;
            }

            using (AutoTreeChangeNotify oChangeNotify = new AutoTreeChangeNotify(
                      this,
                      EntityTreeChange.ChildAdd,
                      this,
                      oChildEntity,
                      EntityRelation.Link,
                      null))
            {
                UpdateNonOwnerContainer(
                    oChildEntity,
                    oContainer);
            }
        }

        private void LinkedChildrenList_OnRemove(object sender, NamedEventArgs e)
        {
            INamedObjectList oContainer = (INamedObjectList)sender;
            IEntityBase oChildEntity = e.NamedObject as IEntityBase;

            if (oChildEntity == null)
            {
                return;
            }

            using (AutoTreeChangeNotify oChangeNotify = new AutoTreeChangeNotify(
                      this,
                      EntityTreeChange.ChildRemove,
                      this,
                      oChildEntity,
                      EntityRelation.Link,
                      null))
            {
                // Register the container as parent of the entity
                oChildEntity.RemoveParentContainer(oContainer);

                RemoveUniqueParentLinkIfNeeded(oChildEntity);
            }
        }

        #endregion

        #region AutoTreeChangeNotify

        private class AutoTreeChangeNotify : IDisposable
        {
            private IEntityBase m_oTarget;
            private EntityTreeChangeInfo m_oChangeInfo;

            public AutoTreeChangeNotify(
                IEntityBase oTarget,
                EntityTreeChange enChange,
                IEntityBase oParentEntity,
                IEntityBase oChildEntity,
                EntityRelation enParentChildRelation,
                object oAdditionalInfo)
            {
                m_oTarget = oTarget;

                m_oChangeInfo = m_oTarget.PreTreeChange(
                    enChange,
                    oParentEntity,
                    oChildEntity,
                    enParentChildRelation,
                    oAdditionalInfo);
            }

            public void Dispose()
            {
                m_oTarget.PostTreeChange(m_oChangeInfo);
            }
        }

        #endregion


        void IEntityPostSerializationActions.SetParent(object parent)
        {
            OnSetParent(parent);
        }


		IEnumerable<object> IEntityPostSerializationActions.GetAggregatedChildren()
        {
			return OnGetAggregatedChildren();
		}

		//IEnumerable<IEntityPostSerializationActions> IEntityPostSerializationActions.GetOwnedChildren()
		//{
		//    throw new NotImplementedException();
		//}

		IEnumerable<INamedObjectList> IEntityPostSerializationActions.GetLinkedChildrenLists()
        {
            return OnGetLinkedChildernLists();
        }


        IEnumerable<INamedObjectList> IEntityPostSerializationActions.GetOwnedChildrenLists()
        {
            return OnGetChildernOwnedLists();
        }


        protected virtual void OnSetParent(object parent)
        {

        }

		protected virtual object OnGetParent()
		{
			return null;
		}

		protected virtual IEnumerable<object> OnGetAggregatedChildren()
		{
			yield break;
		}
		protected virtual IEnumerable<INamedObjectList> OnGetLinkedChildernLists()
        {
            yield break;
        }
        protected virtual IEnumerable<INamedObjectList> OnGetChildernOwnedLists()
        {
            yield break;
        }


		public object GetParent()
		{
			return OnGetParent();
		}


	}
}
