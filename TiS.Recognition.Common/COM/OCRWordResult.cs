using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{

    /// <summary>
    /// OCRWordREsult class
    /// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased"), ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("EEEF5BEA-75D1-40e3-98A3-EA723AE88949")]
    public class OCRWordResult : IWord
    {
        TWord m_oWord;

        /// <summary>
        /// Initializes a new instance of the <see cref="OCRWordResult"/> class.
        /// </summary>
        /// <param name="oWord">The o word.</param>
        public OCRWordResult(TWord oWord)
        {
            m_oWord = oWord;
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
                return m_oWord.WordData;
            }
        }

        /// <summary>
        /// Gets the get no of chars.
        /// </summary>
        /// <value>
        /// The get no of chars.
        /// </value>
        public int GetNoOfChars
        {
            get
            {
                return m_oWord.Chars.Count;
            }
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public int Style
        {
            get
            {
                return (int)m_oWord.Style;
            }
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="iCharIndex">Index of the i character.</param>
        /// <returns></returns>
        public IChar GetChar(int iCharIndex)
        {
            TChar oChar = (TChar)m_oWord.Chars[iCharIndex];

            return new OCRCharResult(oChar);
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
                return m_oWord.Rect.Left;
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
                return m_oWord.Rect.Top;
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
                return m_oWord.Rect.Width;
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
                return m_oWord.Rect.Height;
            }
        }
    }
	
}
