using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{
    /// <summary>
    /// 
    /// </summary>
    [ComVisible(true)]
    [Guid("F85FC2A6-EE8E-464a-9F5C-B1FF71B38527")]
    public interface IChar
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        char Data
        {
            get;
        }

        /// <summary>
        /// Gets the confidence.
        /// </summary>
        /// <value>
        /// The confidence.
        /// </value>
        int Confidence
        {
            get;
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        int Left
        {
            get;
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        int Top
        {
            get;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        int Width
        {
            get;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        int Height
        {
            get;
        }
    }
	
}
