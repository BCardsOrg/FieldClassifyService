using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{


    /// <summary>
    /// OCRLineResult class
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("7E1501DE-7990-4862-8BBE-EC1971139AF4")]
    public class OCRLineResult : ILine
    {
       
        TLine m_oLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="OCRLineResult"/> class.
        /// </summary>
        /// <param name="oLine">The o line.</param>
        public OCRLineResult(TLine oLine)
        {
            m_oLine = oLine;
        }
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
                return "";
            }
        }

        /// <summary>
        /// Gets the get no of words.
        /// </summary>
        /// <value>
        /// The get no of words.
        /// </value>
        public int GetNoOfWords
        {
            get
            {
                return m_oLine.Words.Count;
            }
        }

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <param name="iWordIndex">Index of the i word.</param>
        /// <returns></returns>
        public IWord GetWord(int iWordIndex)
        {
            TWord oWord = (TWord)m_oLine.Words[iWordIndex];
            return new OCRWordResult(oWord);
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public int Left
        {
            get
            {
                return m_oLine.Rect.Left;
            }
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public int Top
        {
            get
            {
                return m_oLine.Rect.Top;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get
            {
                return m_oLine.Rect.Width;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height
        {
            get
            {
                return m_oLine.Rect.Height;
            }
        }
    }
	
}
