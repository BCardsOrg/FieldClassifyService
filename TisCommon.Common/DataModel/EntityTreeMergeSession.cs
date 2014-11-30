using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace TiS.Core.TisCommon.DataModel
{
    public delegate EntityBase EntityCloneDelegate(
        IEntityBase oEntity);

    public class EntityTreeMergeSession
    {
        private const int DEF_NODES = 20000;

        private EntityBase m_oSrcRoot;
        private EntityBase m_oDstRoot;

        // Original object -> Object in target tree
        private IDictionary m_oGraph = new Hashtable(DEF_NODES);

        // EntityBase (Copy Root) -> same object 
        private IDictionary m_oCopyRoots = new Hashtable(DEF_NODES);

        // EntityBase (Copy Root) -> same object 
        private ICollection m_oForeignRoots;

        //
        //	Public
        //

        public event ImportObjectExistsDelegate OnObjectExists;

        public event EntityCloneDelegate EntityCloneHandler;

        public EntityTreeMergeSession(
            EntityBase oSrcRoot,
            EntityBase oDstRoot)
        {
            m_oSrcRoot = oSrcRoot;
            m_oDstRoot = oDstRoot;
        }

        public void AddCopyRoot(object oObj)
        {
            EntityBase oEntity = oObj as EntityBase;

            // Validate is EntityBase
            if (oObj == null)
            {
                throw new TisException("Object [{0}] is not EntityBase", oObj);
            }

            // Validate that entity belongs to source tree
            if (!oEntity.IsOwnerParent(m_oSrcRoot))
            {
                throw new TisException(
                    "Entity {0} not belongs to tree with root {1}",
                    oEntity,
                    m_oSrcRoot);
            }

            if (m_oCopyRoots.Contains(oObj))
            {
                // Exists already, ignore
                return;
            }

            // Validate entity don't have parents that already added as Copy Root
            foreach (EntityBase oOwnerEntity in oEntity.OwnersChain)
            {
                if (m_oCopyRoots.Contains(oOwnerEntity))
                {
                    throw new TisException(
                        "Owner [{0}] of entity [{1}] is already copy root",
                        oOwnerEntity,
                        oEntity);
                }
            }

            m_oCopyRoots.Add(oObj, oObj);
        }

        public void AddCopyRoots(
            ICollection oObjects)
        {
            // Add only EntityBase objects
            foreach (object oObj in oObjects)
            {
                AddCopyRoot(oObj);
            }
        }

        public void PerformCopy(
            bool bRemoveObjectsFromSource)
        {
            // Map tree roots
            m_oGraph[m_oSrcRoot] = m_oDstRoot;

            CopyDefinedSubTrees();

            FindAndCopyForeignObjects();

            RestoreLinks();

            if (bRemoveObjectsFromSource)
            {
                // RemoveObjectsFromSource
                RemoveObjectsFromSource();
            }
        }

        public ICollection GetClonedObjects(ICollection oOrigObjects)
        {
            ArrayList oList = new ArrayList();

            foreach (EntityBase oOrigObject in oOrigObjects)
            {
                oList.Add(
                    GetClonedObject(oOrigObject)
                    );
            }

            return oList;
        }

        public EntityBase GetClonedObject(EntityBase oOrigObject)
        {
            EntityBase oCopiedObj = (EntityBase)m_oGraph[oOrigObject];

            if (oCopiedObj == null)
            {
                throw new TisException("Object [{0}] was not copied", oOrigObject);
            }

            return oCopiedObj;
        }

        public ICollection GetAllSourceObjects()
        {
            return m_oGraph.Keys;
        }

        //
        //	Protected
        //

        protected virtual IEntityBase CloneEntity(IEntityBase oObj)
        {
            if (oObj is ICloneable)
            {
                return (IEntityBase)((ICloneable)oObj).Clone();
            }

            return null;
        }

        //
        //	Private
        //

        #region Clone

        private IEntityBase PerformClone(IEntityBase oEntity)
        {
            IEntityBase oCloned = null;

            if (EntityCloneHandler != null)
            {
                oCloned = EntityCloneHandler(oEntity);
            }

            if (oCloned == null)
            {
                oCloned = CloneEntity(oEntity);
            }

            if (oCloned == null)
            {
                throw new TisException("Entity {0} can't be cloned");
            }

            return oCloned;
        }

        #endregion

        #region Foreign objects handling

        private void FindAndCopyForeignObjects()
        {
            do
            {
                FindForeignObjects();

                CopyForeignObjects();

                AddForeignRootsToCopyRoots();
            }
            while (m_oForeignRoots.Count > 0);
        }

        private void FindForeignObjects()
        {
            Hashtable oForeignObjects = new Hashtable();

            foreach (EntityBase oEntity in m_oGraph.Keys)
            {
                FindForeignObjects(
                    oEntity,
                    oForeignObjects);
            }

            m_oForeignRoots =
                FindRootObjects(oForeignObjects);
        }

        private void CopyForeignObjects()
        {
            CopySubTrees(m_oForeignRoots);
        }

        private void AddForeignRootsToCopyRoots()
        {
            foreach (EntityBase oEntity in m_oForeignRoots)
            {
                m_oCopyRoots.Add(oEntity, oEntity);
            }
        }

        private void FindForeignObjects(
            EntityBase oEntity,
            Hashtable oForeignObjects)
        {
            // Add owner parent if required
            // NOTE !!!: Limitation, only one level is checked and added
            //AddForeignObjectIfRequired(oEntity.OwnerParentEntity, oForeignObjects);

            ICollection oParents = oEntity.GetParents(
                true // bMandatoryOnly
                );

            foreach (object oParent in oParents)
            {
                EntityBase oParentEntity = oParent as EntityBase;

                AddForeignObjectIfRequired(oParentEntity, oForeignObjects);
            }


            // Add linked children
			IEnumerable<object> oLinkedChildren =
                oEntity.GetLinkedChildren(false);

            foreach (object oChild in oLinkedChildren)
            {
                EntityBase oChildEntity = oChild as EntityBase;

                AddForeignObjectIfRequired(oChildEntity, oForeignObjects);
            }
        }

        private void AddForeignObjectIfRequired(EntityBase oEntity, Hashtable oForeignObjects)
        {
            if (oEntity != null && !IsBelongToCopyGraph(oEntity))
            {
                // Foreign object
                oForeignObjects[oEntity] = oEntity;
            }
        }


        #endregion

        #region SubTree copy

        private void CopyDefinedSubTrees()
        {
            CopySubTrees(m_oCopyRoots.Keys);
        }

        private void CopySubTrees(ICollection oRoots)
        {
            EntityBase[] SortedRoots = new EntityBase[oRoots.Count];

            oRoots.CopyTo(SortedRoots, 0);

            Array.Sort(SortedRoots, new OwnerListOrderComparer());

            foreach (EntityBase oRoot in SortedRoots)
            {
                CopySubTree(oRoot);
            }
        }

        private void CopySubTree(IEntityBase oRoot)
        {
            IEntityBase[] Owners = oRoot.OwnersChain;

            // Find SrcRoot location in chain of owners
            int nSrcRootIndex = Array.IndexOf(Owners, m_oSrcRoot);

            if (nSrcRootIndex < 0)
            {
                throw new TisException("SrcRoot [{0}] is not owner of entity [{1}]", m_oSrcRoot, oRoot);
            }

            // Start one level after SrcRoot
            for (int i = nSrcRootIndex + 1; i < Owners.Length; i++)
            {
                IEntityBase oOwner = Owners[i];

                if (!IsBelongToCopyGraph(oOwner))
                {
                    CopyObject(
                        oOwner,
                        false // WithOwnedChildren
                        );
                }
            }

            IEntityBase oCopiedEntity = CopyObject(
                oRoot,
                true // WithOwnedChildren
                );
        }

        #endregion

        #region Copy objects

        private IEntityBase CopyObject(
            IEntityBase oEntity,
            bool bWithOwnedChildren)
        {
            IEntityBase oParentEntity = oEntity.OwnerParentEntity;

            if (oParentEntity == null)
            {
                throw new TisException("Entity {0} don't have EntityBase parent", oEntity);
            }

            EntityBase oDstParent = (EntityBase)m_oGraph[oParentEntity];

            if (oDstParent == null)
            {
                throw new TisException(
                    "Entity [{0}] must exists in target tree", oParentEntity);
            }

			Type oSrcContainerType = oEntity.OwnerContainter.GetType();

			var desLIst = (oDstParent as IEntityPostSerializationActions).GetOwnedChildrenLists().Where(x => x.GetType() == oSrcContainerType).FirstOrDefault();
			if (desLIst != null)
			{ 					
				return CopyObject(
						desLIst as INamedObjectList,
						oEntity,
						bWithOwnedChildren);
			}
			else			
				throw new TisException("Failed to get entity [{0}] in destination object", oEntity);
        }

        private IEntityBase CopyObject(
            INamedObjectList oDstList,
            IEntityBase oEntity,
            bool bWithOwnedChildren)
        {
            IEntityBase oClonedEntity =
                PerformClone(oEntity);

            IEntityBase oDstListEntity = oClonedEntity;

            try
            {
                oDstList.Add(oClonedEntity);
            }
            catch
            {
                // Collision

                string sObjName = oEntity.Name;

                // Determine what to do
                CollisionAction enCollisionAction = GetCollisionAction(
                    ref sObjName,
                    oEntity.TypeInfo.TheType.Name,
                    oDstList);

                bool bRetryAdd = true;

                switch (enCollisionAction)
                {
                    case CollisionAction.Rename:
                        oClonedEntity.Rename(sObjName);
                        break;
                    case CollisionAction.Overwrite:
                        oDstList.Remove(oEntity.Name);
                        break;
                    case CollisionAction.KeepOriginal:
                        oDstListEntity = (EntityBase)oDstList[oEntity.Name];
                        bRetryAdd = false;
                        break;
                }

                if (bRetryAdd)
                {
                    oDstList.Add(oClonedEntity);
                }
            }

            m_oGraph[oEntity] = oDstListEntity;

            if (bWithOwnedChildren)
            {
                CopyOwnedChildren(
                    oEntity,
                    oDstListEntity,
                    true  // Recursive
                    );
            }

            return oDstListEntity;
        }

        private void CopyOwnedChildren(
            IEntityBase oSrcEntity,
            IEntityBase oDstEntity,
            bool bRecursive)
        {
			var srcIepsa = oSrcEntity as IEntityPostSerializationActions;
			var dstIepsa = oDstEntity as IEntityPostSerializationActions;
			foreach (var oSrcChildList in srcIepsa.GetOwnedChildrenLists())
			{
				// Find the correct destination list
				var oDstChildList = dstIepsa.GetOwnedChildrenLists().Where(x => x.GetType() == oSrcChildList.GetType()).FirstOrDefault();

				if (oDstChildList != null)
				{
					IEnumerator oSrcEnumerator = GetListEnumerator(oSrcChildList);

					while (oSrcEnumerator.MoveNext())
					{
						EntityBase oChildEntity = oSrcEnumerator.Current as EntityBase;

						if (oChildEntity == null)
						{
							continue;
						}

						CopyObject(
							oDstChildList,
							oChildEntity,
							bRecursive);
					}
				}

			}

		}

        private bool IsBelongToCopyGraph(IEntityBase oEntity)
        {
            return m_oGraph.Contains(oEntity);
        }

        #endregion

        #region Collision

        private enum CollisionAction { Rename, Overwrite, KeepOriginal }

        private CollisionAction GetCollisionAction(
            ref string sObjectName,
            string sObjTypeName,
            INamedObjectList oTargetList)
        {
            //			const string IMPORTED_SUFFIX = "_Imported";
            //
            //			System.Text.StringBuilder oNewObjectName = new System.Text.StringBuilder();
            //			oNewObjectName.Append(sObjectName);
            //			oNewObjectName.Append(IMPORTED_SUFFIX);
            //			
            //			int nImportedSuffixIndex = sObjectName.IndexOf(IMPORTED_SUFFIX);
            //
            //			if(nImportedSuffixIndex >= 0)
            //			{
            //				string sImportNum = 
            //					sObjectName.Substring(nImportedSuffixIndex + IMPORTED_SUFFIX.Length);
            //
            //				int nImportNum = 1;
            //
            //				if(sImportNum.Length > 0)
            //				{
            //					try
            //					{
            //						nImportNum = int.Parse(sImportNum) + 1;
            //					}
            //					catch
            //					{
            //					}
            //				}
            //
            //				oNewObjectName.Append(nImportNum.ToString());
            //			}
            //
            //			sObjectName = oNewObjectName.ToString();
            //
            //			return CollisionAction.Rename;


            if (OnObjectExists != null)
            {
                string sPrevObjectName = sObjectName;

                ImportOverwriteInfo oOverwriteInfo = new ImportOverwriteInfo(
                    false, // Overwrite
                    sObjectName);

                string sCurrObjName = sObjectName;

                // If rename specified, loop until a valid name is provided
                do
                {
                    OnObjectExists(
                        sCurrObjName,
                        sObjTypeName,
                        oOverwriteInfo);

                    sCurrObjName = oOverwriteInfo.NewObjectName;
                }
                while (oOverwriteInfo.Overwrite && sCurrObjName != sPrevObjectName && oTargetList[sCurrObjName] != null);

                sObjectName = sCurrObjName;

                //	Currently callback misses the specific Rename & KeepOriginal flags
                //  so we need to check if the NewObjectName was set to other name in order
                //  to determine Rename action.

                if (oOverwriteInfo.Overwrite)
                {
                    if (sObjectName != sPrevObjectName)
                    {
                        return CollisionAction.Rename;
                    }

                    return CollisionAction.Overwrite;
                }
                else
                {
                    return CollisionAction.KeepOriginal;
                }
            }

            // Default
            return CollisionAction.KeepOriginal;
        }

        #endregion

        #region Remove Objects

        public void RemoveObjectsFromSource()
        {
            ArrayList oAllRoots = new ArrayList();

            oAllRoots.AddRange(m_oCopyRoots.Keys);
            oAllRoots.AddRange(m_oForeignRoots);

            foreach (EntityBase oEntity in oAllRoots)
            {
				INamedObjectList oOwnerContainer = oEntity.OwnerContainter as INamedObjectList;
                  //  GetOwnerContainer(oEntity);

                if (oOwnerContainer == null)
                {
                    continue;
                }

                oOwnerContainer.Remove(oEntity.Name);
            }
        }

        #endregion

        #region Utils

        private void RemoveObjects(
            INamedObjectList oList,
            ICollection oObjects)
        {
            foreach (INamedObject oObj in oObjects)
            {
                oList.Remove(oObj.Name);
            }
        }

        private ICollection FindRootObjects(Hashtable oObjects)
        {
            ArrayList oRoots = new ArrayList();

            foreach (EntityBase oEntity in oObjects.Keys)
            {
                if (!oObjects.Contains(oEntity.OwnerParentEntity))
                {
                    oRoots.Add(oEntity);
                }
            }

            return oRoots;
        }

        private IEnumerator GetListEnumerator(INamedObjectList oList)
        {
            INamedObjectOrder oOrder = oList as INamedObjectOrder;

            IEnumerator oEnumerator = null;

            if (oOrder != null)
            {
                oEnumerator = oOrder.GetOrderedEnumerator();
            }
            else
            {
                oEnumerator = oList.GetEnumerator();
            }

            return oEnumerator;
        }

        #endregion

        #region RestoreLinks

        private void RestoreLinks()
        {
            foreach (DictionaryEntry oEntry in m_oGraph)
            {
                RestoreLinks(
                    (EntityBase)oEntry.Key,
                    (EntityBase)oEntry.Value);
            }
        }

        private void RestoreLinks(
            EntityBase oEntity,
            EntityBase oClonedEntity)
        {
			var orginalIepsa = oEntity as IEntityPostSerializationActions;
			var cloneIepsa = oClonedEntity as IEntityPostSerializationActions;
			foreach (var oOriginalChildList in orginalIepsa.GetLinkedChildrenLists())
			{
				// Find the correct destination list
				var oCloneChildList = cloneIepsa.GetLinkedChildrenLists().Where(x => x.GetType() == oOriginalChildList.GetType()).FirstOrDefault();

				if (oCloneChildList != null)
				{
					IEnumerator oOrgEnumerator = GetListEnumerator(oOriginalChildList);

					while (oOrgEnumerator.MoveNext())
					{
						EntityBase oChildEntity = oOrgEnumerator.Current as EntityBase;

						if (oChildEntity == null)
						{
							continue;
						}

						if (!oCloneChildList.Contains(oChildEntity.Name))
						{
							EntityBase oClonedLinkedObj = (EntityBase)m_oGraph[oChildEntity];
							if (oClonedLinkedObj == null)
							{
								throw new TisException("No cloned object found for entity {0}", oChildEntity);
							}
							oCloneChildList.Add(oClonedLinkedObj);
						}
					}
				}
			}
        }

        #endregion

        private class OwnerListOrderComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object oObjEntity1, object oObjEntity2)
            {
                EntityBase oEntity1 = oObjEntity1 as EntityBase;
                EntityBase oEntity2 = oObjEntity2 as EntityBase;

                if (oEntity1 == null || oEntity2 == null)
                {
                    return 0;
                }

                return GetOrderInOwnerContainer(oEntity1) - GetOrderInOwnerContainer(oEntity2);
            }

            private int GetOrderInOwnerContainer(EntityBase oEntity)
            {
                INamedObjectList oOwnerContainer = oEntity.OwnerContainter;

                int nOrder = -1;

                if (oOwnerContainer != null)
                {
                    INamedObjectOrder oOrder = oOwnerContainer as INamedObjectOrder;

                    if (oOrder != null)
                    {
                        nOrder = oOrder.GetOrderByName(oEntity.Name);
                    }
                }

                return nOrder;
            }

            #endregion
        }
    }
}
