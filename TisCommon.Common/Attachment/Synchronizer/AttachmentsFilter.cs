using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using TiS.Core.TisCommon.DataModel;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Attachment.Synchronizer
{
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public enum TIS_ATTACHMENTS_FILTER
    {
        [EnumMember]
        ALL = 0,
        [EnumMember]
        NONE = 1,
    }

    #region ITisAttachmentsFilter

    [ComVisible(false)]
    public interface ITisAttachmentsFilter
    {
        bool IsPass(string attachment);
    }

    #endregion

    #region SpecificFileAttachmentsFilter

    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(false)]
    public class SpecificFileAttachmentsFilter : ITisAttachmentsFilter
    {
        private TIS_ATTACHMENTS_FILTER m_AttachmentsFilter;
        private string m_ExcludedFileName;
        private string m_ExcludedFileDirectory;

        //
        //	Public
        //

        public SpecificFileAttachmentsFilter(
            TIS_ATTACHMENTS_FILTER attachmentsFilter,
            string excludedFileName,
            string excludedFileDirectory = null)
        {
            m_AttachmentsFilter = attachmentsFilter;
            m_ExcludedFileName = excludedFileName;
            m_ExcludedFileDirectory = excludedFileDirectory;
        }

        public virtual bool IsPass(string attachment)
        {
          
            bool isExcluded = true;

            if (StringUtil.IsStringInitialized(m_ExcludedFileDirectory))
            {
                isExcluded = StringUtil.CompareIgnoreCase(attachment, Path.Combine(m_ExcludedFileDirectory, m_ExcludedFileName));
            }
            else
            {
                isExcluded = StringUtil.CompareIgnoreCase(Path.GetFileName(attachment), m_ExcludedFileName);
            }

            if (m_AttachmentsFilter == TIS_ATTACHMENTS_FILTER.ALL)
            {
                if (isExcluded)
                {
                    return false;
                }

                return true;
            }

            if (m_AttachmentsFilter == TIS_ATTACHMENTS_FILTER.NONE)
            {
                if (!isExcluded)
                {
                    return false;
                }

                return true;
            }

            throw new TisException("Unknown TIS_ATTACHMENTS_FILTER value: {0}", m_AttachmentsFilter);
        }
    }

    #endregion

    #region SimpleAttachmentsFilter

    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(false)]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class SimpleAttachmentsFilter : ITisAttachmentsFilter
    {
        [DataMember]
        private List<string> m_ExcludedAttachmentTypes;
        [DataMember]
        private TIS_ATTACHMENTS_FILTER m_AttachmentsFilter;

        public SimpleAttachmentsFilter(
            TIS_ATTACHMENTS_FILTER attachmentsFilter, 
            IList<string> excludedAttachmentTypes)
        {
            m_AttachmentsFilter = attachmentsFilter;

            // Create array of the same size
            m_ExcludedAttachmentTypes = new List<string>();

            for (int i = 0; i < excludedAttachmentTypes.Count; i++)
            {
                // Store all values as upper-case
                m_ExcludedAttachmentTypes.Add(excludedAttachmentTypes[i].ToUpper());
            }
        }

        #region ITisSetupAttachmentsFilter

        public virtual bool IsPass(string attachment)
        {
            string attachmentType = AttachmentsUtil.GetAttachmentType(attachment);

            if (m_AttachmentsFilter == TIS_ATTACHMENTS_FILTER.ALL)
            {
                if (isInExcludedAttachmentTypes(attachmentType))
                {
                    return false;
                }

                return true;
            }

            if (m_AttachmentsFilter == TIS_ATTACHMENTS_FILTER.NONE)
            {
                if (!isInExcludedAttachmentTypes(attachmentType))
                {
                    return false;
                }

                return true;
            }

            throw new TisException("Unknown TIS_ATTACHMENTS_FILTER value: {0}", m_AttachmentsFilter);
        }

        #endregion

        public List<string> ExcludedAttachmentTypes
        {
            get
            {
                return m_ExcludedAttachmentTypes;
            }
        }

        public SimpleAttachmentsFilter Clone()
        {
            return new SimpleAttachmentsFilter(m_AttachmentsFilter, m_ExcludedAttachmentTypes.ToArray());
        }

        public static SimpleAttachmentsFilter operator +(SimpleAttachmentsFilter firstFilter, SimpleAttachmentsFilter secondFilter)
        {
            foreach (string attachmentType in secondFilter.ExcludedAttachmentTypes)
            {
                if (!firstFilter.ExcludedAttachmentTypes.Contains(attachmentType))
                {
                    firstFilter.ExcludedAttachmentTypes.Add(attachmentType);
                }
            }

            return firstFilter;
        }

        public static SimpleAttachmentsFilter operator -(SimpleAttachmentsFilter firstFilter, SimpleAttachmentsFilter secondFilter)
        {
            foreach (string attachmentType in secondFilter.ExcludedAttachmentTypes)
            {
                if (firstFilter.ExcludedAttachmentTypes.Contains(attachmentType))
                {
                    firstFilter.ExcludedAttachmentTypes.Remove(attachmentType);
                }
            }

            return firstFilter;
        }

        //
        //	Private
        //

        private bool isInExcludedAttachmentTypes(string attachmentType)
        {
            if (m_ExcludedAttachmentTypes.Contains(attachmentType))
            {
                // Found in list
                return true;
            }

            return false;
        }
    }

    #endregion

    #region CompositeAttachmentsFilter

    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(false)]
    public class CompositeAttachmentsFilter : ITisAttachmentsFilter
    {
        private ArrayList m_Filters = new ArrayList();

        public CompositeAttachmentsFilter(params ITisAttachmentsFilter[] filters)
        {
            m_Filters.AddRange(filters);

            IsConjuctive = true;
        }

        public void AddFilter(ITisAttachmentsFilter filter)
        {
            m_Filters.Add(filter);
        }

        public bool IsConjuctive { get; set; }

        #region ITisSetupAttachmentsFilter

        public virtual bool IsPass(string attachment)
        {
            bool atLeastOnePassed = false;

            // Scan all filters
            for (int i = 0; i < m_Filters.Count; i++)
            {
                ITisAttachmentsFilter filter = (ITisAttachmentsFilter)m_Filters[i];

                if (filter.IsPass(attachment))
                {
                    atLeastOnePassed = true;
                }
                else
                {
                    if (IsConjuctive)
                    {
                        // Stop check & return
                        return false;
                    }
                }
            }

            return atLeastOnePassed;
        }

        #endregion

    }

    #endregion

    #region SingleFileAttachmentsFilter

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class SingleFileAttachmentsFilter :
        ITisAttachmentsFilter
    {
        private object m_oObject;
        private ITisEntityReflection m_oEntityReflection;

        //		private ICollection			  m_oObjectChildren;	

        private Hashtable m_oSupportedAttachmentNames;

        public SingleFileAttachmentsFilter(
            object oObject,
            ITisEntityReflection oReflectionServices)
        {
            m_oObject = oObject;
            m_oEntityReflection = oReflectionServices;

            if (oObject is EntityBase)
            {
                // Subscribe SubtreeChangedEvent in order to clean children
                // list cache if object subtree changes
                ((EntityBase)oObject).OnTreeChange
                    += new TreeChangeDelegate(OnObjectChanged);
            }
        }


        #region ITisSetupAttachmentsFilter

        public virtual bool IsPass(string sAttachment)
        {
            return IsBelongToObject(sAttachment, m_oObject);
        }

        #endregion

        //
        //	Private
        //

        private void AddObjectAttachmentNameToCache(
            ISupportsAttachments oObj)
        {
            string sAttName = AttachmentsUtil.GetAttachmentNameWithoutType(
                oObj.GetAttachmentFileNameWithoutPath(""));

            m_oSupportedAttachmentNames[sAttName] = true;
        }

        private bool IsBelongToObject(
            string sAttachmentFileName,
            object oObj)
        {

            if (m_oSupportedAttachmentNames == null)
            {
                //m_oSupportedAttachmentNames = new Hashtable(
                //    new CaseInsensitiveHashCodeProvider(),
                //    new CaseInsensitiveComparer());

                // Changed after moving from .NET 1.1
                // TODO : check whether we should use InvariantCulture or CurrentCulture
                m_oSupportedAttachmentNames = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

                if (oObj is ISupportsAttachments)
                {
                    AddObjectAttachmentNameToCache((ISupportsAttachments)oObj);
                }

                foreach (object oChildObj in (oObj as EntityBase).GetAllChildren(true).Where( x => x is ISupportsAttachments))
                {
                    AddObjectAttachmentNameToCache(
                        (ISupportsAttachments)oChildObj);
                }

            }

            string sAttName = AttachmentsUtil.GetAttachmentNameWithoutType(
                sAttachmentFileName);

            if (m_oSupportedAttachmentNames[sAttName] == null)
            {
                return false;
            }

            return true;

            //			if(IsBelongToObjectNonRecursive(
            //				sAttachmentFileName,
            //				oObj))
            //			{
            //				return true;
            //			}

            //			if(m_oObjectChildren == null)
            //			{
            //				ICollection oAllChildren = m_oEntityReflection.GetAllChildren(
            //					oObj, 
            //					true	// Recursive
            //					);
            //
            //				ArrayList oSupportAttachmentsChildren = new ArrayList(
            //					oAllChildren.Count);
            //
            //				foreach(object oChildObj in oAllChildren)
            //				{
            //					if(oChildObj is ISupportsAttachments)
            //					{
            //						oSupportAttachmentsChildren.Add(oChildObj);
            //					}
            //				}
            //
            //				m_oObjectChildren = oSupportAttachmentsChildren;
            //			}

            //			// Check all children
            //			foreach(object oChild in m_oObjectChildren)
            //			{
            //				if(IsBelongToObjectNonRecursive(sAttachmentFileName, oChild))
            //				{
            //					return true;
            //				}
            //			}

            //			return false;
        }

        private bool IsBelongToObjectNonRecursive(
            string sAttachmentFileName,
            object oObj)
        {
            // Cast to ISupportsAttachments
            ISupportsAttachments oSupportsAttachments = oObj as ISupportsAttachments;

            // Cast succeeded
            if (oSupportsAttachments != null)
            {
                // Get attachment type
                string sAttType = AttachmentsUtil.GetAttachmentType(
                    sAttachmentFileName);

                // Get file name for attachment type that can belong to object
                string sSupportedAttachmentFileName =
                    oSupportsAttachments.GetAttachmentFileName(sAttType);

                // Get attachment name (without path)
                string sAttachmentName =
                    AttachmentsUtil.GetAttachmentName(sAttachmentFileName);

                // Get supported attachment name (without path)
                string sSupportedAttachmentName =
                    AttachmentsUtil.GetAttachmentName(sSupportedAttachmentFileName);

                // Check if object supports the specified attachment
                // By comparing names 
                if (StringUtil.CompareIgnoreCase(
                    sSupportedAttachmentName,
                    sAttachmentName))
                {
                    // Passed the filter
                    return true;
                }
            }

            return false;
        }

        private void OnObjectChanged(TreeChangeEventArgs oArgs)
        {
            if (m_oSupportedAttachmentNames != null)
            {
                Log.Write(
                    Log.Severity.DETAILED_DEBUG,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "Clearing cache of attachment names");
            }

            //m_oObjectChildren = null;
            m_oSupportedAttachmentNames = null;
        }
    }

    #endregion
}
