using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public class EntityTreeChangeInfo
    {
        public readonly EntityTreeChange Change;
        public readonly IEntityBase Entity;
        public readonly IEntityBase ChildEntity;
        public readonly EntityRelation ChildRelation;
        public readonly object AdditionalInfo;
        public readonly object Proxy;

        public EntityTreeChangeInfo(
            EntityTreeChange enChange,
            IEntityBase oEntity,
            IEntityBase oChildEntity,
            EntityRelation enChildRelation,
            object oAdditionalInfo)
        {
            Change = enChange;
            Entity = oEntity;
            ChildEntity = oChildEntity;
            ChildRelation = enChildRelation;
            AdditionalInfo = oAdditionalInfo;
        }
    }
}
