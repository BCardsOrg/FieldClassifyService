using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Attachment;

namespace TiS.Core.TisCommon.DataModel
{
    [ComVisible(false)]
    public class DataLayerTreeNodeServices
    {
        public static void CopyAttachments(
            ICollection oSrcObjects,
            ICollection oTargetObjects)
        {
            if (oSrcObjects.Count != oTargetObjects.Count)
            {
                throw new TisException("oSrcObjects.Count != oTargetObjects.Count");
            }

            IEnumerator oSrcEnumerator = oSrcObjects.GetEnumerator();
            IEnumerator oTargetEnumerator = oTargetObjects.GetEnumerator();

            while (oSrcEnumerator.MoveNext() && oTargetEnumerator.MoveNext())
            {
                ISupportsAttachments oSrcObject =
                    oSrcEnumerator.Current as ISupportsAttachments;

                ISupportsAttachments oTargetObject =
                    oTargetEnumerator.Current as ISupportsAttachments;

                if (oSrcObject != null && oTargetObject != null)
                {
                    CopyAttachments(oSrcObject, oTargetObject);
                }
            }

        }

        public static void CopyAttachments(
            ISupportsAttachments oSource,
            ISupportsAttachments oTarget)
        {
            IList<string> Attachments = oSource.LocalAttachments;

            foreach (string sAttachment in Attachments)
            {
                // Get attachment type
                string sAttType = AttachmentsUtil.GetAttachmentType(sAttachment);

                // Create target file name
                string sTargetAttFileName =
                    oTarget.GetAttachmentFileName(sAttType);

                // Copy file
                System.IO.File.Copy(
                    sAttachment,
                    sTargetAttFileName,
                    true // Overwrite
                    );
            }
        }

        public static void UpdateNames(ICollection oObjects)
        {
            foreach (object oObj in oObjects)
            {
                IAutoNamed oAutoNamedObj = oObj as IAutoNamed;

                if (oAutoNamedObj != null)
                {
                    oAutoNamedObj.UpdateName();
                }
            }
        }

        public static string[] GetFullContextNames(ICollection oDLTNodes)
        {
            List<string> oFullContextNames = new List<string>();

            foreach (ITisDataLayerTreeNode oDLTNode in oDLTNodes)
            {
                oFullContextNames.Add(oDLTNode.FullContextName);
            }

            return oFullContextNames.ToArray();
        }

        public static string[] GetNames(ICollection oDLTNodes)
        {
            List<string> oNames = new List<string>();

            foreach (ITisDataLayerTreeNode oDLTNode in oDLTNodes)
            {
                oNames.Add(oDLTNode.Name);
            }

            return oNames.ToArray();
        }


    }
}
