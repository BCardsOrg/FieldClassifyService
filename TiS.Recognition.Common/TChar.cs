using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TiS.Recognition.Common
{

    /// <summary>
    /// TChars
    /// </summary>
	[Serializable]
	public class TChars :  System.Collections.Generic.List<TChar>
	{ }


    /// <summary>
    /// TBasicChar
    /// </summary>
	[DataContract]
	[Serializable]
	public class TBasicChar : ICloneable, IComparable<TBasicChar>
	{

        /// <summary>
        /// The M_C character data
        /// </summary>
		[DataMember]
		char m_cCharData;

        /// <summary>
        /// The m_i confidance
        /// </summary>
		[DataMember]
		internal int m_iConfidance;

        /// <summary>
        /// The m_i original confi
        /// </summary>
		[IgnoreDataMember]
		[NonSerialized]
		int m_iOriginalConfi;


		//
		// Public methods
		//

        /// <summary>
        /// Initializes a new instance of the <see cref="TBasicChar" /> class.
        /// </summary>
        /// <param name="cCharData">The c character data.</param>
        /// <param name="iConfidance">The i confidance.</param>
		public TBasicChar(char cCharData, short iConfidance)
		{
			m_cCharData = cCharData;
			m_iConfidance = iConfidance;
			m_iOriginalConfi = iConfidance;
		}

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
		public System.Object Clone()
		{
			return MemberwiseClone();
		}

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="oBasicChar">The o basic character.</param>
        /// <returns></returns>
		public int CompareTo(TBasicChar oBasicChar)
		{
			return oBasicChar.m_iConfidance - m_iConfidance;
		}

        /// <summary>
        /// Gets the character data.
        /// </summary>
        /// <value>
        /// The character data.
        /// </value>
		public char CharData
		{
			get
			{
				return m_cCharData;
			}
		}

        /// <summary>
        /// Gets the confidance.
        /// </summary>
        /// <value>
        /// The confidance.
        /// </value>
		public int Confidance
		{
			get
			{
				return m_iConfidance;
			}
		}



	}


	// Description: Basic char data
    /// <summary>
    /// TChar
    /// </summary>
	[DataContract]
	[Serializable]
	public class TChar : ICloneable, IOCRData, IComparable<TChar>
	{
		// Array of TBasicChar for all char candidate
        /// <summary>
        /// The m_o character candidate
        /// </summary>
		[DataMember]
		internal List<TBasicChar> m_oCharCandidate;
		// The char rectangle [pixel]
        /// <summary>
        /// The m_o rectangle
        /// </summary>
		[DataMember]
		TOCRRect m_oRectangle;


		//
		// Public methods
		//

        /// <summary>
        /// Initializes a new instance of the <see cref="TChar"/> class.
        /// </summary>
		public TChar()
		{
			m_oRectangle = new TOCRRect();
			m_oCharCandidate = new List<TBasicChar>();
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="TChar"/> class.
        /// </summary>
        /// <param name="cCharData">The c character data.</param>
        /// <param name="iConfidance">The i confidance.</param>
        /// <param name="cCharDataCand2">The c character data cand2.</param>
        /// <param name="iConfidance2">The i confidance2.</param>
        /// <param name="oRectangle">The o rectangle.</param>
		public TChar(
			char cCharData,
			short iConfidance,
			char cCharDataCand2,
			short iConfidance2,
			TOCRRect oRectangle)
			: this(cCharData, iConfidance, oRectangle)
		{
			m_oCharCandidate.Add(new TBasicChar(cCharDataCand2, iConfidance2));
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TChar"/> class.
        /// </summary>
        /// <param name="cCharData">The c character data.</param>
        /// <param name="iConfidance">The i confidance.</param>
        /// <param name="oRectangle">The o rectangle.</param>
		public TChar(
			char cCharData,
			short iConfidance,
			TOCRRect oRectangle)
			: this()
		{
			m_oRectangle = new TOCRRect(oRectangle);
			m_oCharCandidate.Add(new TBasicChar(cCharData, iConfidance));
		}

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
		public System.Object Clone()
		{
			TChar oNewChar = (TChar)MemberwiseClone();

			oNewChar.m_oCharCandidate = new List<TBasicChar>();
			foreach ( TBasicChar oChar in m_oCharCandidate )
			{
				oNewChar.m_oCharCandidate.Add((TBasicChar)oChar.Clone());
			}
			return oNewChar;
		}


        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
		public IEnumerator<TBasicChar> GetEnumerator()
		{
			return m_oCharCandidate.GetEnumerator();
		}

        /// <summary>
        /// Sorts the candidates.
        /// </summary>
		public void SortCandidates()
		{
			m_oCharCandidate.Sort();
		}

		// Add candidate (remove duplicated is exist)
        /// <summary>
        /// Adds the candidate.
        /// </summary>
        /// <param name="oNewCandidate">The o new candidate.</param>
		public void AddCandidate(TChar oNewCandidate)
		{
			foreach ( TBasicChar oNewChar in oNewCandidate )
			{
				Boolean bAdd = false;
				int iIdx = -1;
				// Check if we already have candidate with the same data
				for ( int i = 0; ( i < m_oCharCandidate.Count ) && ( iIdx < 0 ); i++ )
				{
					TBasicChar oChar = (TBasicChar)m_oCharCandidate[i];
					if ( oChar.CharData == oNewChar.CharData )
					{
						iIdx = i;
					}
				}
				//int iIdx = m_oCharCandidate.BinarySearch( oNewChar ) ; xxx
				if ( iIdx >= 0 )
				{
					TBasicChar oExistChar = (TBasicChar)( m_oCharCandidate[iIdx] );
					// The same character have lower confidence - so we now replace the old with the new
					if ( oExistChar.CompareTo(oNewChar) > 0 )
					{
						m_oCharCandidate.RemoveAt(iIdx);
						bAdd = true;
					}
				}
				else
				{
					bAdd = true;
				}
				if ( bAdd )
				{
					// Add the new candidate (& sort acurding the candidates confidence )
					m_oCharCandidate.Add(oNewChar);
					m_oCharCandidate.Sort();
					// In case the new candidate is the best, then we take also the char rectangle.
					if ( m_oCharCandidate[0] == oNewChar )
						m_oRectangle = oNewCandidate.m_oRectangle;
				}
			}
		}

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			return new string(CharData, 1);
		}

        /// <summary>
        /// Gets the character data.
        /// </summary>
        /// <value>
        /// The character data.
        /// </value>
		public char CharData
		{
			get
			{
				return ( (TBasicChar)m_oCharCandidate[0] ).CharData;
			}
		}

        /// <summary>
        /// Gets the confidance.
        /// </summary>
        /// <value>
        /// The confidance.
        /// </value>
		public int Confidance
		{
			get
			{
				return ( (TBasicChar)m_oCharCandidate[0] ).Confidance;
			}
		}


		// Return char rectangle
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

		#region IOCRData Members

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
		public string Data
		{
			get { return CharData.ToString(); }
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


		#region IComparable<TChar> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
		public int CompareTo(TChar other)
		{
			return other.Confidance - Confidance;
		}

		#endregion

        /// <summary>
        /// 
        /// </summary>
		public class ConfidanceComparer : IComparer<TChar>
		{

			#region IComparer<TChar> Members

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
            /// </returns>
			public int Compare(TChar x, TChar y)
			{
				return x.Confidance - y.Confidance;
			}

			#endregion
		}

        /// <summary>
        /// Compares the left.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
		public static int CompareLeft(TChar x, TChar y)
		{
			return x.m_oRectangle.Left - y.m_oRectangle.Left;
		}
	}

}
