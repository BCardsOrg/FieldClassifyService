using System;
using System.Collections.Generic;
using System.Text;

namespace TiS.Recognition.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOCRData
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        string Data { get;}
        /// <summary>
        /// Gets the confidance.
        /// </summary>
        /// <value>
        /// The confidance.
        /// </value>
        int Confidance { get; }
        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>
        /// The rectangle.
        /// </value>
        System.Drawing.Rectangle Rectangle { get; }
        
    }
}
