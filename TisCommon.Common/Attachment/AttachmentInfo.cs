using System;

namespace TiS.Core.TisCommon.Attachment
{
    public interface ITisAttachmentInfo
    {
        string AttachmentName { get; }
        string AttachmentType { get; }
        DateTime LastModified { get; set; }
        int CRC32 { get; }
        long SizeBytes { get; set; }
    }

    public class AttachmentInfo : ITisAttachmentInfo
	{
		string   m_sAttType;
		string   m_sAttName;
		long     m_lSizeBytes;
		DateTime m_oLastModified;
		int      m_nCRC32;

		public AttachmentInfo(
			string   sAttName,
			string   sAttType,
			long     lSizeBytes,
			DateTime oLastModified,
			int      nCRC32)
		{
			m_sAttType      = sAttType;
			m_sAttName      = sAttName;
			m_lSizeBytes    = lSizeBytes;
			m_oLastModified = oLastModified;
			m_nCRC32        = nCRC32;
		}

		#region ITisAttachmentInfo Members

		public string AttachmentType
		{
			get
			{
				return m_sAttType;
			}
		}

		public long SizeBytes
		{
			get
			{
				return m_lSizeBytes;
			}
            set
            {
                m_lSizeBytes = value;
            }
		}

		public DateTime LastModified
		{
			get
			{
				return m_oLastModified;
			}
            set
            {
                m_oLastModified = value;
            }
		}

		public string AttachmentName
		{
			get
			{
				return m_sAttName;
			}
		}

		public int CRC32
		{
			get
			{
				return m_nCRC32;
			}
		}

		#endregion
	}
}
