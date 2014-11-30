using System;
using System.Collections.Generic;
using TiS.Core.TisCommon.Services;
using System.Runtime.Serialization;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Collections.Specialized;
using TiS.Core.TisCommon.Validation;
using TiS.Core.TisCommon.Customizations;
using TiS.Core.TisCommon.Attachment;
using TiS.Core.TisCommon.Attachment.File;

namespace TiS.Core.TisCommon.DataModel
{

    [ComVisible(false)]
    [Serializable]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow", IsReference = true)]
    [KnownType(typeof(Hashtable))]
    public class TisDataLayerTreeNode : EntityBase, ITisDataLayerTreeNodeEx, ITisDataLayerTreeNodeImport, IPersistKeyProvider
    {
        public const string PERSIST_KEY_SEPARATOR = "$";

        [DataMember]
        protected IDictionary m_oUserTags;

        [NonSerialized]
        [IgnoreDataMember]
        private IDictionary m_oPrivateData;

        [NonSerialized]
        [IgnoreDataMember]
        private TisValidator m_validator;

        [NonSerialized]
        [IgnoreDataMember]
        private TisValidatorManager m_validatorMngr;

        [NonSerialized]
        [IgnoreDataMember]
        private static TisValidationPolicy m_validationPolicy;

        [DataMember]
        private TisValidationsResult m_lastValidationsResult = new TisValidationsResult();

        [Serializable]
        [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
        private class RareNonPersistentData
        {
            [NonSerialized]
            [IgnoreDataMember]
            private IDictionary m_oRenamedNodeMap;

            [NonSerialized]
            [IgnoreDataMember]
            public ITisEntityReflection CachedReflectionExternal;

            // FindNamedChild optimization - mostly for Designer
            // That calls FindNamedChild a lot to detect changes in the tree
            [NonSerialized]
            [IgnoreDataMember]
            public NameTypeLookupTable ChildLookupTable;

            [NonSerialized]
            [IgnoreDataMember]
            public IImportObjectExistsNotify ObjectExistsNotifier;

            public IDictionary RenamedNodeMap
            {
                get
                {
                    if (m_oRenamedNodeMap == null)
                    {
                        m_oRenamedNodeMap = new Hashtable();
                    }

                    return m_oRenamedNodeMap;
                }
            }
        }

        [NonSerialized]
        [IgnoreDataMember]
        private RareNonPersistentData m_oRareNonPersistentData;

        [DataMember]
        private List<EventAssemblyInfo> m_boundEventsAssemblies = new List<EventAssemblyInfo>();

        public List<EventAssemblyInfo> BoundEventsAssemblies
        {
            get
            {
                return m_boundEventsAssemblies;
            }
        }

        //
        //	Public
        //	


        public TisDataLayerTreeNode()
        {
        }

        public TisDataLayerTreeNode(string sName)
            : base(sName)
        {
        }

        public TisDataLayerTreeNode(
            string sName,
            TisServicesAccessorBase servicesAccessor)
            : base(sName, servicesAccessor)
        {
        }

        #region ITisDataLayerTreeNode Members

        #region PrivateData

        public int GetPrivateData(short nIndex)
        {
            if (m_oPrivateData == null)
            {
                return 0;
            }

            object oVal = m_oPrivateData[nIndex];

            return oVal == null ? 0 : (int)oVal;
        }

        public void SetPrivateData(short nIndex, int nVal)
        {
            Log.WriteDetailedDebug("{0}.SetPrivateData(Index={1}, Val={2})",
                GetType().Name, nIndex, nVal);

            InitPrivateDataIfNeeded();

            m_oPrivateData[nIndex] = nVal;
        }

        #endregion

        #region UserTags

        public void SetNamedUserTags(string sTagName, string Value)
        {
            InternalSetUserTag(sTagName, Value);
        }

        public string GetNamedUserTags(string sTagName)
        {
            object oVal = InternalGetUserTag(sTagName);

            if (oVal == null)
            {
                return String.Empty;
            }

            return oVal.ToString();
        }


        public void SetUserTags(short TagIndex, string Value)
        {
            InternalSetUserTag(
                GetUserTagName(TagIndex),
                Value);
        }

        public string GetUserTags(short TagIndex)
        {
            object oVal = InternalGetUserTag(
                GetUserTagName(TagIndex)
                );

            if (oVal == null)
            {
                return String.Empty;
            }

            return oVal.ToString();
        }

        public IDictionary UserTagsMap
        {
            get
            {
                InitUserTagsIfNeeded();

                return m_oUserTags;
            }
        }

        public short NumberOfUserTags
        {
            get
            {
                if (m_oUserTags == null)
                {
                    return 0;
                }

                return (short)m_oUserTags.Count;
            }
            set
            {
                int nRequiredCount = value;

                if (nRequiredCount == 0)
                {
                    ClearUserTags();
                    return;
                }

                int nTagsToAdd = nRequiredCount - NumberOfUserTags;

                // Add empty tags
                for (int i = 0; i < nTagsToAdd; i++)
                {
                    SetUserTags(NumberOfUserTags, String.Empty);
                }
            }
        }

        public void ClearUserTags()
        {
            if (m_oUserTags != null)
            {
                m_oUserTags.Clear();

                m_oUserTags = null;
            }
        }

        public event UserTagsMapChangedDelegate OnUserTagsMapChanged;

        #endregion

        #region Reflection/TypeInfo

        public IEnumerable Children
        {
            get
            {
				return GetAllChildren(false);
            }
        }

        public IList GetChildrenOfType(
            string sTypeName,
            bool bRecursive)
        {
            List<object> oVector = new List<object>();

            oVector.AddRange(
                ReflectionExternal.GetChildrenOfType(this, sTypeName, bRecursive));

            return oVector;
        }

        public object GetParameter(string sParameterName)
        {
            ITisEntityParameterInfo oParamInfo =
                CheckedGetParameterInfo(sParameterName);

            return oParamInfo.GetValue(this);
        }

        public void SetParameter(string sParameterName, object oVal)
        {
            ITisEntityParameterInfo oParamInfo =
                CheckedGetParameterInfo(sParameterName);

            oParamInfo.SetValue(this, oVal);
        }

        public new ITisEntityTypeInfo TypeInfo
        {
            get
            {
                return new TisEntityTypeInfoExternal(
                    base.TypeInfo,
                    ReflectionExternal);
            }
        }

        public virtual object FindNamedChild(
            string sChildTypeName,
            string sChildName,
            bool bRecursive)
        {
            Type oChildType =
                ReflectionExternal.TypeNameToType(sChildTypeName);

            FillChildLookupIfRequired();

            object oNamedChild =
                Rare.ChildLookupTable.Get(oChildType, sChildName);

            if (oNamedChild == null)
            {
                if (oChildType != null)
                {
                    oNamedChild =
                        Rare.ChildLookupTable.Get(oChildType, sChildName);
                }
            }

            return oNamedChild;
        }

        public object CreateChild(
            string sChildTypeName,
            string sChildName)
        {
            return ReflectionExternal.CreateChild(
                this,
                sChildTypeName,
                sChildName);
        }

        public object ObjectOwner
        {
            get
            {
                return base.OwnerParent;
            }
        }

        public object GetObjectOwnerOfType(string sTypeName)
        {
            return ReflectionExternal.GetObjectOwnerOfType(
                this,
                sTypeName);
        }

        public object Root
        {
            get
            {
                return EntityReflection.GetRoot(this);
            }
        }

        public TisEntityReflection EntityReflection
        {
            get
            {
                return ((TisServicesAccessorBase)CheckedServicesAccessor).GetEntityReflection(PersistKeyPrefix);
            }
        }

        #endregion

        #region Cloning

		public ITisDataLayerTreeNode Clone(EntityCloneSpec enEntityCloneSpec)
		{
			return Clone(enEntityCloneSpec, null);
		}

		public virtual ITisDataLayerTreeNode Clone(EntityCloneSpec enEntityCloneSpec, Func<IEntityBase, string> nameingFun)
        {
            bool bCloneData = (enEntityCloneSpec & EntityCloneSpec.Data) > 0;
            bool bCloneOwnedChildren = (enEntityCloneSpec & EntityCloneSpec.OwnedChildren) > 0;
            bool bCloneLinkedChildren = (enEntityCloneSpec & EntityCloneSpec.LinkedChildren) > 0;


            // Performs clone, returns empty object
			ITisDataLayerTreeNode oCloned = this.CloneObject(nameingFun);

            // Clone parameters if requested
            if (bCloneData)
            {
                CloneDataTo(oCloned);
            }

            // Clone owned children if requested
            if (bCloneOwnedChildren)
            {
                CloneOwnedChildrenTo(oCloned, enEntityCloneSpec);
            }

            // Clone linked children if requested
            if (bCloneLinkedChildren)
            {
                CloneLinkedChildrenTo(oCloned, enEntityCloneSpec);
            }

            return oCloned;
        }

        public void CopyParametersTo(ITisDataLayerTreeNode oTarget)
        {
            CloneParametersTo(oTarget);
        }

        public void CopyUserTagsTo(ITisDataLayerTreeNode oTarget)
        {
            ((TisDataLayerTreeNode)oTarget).UserTagsMap.Clear();

            foreach (DictionaryEntry oEntry in this.UserTagsMap)
            {
                ((TisDataLayerTreeNode)oTarget).UserTagsMap.Add(oEntry.Key, oEntry.Value);
            }
        }

        #endregion

        #region Validation

        public bool HasValidator
        {
            get
            {
                return Validator != null;
            }
        }

        public TisValidationsResult LastValidationsResult
        {
            get
            {
                return m_lastValidationsResult ?? (m_lastValidationsResult = new TisValidationsResult());
            }
        }

        public TisValidationPolicy ValidationPolicy
        {
            get
            {
                if (m_validationPolicy == null)
                {
                    m_validationPolicy = this.ServicesAccessor.ValidationPolicy;
                }

                return m_validationPolicy;
            }
        }

        public ValidationStatus Validate(TisValidationPolicy validationPolicy)
        {
            ValidationStatus validationStatus = ValidationStatus.Unknown;

            TisValidator validator = Validator;

            if (validator != null)
            {
                if (validationPolicy == null)
                {
                    validationPolicy = ValidationPolicy;
                }

                if (m_lastValidationsResult == null)
                {
                    m_lastValidationsResult = new TisValidationsResult();
                }

                validationStatus =
                    validator.Validate(validationPolicy, m_lastValidationsResult);
            }

            return validationStatus;
        }

        protected virtual TisValidator GetValidator(Type validatorType)
        {
            if (m_validatorMngr == null)
            {
                m_validatorMngr =
                    (TisValidatorManager)GetContextService(TisServicesSchema.ValidatorManager);
            }

            if (m_validatorMngr != null)
            {

                if (m_validator == null)
                {
                    m_validator =
                        m_validatorMngr.GetValidatorByType(validatorType);
                }

                if (m_validator != null)
                {
                    m_validator.ValidationTarget = this;
                }
                else
                {
                    Log.WriteInfo("Type {0} has no validators defined.", TypeInfo.TypeName);
                }
            }
            else
            {
                Log.WriteError("Failed to obtain validator manager.");
            }

            return m_validator;
        }

        protected virtual TisValidator Validator
        {
            get
            {
                return GetValidator(this.GetType());
            }
        }

        #endregion

        string ITisDataLayerTreeNode.FullContextName
        {
            get { return ((EntityBase)this).FullContextName; }
        }

        #endregion

        #region Import

        public ITisDataLayerTreeNodeImport Import
        {
            get { return this; }
        }

        public void ImportNodesArray(
            ITisDataLayerTreeNode[] NodesToImport,
            IImportObjectExistsNotify oObjectExistsNotifier)
        {
            ImportNodesCollection(
                NodesToImport as ICollection,
                oObjectExistsNotifier);
        }

        public void ImportNodesCollection(
            ICollection NodesToImport,
            IImportObjectExistsNotify oObjectExistsNotifier)
        {
            foreach (ITisDataLayerTreeNode oNodeToImport in NodesToImport)
            {
                if (oNodeToImport != null)
                {
                    ImportNode(
                        oNodeToImport,
                        oObjectExistsNotifier,
                        (ITisDataLayerTreeNode[])EmptyArrays.ObjectArray);
                }
            }
        }

        public void ImportNode(
            ITisDataLayerTreeNode oNodeToImport,
            IImportObjectExistsNotify oObjectExistsNotifier,
            ITisDataLayerTreeNode[] OptionalLinks)
        {
            try
            {
                Rare.ObjectExistsNotifier = oObjectExistsNotifier;

                ITisDataLayerTreeNode oImportedRootOwner =
                    (ITisDataLayerTreeNode)((EntityBase)oNodeToImport).RootOwner;

                DLTNodesCopySession oMergeSession = new DLTNodesCopySession(
                    oImportedRootOwner,
                    (ITisDataLayerTreeNode)this.RootOwner
                    );

                oMergeSession.AddCopyRoot(oNodeToImport);

                oMergeSession.OnObjectExists += new ImportObjectExistsDelegate(
                    this.oCopySession_OnObjectExists);

                oMergeSession.PerformCopy(false);

                Rare.ObjectExistsNotifier = null;

                ICollection oSrcObjects = oMergeSession.GetAllSourceObjects();

                // Copy attachments
                foreach (EntityBase oSrcObj in oSrcObjects)
                {
                    ITisDataLayerTreeNode oSrcNode = oSrcObj as ITisDataLayerTreeNode;
                    ITisDataLayerTreeNode oCopiedObj =
                        oMergeSession.GetClonedObject(oSrcObj) as ITisDataLayerTreeNode;

                    if (oSrcNode != null && oCopiedObj != null)
                    {
                        CopyNodeAttachments(
                            oSrcNode,
                            oCopiedObj);
                    }
                }

            }
            catch (Exception oExc)
            {
                Log.Write(
                    Log.Severity.ERROR,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "Failed", oExc);

                Log.WriteException(oExc);

                throw;
            }
        }

        [field: IgnoreDataMember]
        public event ImportObjectExistsDelegate OnObjectExists;

        //void ITisDataLayerTreeNodeImport.add_OnObjectExists(ImportObjectExistsDelegate obj) { }
        //void ITisDataLayerTreeNodeImport.remove_OnObjectExists(ImportObjectExistsDelegate obj) { }

        #endregion

        public virtual void WebCleanUp()
        {
            if (m_validator != null)
            {
                m_validator.Dispose();
                m_validator = null;
            }

            if (m_validatorMngr != null)
            {
                m_validatorMngr.Dispose();
                m_validatorMngr = null;
            }

            IEnumerable<object> allChildren = GetAllChildren(true);

            foreach (TisDataLayerTreeNode child in allChildren)
            {
                child.WebCleanUp();
            }
        }

        public void CloneDataTo(ITisDataLayerTreeNode oNode)
        {
            this.CloneDataTo((object)oNode);
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            if (m_boundEventsAssemblies == null)
            {
                m_boundEventsAssemblies = new List<EventAssemblyInfo>();
            }
        }

        //
        //	Private
        //

        #region Import support

        private void CopyNodeAttachments(
            ITisDataLayerTreeNode oSrcTreeNode,
            ITisDataLayerTreeNode oDstTreeNode)
        {

			ISupportsAttachments oSrc = oSrcTreeNode as ISupportsAttachments;
			ISupportsAttachments oDst = oDstTreeNode as ISupportsAttachments;

			EntityBase oSrcEntity = oSrcTreeNode as EntityBase;
			EntityBase oDstEntity = oDstTreeNode as EntityBase;

			if (oSrc == null || oDst == null || oSrcEntity == null || oDstEntity == null)
			{
				return;
			}

			IList<string> oLocalAttachments = oSrc.LocalAttachments;

            ITisAttachedFileManager oSrcAttachedFileManager = (ITisAttachedFileManager)oSrcEntity.GetContextService(
                TisServicesSchema.SetupAttachmentsFileManager.ServiceName);

			ITisAttachedFileManager oDstAttachedFileManager = (ITisAttachedFileManager)oDstEntity.GetContextService(
				TisServicesSchema.SetupAttachmentsFileManager.ServiceName);

			foreach (string sAttachment in oLocalAttachments)
			{
				string sAttType = AttachmentsUtil.GetAttachmentType(sAttachment);
                string sDstAttName = oDst.GetAttachmentFileName(sAttType) ?? oDst.GetAttachmentNameByFileName(sAttachment);

				try
				{
                    if (!StringUtil.CompareIgnoreCase(sAttachment, sDstAttName))
                    {
                        if (!File.Exists(sAttachment))
                        {
                            oSrcAttachedFileManager.GetAttachment(sAttachment);
                        }

                        if (StringUtil.CompareIgnoreCase(sAttType, "EFI"))
                        {
                            // Special handling for EFIs
                            int nRetVal = FoLearn.CopyToNewEfi(
                                sAttachment,
                                sDstAttName);

                            if (nRetVal != 0)
                            {
                                throw new TisException("FoLearn.CopyToNewEfi failed, Code={0}", nRetVal);
                            }
                        }
                        else
                        {
                            // Copy local (cached)
                            File.Copy(
                                sAttachment,
                                sDstAttName,
                                true // Overwrite
                                );
                        }
                    }
				}
				catch (Exception oExc)
				{
					Log.WriteException(oExc);

					throw;
				}

				// Save to server
				oDstAttachedFileManager.SaveAttachment(
					sDstAttName,
					TIS_ATTACHMENT_EXISTS_ACTION.TIS_EXISTING_OVERRIDE);
			}
        }

        private void oCopySession_OnObjectExists(
            string sObjectName,
            string sObjectType,
            IImportOverwriteInfo oOverwriteInfo)
        {
            if (OnObjectExists != null)
            {
                OnObjectExists(
                    sObjectName,
                    sObjectType,
                    oOverwriteInfo);
            }
            else
            {
                // TEMP

                ImportAutoRename(
                    sObjectName,
                    oOverwriteInfo);
            }

            //			TEMPORARY CLOSED, ALWAYS PERFORM AutoRename IF COLLISION OCCURED DURING IMPORT

            //			if (Rare.ObjectExistsNotifier != null)
            //			{
            //				Rare.ObjectExistsNotifier.Notify (
            //					sObjectName, 
            //					sObjectType,
            //					oOverwriteInfo);
            //			}

            //			if (!StringUtil.CompareIgnoreCase (sObjectName, oOverwriteInfo.NewObjectName))
            //			{
            //				Rare.RenamedNodeMap.Add (sObjectName, oOverwriteInfo.NewObjectName);
            //			}
        }

        private void ImportAutoRename(
            string sObjectName,
            IImportOverwriteInfo oOverwriteInfo)
        {
            const string IMPORTED_SUFFIX = "_Imported";

            string sNewName;

            int nImportedSuffixIndex = sObjectName.LastIndexOf(IMPORTED_SUFFIX);

            if (nImportedSuffixIndex >= 0)
            {
                string sBaseName = sObjectName.Substring(0, nImportedSuffixIndex);

                string sImportNum =
                    sObjectName.Substring(nImportedSuffixIndex + IMPORTED_SUFFIX.Length);

                int nImportNum = 1;

                if (sImportNum.Length > 0)
                {
                    try
                    {
                        nImportNum = int.Parse(sImportNum) + 1;
                    }
                    catch
                    {
                    }
                }

                sNewName = String.Format("{0}{1}{2}", sBaseName, IMPORTED_SUFFIX, nImportNum);
            }
            else
            {
                sNewName = sObjectName + IMPORTED_SUFFIX;
            }

            oOverwriteInfo.NewObjectName = sNewName;
            oOverwriteInfo.Overwrite = true;
        }

        #endregion

        #region Cloning support

        // Creates an object of the same type, may be overriden if ctor parameters
        // should be different
        protected virtual ITisDataLayerTreeNode CloneObject(Func<IEntityBase, string> nameingFun)
        {
            // Default implementation
            return (ITisDataLayerTreeNode)Activator.CreateInstance(
                GetType(),
				new object[] { nameingFun == null ? Name : nameingFun(this) });
        }

        // May be overriden of object contains a complex data, not just parameters
        protected virtual void CloneDataTo(object oTargetObj)
        {
            // Clone all parameters
            CloneParametersTo(oTargetObj);

            // Copy UserTags

            ITisDataLayerTreeNode oTargetDLTNode =
                oTargetObj as ITisDataLayerTreeNode;

            if (oTargetDLTNode != null)
            {
                CopyUserTagsTo(oTargetDLTNode);
            }

        }

        private void CloneParametersTo(object oTargetObj)
        {
            foreach (ITisEntityParameterInfo oParamInfo in this.TypeInfo.Parameters)
            {
                if (!oParamInfo.IsReadOnly)
                {
                    try
                    {
                        // Copy parameter value
                        oParamInfo.SetValue(
                            oTargetObj,
                            oParamInfo.GetValue(this));
                    }
                    catch (Exception oExc)
                    {
                        continue;
                    }
                }
            }
        }

        private void CloneChildrenTo(
            ITisDataLayerTreeNode oTargetObj,
            ICollection<ITisEntityChildInfo> ChildrenInfoCollection,
            EntityCloneSpec enEntityCloneSpec)
        {
            foreach (ITisEntityChildInfo oChildInfo in ChildrenInfoCollection)
            {
                // Get source list
                INamedObjectList oSrcList = oChildInfo.GetChildList(this);

                // Get destination list
                INamedObjectList oDstList = oChildInfo.GetChildList(oTargetObj);

                // Add cloned objects to destination list

                TisDataLayerTreeNode oChildNode;

                for (int i = 0; i < oSrcList.Count; i++)
                {
                    if (oSrcList is INamedObjectOrder)
                    {
                        oChildNode =
                            ((oSrcList as INamedObjectOrder).GetByOrder(i)) as TisDataLayerTreeNode;
                    }
                    else
                    {
                        oChildNode =
                            (oSrcList[oSrcList.NameByIndex(i)]) as TisDataLayerTreeNode;
                    }

                    // Clone
                    ITisDataLayerTreeNode oClonedChildNode =
                        oChildNode.Clone(enEntityCloneSpec);

                    // Add to list
                    oDstList.Add(oClonedChildNode);
                }
            }
        }

        private void CloneOwnedChildrenTo(
            ITisDataLayerTreeNode oTargetObj,
            EntityCloneSpec enEntityCloneSpec)
        {
            CloneChildrenTo(
                oTargetObj,
                this.TypeInfo.OwnedChildren,
                enEntityCloneSpec);
        }

        private void CloneLinkedChildrenTo(
            ITisDataLayerTreeNode oTargetObj,
            EntityCloneSpec enEntityCloneSpec)
        {
            CloneChildrenTo(
                oTargetObj,
                this.TypeInfo.LinkedChildren,
                enEntityCloneSpec);
        }

        #endregion

        #region Child Lookup Table handling

        private void OnTreeChangeHandler(TreeChangeEventArgs oArgs)
        {
            // Invalidate child lookup table
            Rare.ChildLookupTable = null;

            // Unsubscribe TreeChange event
            RootOwner.OnTreeChange -= new TreeChangeDelegate(OnTreeChangeHandler);
        }

        private void FillChildLookupIfRequired()
        {
            if (Rare.ChildLookupTable == null)
            {
                FillChildLookup();
            }
        }

        private void FillChildLookup()
        {
            Rare.ChildLookupTable = new NameTypeLookupTable();

            Rare.ChildLookupTable.AddRange(
                GetOwnedChildren(true));

            Rare.ChildLookupTable.AddRange(
                GetLinkedChildren(true));

            // Subscribe TreeChange event
            RootOwner.OnTreeChange += new TreeChangeDelegate(OnTreeChangeHandler);
        }

        #endregion

        #region Reflection Helpers

        private ITisEntityParameterInfo CheckedGetParameterInfo(string sParameterName)
        {
            ITisEntityParameterInfo oParamInfo =
                this.TypeInfo.GetParameterInfo(sParameterName);

            if (oParamInfo == null)
            {
                throw new TisException(
                    "Parameter [{0}] not exist in object [{1}]",
                    sParameterName,
                    this);
            }

            return oParamInfo;
        }

        protected ITisEntityReflection ReflectionExternal
        {
            get
            {
                if (Rare.CachedReflectionExternal == null)
                {
                    Rare.CachedReflectionExternal = EntityReflection;
                }

                return Rare.CachedReflectionExternal;
            }
        }

        #endregion

        #region PrivateData & UserTags

        private void InitPrivateDataIfNeeded()
        {
            if (m_oPrivateData == null)
            {
                m_oPrivateData = new Hashtable(1);
            }
        }

        private void InitUserTagsIfNeeded()
        {
            if (m_oUserTags == null)
            {
                m_oUserTags = CollectionsUtil.CreateCaseInsensitiveHashtable(1);
            }
        }

        private string GetUserTagName(int nIndex)
        {
            return String.Format("UserTag${0,4:0000}", nIndex);
        }

        private object InternalGetUserTag(object oKey)
        {
            if (m_oUserTags == null)
            {
                return null;
            }

            return m_oUserTags[oKey];
        }

        private void InternalSetUserTag(object oKey, object oVal)
        {
            InitUserTagsIfNeeded();

            object oOldValue = m_oUserTags[oKey];

            m_oUserTags[oKey] = oVal;

            if (oOldValue != oVal && OnUserTagsMapChanged != null)
            {
                OnUserTagsMapChanged(m_oUserTags, oKey, oOldValue, oVal);
            }
        }

        #endregion

        private RareNonPersistentData Rare
        {
            get
            {
                if (m_oRareNonPersistentData == null)
                {
                    m_oRareNonPersistentData = new RareNonPersistentData();
                }

                return m_oRareNonPersistentData;
            }
        }

        #region IPersistKeyProvider Members

        protected virtual string PersistKeyPrefix
        {
            get
            {
                return String.Empty;
            }
        }

        public virtual string TypedPersistKey
        {
            get
            {
                return TypeInfo.TypeName;
            }
        }

        public virtual string FullPersistKey
        {
            get
            {
                return CalculateFullPersistKey(FullContextName);
            }
        }

        public virtual string CalculateFullPersistKey(string sFullContextName)
        {
            return TypedPersistKey + PERSIST_KEY_SEPARATOR + sFullContextName;
        }

        #endregion




	}
}
