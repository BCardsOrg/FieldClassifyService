using System;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.DataModel
{
    #region TisRect

    // Currently the TIS_RECT struct is declared in eFlowInterfaces DLL
    // which is auto-generated, and its structs can't contain code
    // Since we need custom GetHashCode implementation, we use this class
    // to store rectangles and convert to TIS_RECT when necessary
    [Serializable]
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
	[System.Runtime.InteropServices.ComVisible(false)]
    public struct TisRect
    {
        private int m_nTop;
        private int m_nLeft;
        private int m_nBottom;
        private int m_nRight;

        // Use AutoHashCode since a case when all points are zero is
        // very common, and it can cause a problem to generate a good
        // hash code
        [NonSerialized]
        private AutoHashCode m_oAutoHashCode;

        public TisRect(int nLeft, int nTop, int nRight, int nBottom)
        {
            m_nTop = nTop;
            m_nLeft = nLeft;
            m_nBottom = nBottom;
            m_nRight = nRight;

            m_oAutoHashCode = new AutoHashCode();
        }

        public TisRect(TIS_RECT oTisRect)
            : this(oTisRect.Left, oTisRect.Top, oTisRect.Right, oTisRect.Bottom)
        {
        }

		[DataMember]
        public int Right
        {
            get { return m_nRight; }
            set { m_nRight = value; }
        }

		[DataMember]
		public int Top
        {
            get { return m_nTop; }
            set { m_nTop = value; }
        }

		[DataMember]
		public int Left
        {
            get { return m_nLeft; }
            set { m_nLeft = value; }
        }

		[DataMember]
		public int Bottom
        {
            get { return m_nBottom; }
            set { m_nBottom = value; }
        }

        public override int GetHashCode()
        {
            return m_oAutoHashCode.GetHashCode();
        }

        public TIS_RECT ToTIS_RECT()
        {
            TIS_RECT oTisRect = new TIS_RECT();

            oTisRect.Top = m_nTop;
            oTisRect.Bottom = m_nBottom;
            oTisRect.Left = m_nLeft;
            oTisRect.Right = m_nRight;

            return oTisRect;
        }

        public static bool RectEquals(TIS_RECT rect1, TIS_RECT rect2)
        {
            return (
                    rect1.Top == rect2.Top &&
                    rect1.Bottom == rect2.Bottom &&
                    rect1.Left == rect2.Left &&
                    rect1.Right == rect2.Right
                    );
        }

    }

    #endregion

    #region TisMinMaxShort

    [Serializable]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisMinMaxShort
    {
        [DataMember]
        public short Min { get; set; }
        [DataMember]
        public short Max { get; set; }

        [IgnoreDataMember]
        AutoHashCode m_oAutoHashCode;

        public TisMinMaxShort(short nMin, short nMax)
        {
            Min = nMin;
            Max = nMax;

            m_oAutoHashCode = new AutoHashCode();
        }

        public TisMinMaxShort(TIS_MIN_MAX_SHORT oObj)
            : this(oObj.Min, oObj.Max)
        {

        }

        public override int GetHashCode()
        {
            return m_oAutoHashCode.GetHashCode();
        }

        public TIS_MIN_MAX_SHORT ToTIS_MIN_MAX_SHORT()
        {
            TIS_MIN_MAX_SHORT oVal = new TIS_MIN_MAX_SHORT();

            oVal.Min = Min;
            oVal.Max = Max;

            return oVal;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }

            if (obj.GetType() != typeof(TisMinMaxShort))
            {
                return false;
            }

            TisMinMaxShort oOther = (TisMinMaxShort)obj;

            return (Max == oOther.Max) && (Min == oOther.Min);
        }

    }

    #endregion

    #region TisPaddingInfo

    [Serializable]
    public struct TisPaddingInfo
    {
        private short m_nCharacter;
        private TIS_LEFT_RIGHT m_enDirection;
        private int m_nLength;

        [NonSerialized]
        AutoHashCode m_oAutoHashCode;

        public TisPaddingInfo(
            short nCharacter,
            TIS_LEFT_RIGHT enDirection,
            int nLength)
        {
            m_enDirection = enDirection;
            m_nCharacter = nCharacter;
            m_nLength = nLength;

            m_oAutoHashCode = new AutoHashCode();
        }

        public TisPaddingInfo(TIS_PADDING_INFO oObj)
            : this(oObj.Character, oObj.Direction, oObj.Length)
        {
        }

        public short Character
        {
            get { return m_nCharacter; }
            set { m_nCharacter = value; }
        }

        public TIS_LEFT_RIGHT Direction
        {
            get { return m_enDirection; }
            set { m_enDirection = value; }
        }

        public int Length
        {
            get { return m_nLength; }
            set { m_nLength = value; }
        }

        public override int GetHashCode()
        {
            return m_oAutoHashCode.GetHashCode();
        }

        public TIS_PADDING_INFO ToTIS_PADDING_INFO()
        {
            TIS_PADDING_INFO oVal = new TIS_PADDING_INFO();

            oVal.Character = m_nCharacter;
            oVal.Length = m_nLength;
            oVal.Direction = m_enDirection;

            return oVal;
        }

    }

    #endregion

    #region TisStringAndPosition

    [Serializable]
    public struct TisStringAndPosition
    {
        private int m_nPosition;
        private string m_sStr;

        [NonSerialized]
        AutoHashCode m_oAutoHashCode;

        public TisStringAndPosition(int nPos, string sStr)
        {
            m_nPosition = nPos;
            m_sStr = sStr;

            m_oAutoHashCode = new AutoHashCode();
        }

        public TisStringAndPosition(TIS_STRING_AND_POSITION oObj)
            : this(oObj.Position, oObj.Str)
        {

        }

        public int Position
        {
            get { return m_nPosition; }
            set { m_nPosition = value; }
        }

        public string Str
        {
            get { return m_sStr; }
            set { m_sStr = value; }
        }

        public override int GetHashCode()
        {
            return m_oAutoHashCode.GetHashCode();
        }

        public TIS_STRING_AND_POSITION ToTIS_STRING_AND_POSITION()
        {
            TIS_STRING_AND_POSITION oVal = new TIS_STRING_AND_POSITION();

            oVal.Str = m_sStr;
            oVal.Position = m_nPosition;

            return oVal;
        }

    }

    #endregion

    #region TisFormoutFlags

    [Serializable]
    public struct TisFormoutFlags
    {
        const uint RMOPTION_FRM_REGISTRATION = 0x00200000;
		const uint RMOPTION_BAD_BEGINNING = 0x00004000;
		const uint RMOPTION_NOT_ORIGINAL = 0x00000002;
		const uint RMOPTION_CHANGED_MARGINS = 0x00078000;

        private short m_nBadBeginning;
        private short m_nBadMargins;
        private short m_nNotOriginal;
        private short m_nRegistration;

        [NonSerialized]
        AutoHashCode m_oAutoHashCode;

        public TisFormoutFlags(
            short nBadBeginning,
            short nBadMargins,
            short nNotOriginal,
            short nRegistration)
        {
            m_nBadBeginning = nBadBeginning;
            m_nBadMargins = nBadMargins;
            m_nNotOriginal = nNotOriginal;
            m_nRegistration = nRegistration;

            m_oAutoHashCode = new AutoHashCode();
        }

        public TisFormoutFlags(TIS_FORMOUT_FLAGS oObj)
            : this(oObj.BadBeginning, oObj.BadMargins, oObj.NotOriginal, oObj.Registration)
        {
        }

        public TisFormoutFlags(int nFlags)
        {
            uint nFlagsValue = (uint)nFlags;

            m_oAutoHashCode = new AutoHashCode();

            m_nBadBeginning = ToShortFlag(BitUtil.IsBitSet(nFlagsValue, RMOPTION_BAD_BEGINNING));
            m_nBadMargins = ToShortFlag(BitUtil.IsBitSet(nFlagsValue, RMOPTION_CHANGED_MARGINS));
            m_nNotOriginal = ToShortFlag(BitUtil.IsBitSet(nFlagsValue, RMOPTION_NOT_ORIGINAL));
            m_nRegistration = ToShortFlag(BitUtil.IsBitSet(nFlagsValue, RMOPTION_FRM_REGISTRATION));

        }

        public short BadBeginning
        {
            get { return m_nBadBeginning; }
            set { m_nBadBeginning = value; }
        }

        public short BadMargins
        {
            get { return m_nBadMargins; }
            set { m_nBadMargins = value; }
        }

        public short NotOriginal
        {
            get { return m_nNotOriginal; }
            set { m_nNotOriginal = value; }
        }

        public short Registration
        {
            get { return m_nRegistration; }
            set { m_nRegistration = value; }
        }

        public override int GetHashCode()
        {
            return m_oAutoHashCode.GetHashCode();
        }

        public TIS_FORMOUT_FLAGS ToTIS_FORMOUT_FLAGS()
        {
            TIS_FORMOUT_FLAGS oVal = new TIS_FORMOUT_FLAGS();

            oVal.BadBeginning = m_nBadBeginning;
            oVal.BadMargins = m_nBadMargins;
            oVal.NotOriginal = m_nNotOriginal;
            oVal.Registration = m_nRegistration;

            return oVal;
        }

        public int ToInt()
        {
            uint nVal = 0;

            if (m_nBadBeginning > 0)
            {
                nVal |= RMOPTION_BAD_BEGINNING;
            }

            if (m_nBadMargins > 0)
            {
                nVal |= RMOPTION_CHANGED_MARGINS;
            }

            if (m_nNotOriginal > 0)
            {
                nVal |= RMOPTION_NOT_ORIGINAL;
            }

            if (m_nRegistration > 0)
            {
                nVal |= RMOPTION_FRM_REGISTRATION;
            }

            return (int)nVal;
        }

        public static int ToInt(TIS_FORMOUT_FLAGS oFlagsStruct)
        {
            return new TisFormoutFlags(oFlagsStruct).ToInt();
        }

        public static TIS_FORMOUT_FLAGS ToTIS_FORMOUT_FLAGS(int nFlags)
        {
            return new TisFormoutFlags(nFlags).ToTIS_FORMOUT_FLAGS();
        }

        //
        //	Private
        //

        private static short ToShortFlag(bool bVal)
        {
            if (bVal == true)
            {
                return 1;
            }

            return 0;
        }
    }

    #endregion
    
    [EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")]
    public struct TIS_CHAR_FILTER
    {
        public short ConfidenceFilter;
        public short Pad;
        public TIS_CHAR_RECT_FILTER RectFilter;
    }

    [EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")]
    public struct TIS_FORMOUT_FLAGS
    {
        public short BadBeginning;
        public short BadMargins;
        public short NotOriginal;

        public short Registration;
    }

    [EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")]
    public struct TIS_MIN_MAX_SHORT
    {
        public short Max;
        public short Min;
    }

    //<EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")> _
    //Public Structure TIS_BLOB
    //    <EmbedAttributeAttribute("MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)")> Public pData As Byte()
    //    Public Size As Integer
    //End Structure

    [EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")]
    public struct TIS_HISTORY_TAG
    {
        public int Flags;
        [EmbedAttributeAttribute("MarshalAs(UnmanagedType.BStr)")]
        public string Name;

        public int Version;
    }


    [EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")]
    public struct TIS_PADDING_INFO
    {
        public short Character;
        public TIS_LEFT_RIGHT Direction;

        public int Length;
    }

    [EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")]
    public struct TIS_RECT
    {

        public int Left;
        public int Top;
        public int Right;

        public int Bottom;
        public static TIS_RECT Zero
        {
            get { return new TIS_RECT(); }
        }

        public bool IsZero
        {
            get { return Bottom == 0 & Left == 0 & Right == 0 & Top == 0; }
        }
    }

    [EmbedAttributeAttribute("Serializable"), EmbedAttributeAttribute("StructLayout(LayoutKind.Sequential)")]
    public struct TIS_STRING_AND_POSITION
    {


        public int Position;
        [EmbedAttributeAttribute("MarshalAs(UnmanagedType.BStr)")]

        public string Str;
    }
}
