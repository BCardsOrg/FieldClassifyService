using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using System.IO;


namespace TiS.Recognition.Common
{

    /// <summary>
    /// TWords
    /// </summary>
    [Serializable]
    public class TWords : System.Collections.Generic.List<TWord>
    { }

    /// <summary>
    /// TWord : contains basic word data
    /// </summary>
    [DataContract]
    [Serializable]
    public class TWord : ICloneable, IOCRData
    {
        // Array of TChar
        /// <summary>
        /// The m_o chars
        /// </summary>
        [DataMember]
        TChars m_oChars;
        // Word data
        /// <summary>
        /// The M_S word data
        /// </summary>
        [DataMember]
        string m_sWordData;
        // Word rectangle
        /// <summary>
        /// The m_o rectangle
        /// </summary>
        [DataMember]
        TOCRRect m_oRectangle;
        // The word confidence - the location confidence
        /// <summary>
        /// The m_i confidance
        /// </summary>
        [DataMember]
        internal short m_iConfidance;
        // Word style
        /// <summary>
        /// The m_i style
        /// </summary>
        [DataMember]
        TStyle m_iStyle;


        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum TStyle
        {
            /// <summary>
            /// Ocr
            /// </summary>
            OCR = 0x00000001,
            /// <summary>
            /// Icr
            /// </summary>
            ICR = 0x00000002,
            /// <summary>
            /// Dot matrix
            /// </summary>
            DotMatrix = 0x00000004,
            /// <summary>
            /// Italic
            /// </summary>
            Italic = 0x00000008,
            /// <summary>
            /// Bold
            /// </summary>
            Blod = 0x00000010,
            /// <summary>
            /// Underline
            /// </summary>
            UnderLine = 0x00000020,
            /// <summary>
            /// Monospace
            /// </summary>
            MonoSpace = 0x00000040,
            /// <summary>
            /// Serif
            /// </summary>
            Serif = 0x00000080,
            /// <summary>
            /// Sans serif
            /// </summary>
            SansSerif = 0x00000100,
            /// <summary>
            /// Gray background
            /// </summary>
            GrayBackgroud = 0x00000200,
            /// <summary>
            /// Black background
            /// </summary>
            BlackBackgroud = 0x00000400
        }
        // Return the second candidate word data
        //   iMaxDistance - (input) the char confidence distance that alow char to be consider as second candidate
        //                  In case thare is no char, then the first candidate is take.
        /// <summary>
        /// Gets the second candidate data.
        /// </summary>
        /// <param name="iMaxDistance">The i maximum distance.</param>
        /// <returns></returns>
        public string GetSecondCandidateData(int iMaxDistance)
        {
            string sWordCan2 = "";

            foreach (TChar oChar in m_oChars)
            {
                if (oChar.m_oCharCandidate.Count > 1)
                {
                    if ((oChar.m_oCharCandidate[1] as TBasicChar).Confidance + iMaxDistance >= oChar.Confidance)
                        sWordCan2 += (oChar.m_oCharCandidate[1] as TBasicChar).CharData;
                    else
                        sWordCan2 += oChar.CharData;
                }
                else
                {
                    sWordCan2 += oChar.CharData;
                }
            }
            return sWordCan2;
        }

        /// <summary>
        /// TComparerLeft
        /// </summary>
        public class TComparerLeft : IComparer<TWord>
        {


            #region IComparer<TWord> Members

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
            /// </returns>
            public int Compare(TWord x, TWord y)
            {
                return x.Rect.Left - y.Rect.Left;
            }

            #endregion
        }

        //
        // Public methods
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="TWord" /> class.
        /// </summary>
        public TWord()
        {
            m_oChars = new TChars();
            m_oRectangle = new TOCRRect();
            m_sWordData = "";
            m_iConfidance = 0;
            m_iStyle = 0;
        }
        //
        // Public methods
        //

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TChar> GetEnumerator()
        {
            return m_oChars.GetEnumerator();
        }

        /// <summary>
        /// Gets the chars.
        /// </summary>
        /// <value>
        /// The chars.
        /// </value>
        public TChars Chars
        {
            get
            {
                return m_oChars;
            }
        }

        /// <summary>
        /// Gets the confidence.
        /// </summary>
        /// <value>
        /// The confidence.
        /// </value>
        public int Confidance
        {
            get
            {
                return m_iConfidance;
            }
             set
            {
                m_iConfidance = (short)value;
            }

        }

        /// <summary>
        /// Gets the rect.
        /// </summary>
        /// <value>
        /// The rect.
        /// </value>
        public TOCRRect Rect
        {
            get
            {
                return m_oRectangle;
            }
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public TStyle Style
        {
            get
            {
                return (TStyle)m_iStyle;
            }
        }


        /// <summary>
        /// Sorts the candidates.
        /// </summary>
        public void SortCandidates()
        {
            // Step1: Sort each char 
            foreach (TChar oChar in m_oChars)
            {
                oChar.SortCandidates();
            }
            // Step2: Recalc word data & rectangle
            m_oRectangle.Empty();
            m_sWordData = "";
            foreach (TChar oChar in m_oChars)
            {
                // Update word rectangle
                m_oRectangle.Add(oChar.Rect);

                // Update word data
                m_sWordData += oChar.CharData;
            }
        }


        // 
        /// <summary>
        /// Determines whether the specified and other word are related.
        /// </summary>
        /// <param name="oOtherWord">The other word.</param>
        /// <returns>
        /// True if other word is "related" to the current word
        /// </returns>
        public Boolean IsRelated(TWord oOtherWord)
        {
            // First check we do not have ant chance for intersection
            if (!m_oRectangle.IsOverlap(oOtherWord.m_oRectangle))
                return false;

            foreach (TChar oChar in this)
            {
                foreach (TChar oOtherChar in oOtherWord)
                {
                    TOCRRect oOverlapRect = new TOCRRect();
                    if (oChar.Rect.OverlapRect(oOtherChar.Rect, oOverlapRect))
                    {
                        if (oOverlapRect.Area * 2 > oChar.Rect.Area)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        public void UpdateData()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TChar chr in Chars)
            {
                sb.Append(chr.Data);
            }
            m_sWordData = sb.ToString();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public System.Object Clone()
        {
            TWord oNewWord = (TWord)MemberwiseClone();
            oNewWord.m_oChars = new TChars();
            foreach (TChar oChar in m_oChars)
            {
                oNewWord.m_oChars.Add((TChar)oChar.Clone());
            }
            return oNewWord;
        }

        /// <summary>
        /// Adds the character to the word.
        /// </summary>
        /// <param name="oChar">The character.</param>
        public void AddChar(TChar oChar)
        {
            // Add TChar
            m_oChars.Add(oChar);

            // Update word rectangle
            m_oRectangle.Add(oChar.Rect);

            // Update word data
            m_sWordData += oChar.Data;
        }

        /// <summary>
        /// Adds the style.
        /// </summary>
        /// <param name="iStyle">The i style.</param>
        public void AddStyle(TStyle iStyle)
        {
            m_iStyle |= iStyle;
        }

        /// <summary>
        /// Gets the word data.
        /// </summary>
        /// <value>
        /// The word data.
        /// </value>
        public string WordData
        {
            get
            {
                return m_sWordData;
            }
        }

        #region IOCRData Members

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data
        {
            get { return m_sWordData; }
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>
        /// The rectangle.
        /// </value>
        public System.Drawing.Rectangle Rectangle
        {
            get { return m_oRectangle; }
        }

        #endregion
    }


}
