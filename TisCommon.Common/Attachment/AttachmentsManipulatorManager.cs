using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Attachment
{
    [ComVisible(false)]
    public class AttachmentsManipulatorManager
    {
        Dictionary<string, IAttachmentManipulator> m_allAttachmentManipulators;

        public AttachmentsManipulatorManager()
        {
            using (TisLocalServicesHost servicesHost = new TisLocalServicesHost())
            {
                InstantiateAllAttachmentManipulators(servicesHost);
            }
        }

        public void CopyAttachmentPages(
            string attachmentType,
            string sourceAttachmentFile,
            string destinantionAttachmentFile,
            IEnumerable<int> pageNumbers)
        {
            IAttachmentManipulator attachmentManipulator = ObtainAttachmentManipulator(attachmentType);

            if (attachmentManipulator != null)
            {
                attachmentManipulator.CopyAttachmentPages(sourceAttachmentFile, destinantionAttachmentFile, pageNumbers);
            }
        }

        public void RemoveAttachmentPages(
            string attachmentType, 
            string attachmentFile, 
            IEnumerable<int> pageNumbers)
        {
            IAttachmentManipulator attachmentManipulator = ObtainAttachmentManipulator(attachmentType);

            if (attachmentManipulator != null)
            {
                attachmentManipulator.RemoveAttachmentPages(attachmentFile, pageNumbers);
            }
        }

        private IAttachmentManipulator ObtainAttachmentManipulator(string attachmentType)
        {
            IAttachmentManipulator attachmentManipulator;

            if (!m_allAttachmentManipulators.TryGetValue(attachmentType, out attachmentManipulator))
            {
                Log.WriteWarning("Attachment type [{0}] is not supported.", attachmentType);
            }

            return attachmentManipulator;
        }

        private void InstantiateAllAttachmentManipulators(TisServicesHost servicesHost)
        {
            m_allAttachmentManipulators = new Dictionary<string, IAttachmentManipulator>();

            string[] attachmentManipulatorsServiceNames = TisServicesUtil.GetServicesOfImplType(
                    servicesHost.GetServiceRegistry(TisServicesConst.SystemApplication),
                    typeof(IAttachmentManipulator));

            IAttachmentManipulator attachmentManipulator;

            foreach (string attachmentManipulatorServiceName in attachmentManipulatorsServiceNames)
            {
                try
                {
                    attachmentManipulator =
                        (IAttachmentManipulator)servicesHost.GetService(TisServicesConst.SystemApplication, attachmentManipulatorServiceName);
                }
                catch
                {
                    attachmentManipulator = null;
                }

                if (attachmentManipulator != null)
                {
                    attachmentManipulator.SupportedAttachmentTypes.ForEach(supportedAttachmentType => m_allAttachmentManipulators.Add(supportedAttachmentType, attachmentManipulator));

                    Log.WriteInfo("Attachment manipulator service [{0}] is activated.", attachmentManipulatorServiceName);
                }
                else
                {
                    Log.WriteWarning("Attachment manipulator service [{0}] is not installed.", attachmentManipulatorServiceName);
                }
            }
        }
    }
}
