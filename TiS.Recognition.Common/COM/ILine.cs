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
    [Guid("B87F92E8-1CA7-419e-B7EE-D0B886FFF1A1")]
    public interface ILine
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
        /// Gets the get no of words.
        /// </summary>
        /// <value>
        /// The get no of words.
        /// </value>
        int GetNoOfWords
        {
            get;
        }

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <param name="iWordIndex">Index of the i word.</param>
        /// <returns></returns>
        IWord GetWord(int iWordIndex);

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
