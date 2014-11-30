using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public delegate void UserTagsMapChangedDelegate(object oSender, object oKey, object oOldValue, object oNewValue);

    [ComVisible(false)]
    public interface ITisDataLayerTreeNodeEx : ITisDataLayerTreeNode, ITisDataLayerValidation
    {
        void CloneDataTo(ITisDataLayerTreeNode oNode);
        event UserTagsMapChangedDelegate OnUserTagsMapChanged;
        void WebCleanUp();
    }
}
