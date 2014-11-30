using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

namespace TiS.Core.TisCommon.DataModel
{
    [Guid("C58DF0BB-76B2-494A-A669-B3574F58AACE")]
    public interface IImportOverwriteInfo
    {
        bool Overwrite { get; set; }
        string NewObjectName { get; set; }
    }

    [Guid("CA1CA5DD-C576-4809-AC89-22A3853E5622")]
    public interface IImportObjectExistsNotify
    {
        void Notify(string sObjectName, string sObjectType, IImportOverwriteInfo oOverwriteInfo);
    }

    public delegate void ImportObjectExistsDelegate(string sObjectName, string sObjectType, IImportOverwriteInfo oOverwriteInfo);

    [Guid("7D430722-A3A8-4904-9FDF-1D31F05EF798")]
    public interface ITisDataLayerTreeNodeImport
    {
        void ImportNodesArray(ITisDataLayerTreeNode[] NodesToImport, IImportObjectExistsNotify oObjectExistsNotifier);
        void ImportNodesCollection(ICollection NodesToImport, IImportObjectExistsNotify oObjectExistsNotifier);
        void ImportNode(ITisDataLayerTreeNode oNodeToImport, IImportObjectExistsNotify oObjectExistsNotifier, ITisDataLayerTreeNode[] OptionalLinks);
        event ImportObjectExistsDelegate OnObjectExists;
    }

    [Guid("8A2611A9-3332-41CA-ABCE-FB72010D5885")]
	public interface ITisDataLayerTreeNode : INamedObject, IValidable
    {
        string FullContextName { get; }
        void Rename(String sNewName);
        int GetPrivateData(short nIndex);
        void SetPrivateData(short nIndex, int nVal);
        short NumberOfUserTags { get; set; }
        string GetUserTags(short TagIndex);
        void SetUserTags(short TagIndex, string Value);
        string GetNamedUserTags(string sTagName);
        void SetNamedUserTags(string sTagName, string Value);
        void ClearUserTags();
        object GetContextService(string sServiceName);
        ITisEntityTypeInfo TypeInfo { get; }
        void SetParameter(String sParameterName, Object oVal);
        object GetParameter(String sParameterName);
        object ObjectOwner { get; }
        object GetObjectOwnerOfType(String sTypeName);
        IEnumerable Children { get; }
        IList GetChildrenOfType(string sTypeName, bool bRecursive);
        object FindNamedChild(String sChildTypeName, String sChildName, Boolean bRecursive);
		ITisDataLayerTreeNode Clone(EntityCloneSpec enEntityCloneSpec);
		object CreateChild(String sChildTypeName, String sChildName);
        void CopyParametersTo(ITisDataLayerTreeNode oTarget);
        void CopyUserTagsTo(ITisDataLayerTreeNode oTarget);
        ITisDataLayerTreeNodeImport Import { get; }
        IDictionary UserTagsMap { get; }
    }
}
