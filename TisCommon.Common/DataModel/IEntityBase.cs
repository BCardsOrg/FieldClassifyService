using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public class TreeChangeEventArgs : EventArgs
    {
        public readonly EntityTreeEventTiming Timing;
        public readonly EntityTreeChangeInfo ChangeInfo;

        public TreeChangeEventArgs(
            EntityTreeEventTiming enTiming,
            EntityTreeChangeInfo oChangeInfo)
        {
            this.Timing = enTiming;
            this.ChangeInfo = oChangeInfo;
        }
    }

    public delegate void TreeChangeDelegate(TreeChangeEventArgs oArgs);

    [ComVisible(false)]
    public interface IEntityBase :
        INamedObject,
        IDeserializationCallback,
        IValidable,
        INamedObjectFactory
    {
        string Name { get; }
        string FullContextName { get; }
        void Rename(string sNewName);

        EntityTypeInfo TypeInfo { get; }
        IEntityBase OwnerParentEntity { get; }
        INamedObjectList OwnerContainter { get; }
        IEntityBase RootOwner { get; }
        IEntityBase[] OwnersChain { get; }
        INamedObjectList[] ParentContainers { get; }
        object GetObjectOwnerOfType(Type oType);
        ICollection GetParents(bool bMandatoryOnly);
        IEnumerable<object> GetOwnedChildren(bool bRecursive);
		IEnumerable<object> GetLinkedChildren(bool bRecursive);
        ICollection<object> GetChildrenOfType(Type oType, bool bRecursive);
        object FindNamedChild(Type oChildType, string sChildName, bool bRecursive);
        void SetOwnerParent(IEntityBase oParentEntity);
        bool IsOwnerParent(IEntityBase oEntity);
        object GetContextService(string sServiceName);
        object GetContextService(Type oServiceType);
        void OnPreFullContextNameChange(string sNewFullContextName);
        void TreeChangeNotify(EntityTreeEventTiming enTiming, EntityTreeChangeInfo oChangeInfo);
        void DetachAllChildren();
        void AddParentContainer(INamedObjectList oParentContainer, bool bOwner);
        void RemoveParentContainer(INamedObjectList oParentContainer);
        EntityTreeChangeInfo PreTreeChange(
            EntityTreeChange enChange,
            IEntityBase oParentEntity,
            IEntityBase oChildEntity,
            EntityRelation enParentChildRelation,
            object oAdditionalInfo);

        EntityTreeChangeInfo PostTreeChange(
            EntityTreeChange enChange,
            IEntityBase oParentEntity,
            IEntityBase oChildEntity,
            EntityRelation enParentChildRelation,
            object oAdditionalInfo);

        void PostTreeChange(EntityTreeChangeInfo oChangeInfo);

        event TreeChangeDelegate OnTreeChange;

        TisServicesAccessorBase ServicesAccessor { get; }
        void SetServicesAccessor(TisServicesAccessorBase servicesAccessor);
        //void SetContext(EntityTreeContext oContext);
        //EntityTreeContext Context { get; }

    }
}
