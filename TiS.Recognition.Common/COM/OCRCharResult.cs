using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{
    /// <summary>
    /// OCRCharResult
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("735AF1FB-E5D0-4686-A29D-12D86CA5C239")]
    public class OCRCharResult : IChar
    {
        /// <summary>
        /// The m_o character
        /// </summary>
        TChar m_oChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="OCRCharResult"/> class.
        /// </summary>
        /// <param name="oChar">The o character.</param>
        public OCRCharResult(TChar oChar)
        {
            m_oChar = oChar;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public char Data
        {
            get
            {
                return m_oChar.CharData;
            }
        }

        /// <summary>
        /// Gets the confidence.
        /// </summary>
        /// <value>
        /// The confidence.
        /// </value>
        public int Confidence
        {
            get
            {
                return m_oChar.Confidance;
            }
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
                return m_oChar.Rect.Left;
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
                return m_oChar.Rect.Top;
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
                return m_oChar.Rect.Width;
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
                return m_oChar.Rect.Height;
            }
        }
    }
}
