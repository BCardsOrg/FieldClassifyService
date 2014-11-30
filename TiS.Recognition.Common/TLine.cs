using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using TiS.Core.TisCommon;
using System.Collections;
using System.IO;
using System.Xml;

namespace TiS.Recognition.Common
{

    /// <summary>
    /// TLine
    /// </summary>
    [DataContract]
    [Serializable]
    public class TLine : IEnumerable<TWord>, IOCRData
    {
        // All words in the line sorted left to right
        /// <summary>
        /// The m_o words
        /// </summary>
        [DataMember]
        System.Collections.Generic.List<TWord> m_oWords;
        
        // Line rectangle
        /// <summary>
        /// The m_o rect
        /// </summary>
        [DataMember]
        TOCRRect m_oRect;
       
        // Sort comarere word in line
        /// <summary>
        /// The m_o comparer word left
        /// </summary>
        [IgnoreDataMember]
        [NonSerialized]
        TWord.TComparerLeft m_oComparerWordLeft;

        /// <summary>
        /// Initializes a new instance of the <see cref="TLine"/> class.
        /// </summary>
        public TLine()
        {
            m_oWords = new System.Collections.Generic.List<TWord>();
            m_oRect = new TOCRRect();
            m_oComparerWordLeft = new TWord.TComparerLeft();
        }

        /// <summary>
        /// Adds the word.
        /// </summary>
        /// <param name="oWord">The o word.</param>
        public void AddWord(TWord oWord)
        {
            m_oWords.Add(oWord);
            m_oWords.Sort(m_oComparerWordLeft);
            m_oRect.Add(oWord.Rect);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TWord> GetEnumerator()
        {
            return m_oWords.GetEnumerator();
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y
        {
            get
            {
                return (m_oRect.Top + m_oRect.Bottom) / 2;
            }
        }

        /// <summary>
        /// Words the specified i word.
        /// </summary>
        /// <param name="iWord">The i word.</param>
        /// <returns></returns>
        public TWord Word(int iWord)
        {
            return m_oWords[iWord] as TWord;
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
                return m_oRect;
            }
        }

        /// <summary>
        /// Gets or sets the words.
        /// </summary>
        /// <value>
        /// The words.
        /// </value>
        public System.Collections.Generic.IList<TWord> Words
        {
            get
            {
                return m_oWords;
            }
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="oLine">The o line.</param>
        /// <returns></returns>
        public int CompareTo(object oLine)
        {
            return m_oRect.Top - (oLine as TLine).Rect.Top;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder Result = new StringBuilder();

			if (m_oWords.Count > 0)
			{
				Result.Append(m_oWords[0].WordData);
				for (int i = 1; i < m_oWords.Count; i++)
				{
					Result.Append(" " + m_oWords[i].WordData);
				}
			}

            return Result.ToString();
        }



        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_oWords.GetEnumerator();
        }

        #endregion

        #region IOCRData Members

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (IOCRData word in m_oWords)
                {
                    builder.Append(word.Data);
                }
                return builder.ToString();
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
            get { return 100; }
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>
        /// The rectangle.
        /// </value>
        public System.Drawing.Rectangle Rectangle
        {
            get { return m_oRect; }
        }

        #endregion
    }
}

