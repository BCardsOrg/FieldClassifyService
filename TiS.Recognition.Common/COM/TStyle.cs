using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Recognition.Common.COM
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    [ComVisible(true)]
    [Guid("DC41FF74-4408-463a-9F01-922F207DFD08")]
    public enum TStyle
    {
        /// <summary>
        /// OCR
        /// </summary>
		OCR = 0x00000001,
        /// <summary>
        /// ICR
        /// </summary>
		ICR = 0x00000002,
        /// <summary>
        /// Dot matrix
        /// </summary>
        DotMatrix = 0x00000004,
        /// <summary>
        /// Italic
        /// </summary>
        Italic = 0x00000008,
        /// <summary>
        /// Bold
        /// </summary>
        Blod = 0x00000010,
        /// <summary>
        /// Underline
        /// </summary>
        UnderLine = 0x00000020,
        /// <summary>
        /// Monospace
        /// </summary>
        MonoSpace = 0x00000040,
        /// <summary>
        /// Serif
        /// </summary>
        Serif = 0x00000080,
        /// <summary>
        /// Sans serif
        /// </summary>
        SansSerif = 0x00000100,
        /// <summary>
        /// Gray background
        /// </summary>
        GrayBackgroud = 0x00000200,
        /// <summary>
        /// Black background
        /// </summary>
        BlackBackgroud = 0x00000400
    }
}
