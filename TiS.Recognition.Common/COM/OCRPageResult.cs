using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{

    /// <summary>
    /// OCRPageResult class
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("68F2597A-44A5-47c0-B5D7-37058289A355")]
    public class OCRPageResult : IPage
    {
       
        private TPage m_oPage;
       
        private TLine m_oSetLine;
       
        private TWord m_oSetWord;

        /// <summary>
        /// Gets the deskew.
        /// </summary>
        /// <param name="iXSize">Size of the i x.</param>
        /// <param name="iYSize">Size of the i y.</param>
        public void GetDeskew(ref int iXSize, ref int iYSize)
        {
            iXSize = m_oPage.m_iDeskewX;
            iYSize = m_oPage.m_iDeskewY;
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
                return m_oPage.m_iImageWidth;
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
                return m_oPage.m_iImageHeight;
            }
        }

        /// <summary>
        /// Gets the resolution.
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
        public int Resolution
        {
            get
            {
                return m_oPage.ImageResolution;
            }
        }

        /// <summary>
        /// Loads the specified o raw data.
        /// </summary>
        /// <param name="oRawData">The o raw data.</param>
        /// <param name="oRawDataSize">Size of the o raw data.</param>
        public void Load(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
			byte[] oRawData,
           int oRawDataSize
           )
        {
            m_oPage = TPage.LoadFromRawData(oRawData);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            m_oPage = new TPage();
            m_oSetLine = new TLine();
            m_oSetWord = new TWord();
        }

        /// <summary>
        /// Sets the next character.
        /// </summary>
        /// <param name="cCharData">The c character data.</param>
        /// <param name="iConfidence">The i confidence.</param>
        /// <param name="iLeft">The i left.</param>
        /// <param name="iTop">The i top.</param>
        /// <param name="iWidth">Width of the i.</param>
        /// <param name="iHeight">Height of the i.</param>
        /// <param name="iLine">The i line.</param>
        /// <param name="iWord">The i word.</param>
        public void SetNextChar(char cCharData, short iConfidence, int iLeft, int iTop, int iWidth, int iHeight, int iLine, int iWord)
        {
            if ( ( cCharData == ' ' ) )
            {
                m_oSetLine.AddWord(m_oSetWord);
                m_oSetWord = new TWord();
            }
            else if ( cCharData == '\n' )
            {
                m_oPage.AddLine(m_oSetLine);
                m_oSetLine = new TLine();
            }
            else
            {
                TChar oSetChar = new TChar(cCharData, iConfidence, new TOCRRect(iLeft, iTop, iWidth, iHeight));
                m_oSetWord.AddChar(oSetChar);
            }
        }

        /// <summary>
        /// Saves the specified s PRD file name.
        /// </summary>
        /// <param name="sPRDFileName">Name of the s PRD file.</param>
        public void Save(string sPRDFileName)
        {
            TPage.SaveToPRD(m_oPage, sPRDFileName);
        }

        /// <summary>
        /// Loads the specified s PRD file name.
        /// </summary>
        /// <param name="sPRDFileName">Name of the s PRD file.</param>
        public void Load(string sPRDFileName)
        {
            m_oPage = TPage.LoadFromPRD(sPRDFileName);
        }

        /// <summary>
        /// Gets the get no of lines.
        /// </summary>
        /// <value>
        /// The get no of lines.
        /// </value>
        public int GetNoOfLines
        {
            get
            {
                return m_oPage.NoOfLines;
            }
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <param name="iLineIndex">Index of the i line.</param>
        /// <returns></returns>
        public ILine GetLine(int iLineIndex)
        {
            TLine oLine = (TLine)m_oPage.Lines[iLineIndex];
            return new OCRLineResult(oLine);
        }
    }
	
}
