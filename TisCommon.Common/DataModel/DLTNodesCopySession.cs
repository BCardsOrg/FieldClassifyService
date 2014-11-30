
using System;
namespace TiS.Core.TisCommon.DataModel
{
    public class DLTNodesCopySession : EntityTreeMergeSession
    {
		// A function to return a name for the new /clone entity
		Func<IEntityBase, string> m_nameingFun = null;

        public DLTNodesCopySession(
            ITisDataLayerTreeNode oSrcRoot,
            ITisDataLayerTreeNode oDstRoot,
		    Func<IEntityBase, string> nameingFun = null)
            : base((EntityBase)oSrcRoot, (EntityBase)oDstRoot)
        {
			m_nameingFun = nameingFun;
        }

        protected override IEntityBase CloneEntity(IEntityBase oObj)
        {
            TisDataLayerTreeNode oDLTNode = oObj as TisDataLayerTreeNode;

            if (oDLTNode != null)
            {
				return (EntityBase)oDLTNode.Clone(EntityCloneSpec.Data, m_nameingFun);
            }

            return base.CloneEntity(oObj);
        }

    }
}
