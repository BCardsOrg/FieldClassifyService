using System;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{
    /// <summary>
    /// 
    /// </summary>
    [ComVisible(true)]
	[Guid("CF723E96-D37C-45ec-851F-986438302AF1")]
	public interface IPage
	{
        /// <summary>
        /// Gets the deskew.
        /// </summary>
        /// <param name="iXSize">Size of the i x.</param>
        /// <param name="iYSize">Size of the i y.</param>
		void GetDeskew( ref int iXSize, ref int iYSize) ;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
		int Width
		{
			get ;
		}

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
		int Height
		{
			get ;
		}

        /// <summary>
        /// Gets the resolution.
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
		int Resolution
		{
			get ;
		}

        /// <summary>
        /// Loads the specified o raw data.
        /// </summary>
        /// <param name="oRawData">The o raw data.</param>
        /// <param name="oRawDataSize">Size of the o raw data.</param>
		void Load( 
			[ MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 ) ]
			byte [] oRawData,
			int oRawDataSize
			) ;

        /// <summary>
        /// Loads the specified s PRD file name.
        /// </summary>
        /// <param name="sPRDFileName">Name of the s PRD file.</param>
		void Load( string sPRDFileName ) ;

        /// <summary>
        /// Loads this instance.
        /// </summary>
		void Load() ;

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
		void SetNextChar( char cCharData, short iConfidence, int iLeft, int iTop, int iWidth, int iHeight, int iLine, int iWord ) ;

        /// <summary>
        /// Saves the specified s PRD file name.
        /// </summary>
        /// <param name="sPRDFileName">Name of the s PRD file.</param>
		void Save(  string sPRDFileName ) ;

        /// <summary>
        /// Gets the get no of lines.
        /// </summary>
        /// <value>
        /// The get no of lines.
        /// </value>
		int GetNoOfLines
		{
			get ;
		}

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <param name="iLineIndex">Index of the i line.</param>
        /// <returns></returns>
		ILine GetLine( int iLineIndex ) ;
	}




	

	
	
}
