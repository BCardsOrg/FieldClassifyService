using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{
    /// <summary>
    /// IWord interface
    /// </summary>
    [ComVisible(true)]
    [Guid("DC41FF74-4408-463a-9F00-922F207DFD08")]
    public interface IWord
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        string Data
        {
            get;
        }

        /// <summary>
        /// Gets the get no of chars.
        /// </summary>
        /// <value>
        /// The get no of chars.
        /// </value>
        int GetNoOfChars
        {
            get;
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="iCharIndex">Index of the i character.</param>
        /// <returns></returns>
        IChar GetChar(int iCharIndex);

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

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        int Style
        {
            get;
        }
    }

}
