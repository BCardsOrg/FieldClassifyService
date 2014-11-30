using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TiS.Recognition.Common
{

    /// <summary>
    /// TOCRRect class
    /// </summary>
    [DataContract]
	[Serializable]
	public class TOCRRect
    {
        // The basic rectangle data
        /// <summary>
        /// Left
        /// </summary>
		[DataMember]
        int m_iLeft;
        /// <summary>
        /// Top
        /// </summary>
		[DataMember]
		int m_iTop;
        /// <summary>
        /// Width
        /// </summary>
		[DataMember]
		int m_iWidth;
        /// <summary>
        /// Height
        /// </summary>
		[DataMember]
		int m_iHeight;
        
		// threshold for X-axis
        /// <summary>
        /// x-axis threshold
        /// </summary>
        static int s_iXThreshold;
        // threshold for Y-axis
        /// <summary>
        /// threshold for y-axis threshold
        /// </summary>
        static int s_iYThreshold;

        //
        // Public methods
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="TOCRRect"/> class.
        /// </summary>
        public TOCRRect()
        {
            m_iWidth = -1;
            m_iHeight = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TOCRRect"/> class.
        /// </summary>
        /// <param name="iLeft">The i left.</param>
        /// <param name="iTop">The i top.</param>
        /// <param name="iWidth">Width of the i.</param>
        /// <param name="iHeight">Height of the i.</param>
        public TOCRRect(int iLeft, int iTop, int iWidth, int iHeight)
        {
            m_iLeft = iLeft;
            m_iTop = iTop;
            m_iWidth = iWidth;
            m_iHeight = iHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TOCRRect"/> class.
        /// </summary>
        /// <param name="oRect">The o rect.</param>
        public TOCRRect(TOCRRect oRect)
        {
            m_iLeft = oRect.m_iLeft;
            m_iTop = oRect.m_iTop;
            m_iWidth = oRect.m_iWidth;
            m_iHeight = oRect.m_iHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TOCRRect"/> class.
        /// </summary>
        /// <param name="oRect">The o rect.</param>
				public TOCRRect(System.Drawing.Rectangle oRect)
				{
					m_iLeft = oRect.Left;
					m_iTop = oRect.Top;
					m_iWidth = oRect.Width;
					m_iHeight = oRect.Height;
				}

                /// <summary>
                /// Sets the thresholds.
                /// </summary>
                /// <param name="iXThreshold">The i x threshold.</param>
                /// <param name="iYThreshold">The i y threshold.</param>
        static public void SetThresholds(int iXThreshold, int iYThreshold)
        {
            s_iXThreshold = iXThreshold;
            s_iYThreshold = iYThreshold;
        }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns></returns>
        public Boolean IsEmpty()
        {
            if ( m_iWidth < 0 )
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines whether the specified o other rect is equal.
        /// </summary>
        /// <param name="oOtherRect">The o other rect.</param>
        /// <returns></returns>
        public Boolean IsEqual(TOCRRect oOtherRect)
        {
					int thresholdX = Math.Min(s_iXThreshold, Width / 2);
					int thresholdY = Math.Min(s_iYThreshold, Height / 2);

					return (Math.Abs(m_iLeft - oOtherRect.m_iLeft) <= thresholdX) &&
							(Math.Abs(Right - oOtherRect.Right) <= thresholdX) &&
							(Math.Abs(m_iTop - oOtherRect.m_iTop) <= thresholdY) &&
							(Math.Abs(Bottom - oOtherRect.Bottom) <= thresholdY);
        }

        /// <summary>
        /// Determines whether [is equal top] [the specified o other rect].
        /// </summary>
        /// <param name="oOtherRect">The o other rect.</param>
        /// <returns></returns>
        public Boolean IsEqualTop(TOCRRect oOtherRect)
        {
            return ( Math.Abs(m_iTop - oOtherRect.m_iTop) <= s_iYThreshold );
        }

        /// <summary>
        /// Determines whether [is equal bottom] [the specified o other rect].
        /// </summary>
        /// <param name="oOtherRect">The o other rect.</param>
        /// <returns></returns>
        public Boolean IsEqualBottom(TOCRRect oOtherRect)
        {
            return ( Math.Abs(Bottom - oOtherRect.Bottom) <= s_iYThreshold );
        }

        /// <summary>
        /// Determines whether [is equal left] [the specified o other rect].
        /// </summary>
        /// <param name="oOtherRect">The o other rect.</param>
        /// <returns></returns>
        public Boolean IsEqualLeft(TOCRRect oOtherRect)
        {
            return ( Math.Abs(m_iLeft - oOtherRect.m_iLeft) <= s_iXThreshold );
        }

        /// <summary>
        /// Determines whether [is equal right] [the specified o other rect].
        /// </summary>
        /// <param name="oOtherRect">The o other rect.</param>
        /// <returns></returns>
        public Boolean IsEqualRight(TOCRRect oOtherRect)
        {
            return ( Math.Abs(Right - oOtherRect.Right) <= s_iXThreshold );
        }

        /// <summary>
        /// return true if the oContainedRect contained this
        /// </summary>
        /// <param name="oContainedRect">The o contained rect.</param>
        /// <returns></returns>
				public Boolean IsInside(TOCRRect oContainedRect)
				{
					return ((m_iLeft + s_iXThreshold >= oContainedRect.Left) &&
									 (Right - s_iXThreshold <= oContainedRect.Right) &&
									 (m_iTop + s_iYThreshold >= oContainedRect.Top) &&
									 (Bottom - s_iYThreshold <= oContainedRect.Bottom));
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
                return m_iLeft;
            }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public int Right
        {
            get
            {
                return m_iLeft + m_iWidth;
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
                return m_iTop;
            }
        }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        public int Bottom
        {
            get
            {
                return m_iTop + m_iHeight;
            }
        }

        // Add a rectangle (Union rectangles)
        /// <summary>
        /// Adds the specified o rect.
        /// </summary>
        /// <param name="oRect">The o rect.</param>
        public void Add(TOCRRect oRect)
        {
            // Do nothing if the add rect is empty
            if ( oRect.IsEmpty() )
                return;

            // Copy the add rect of the target rect is empty
            if ( IsEmpty() )
            {
                m_iLeft = oRect.m_iLeft;
                m_iTop = oRect.m_iTop;
                m_iWidth = oRect.m_iWidth;
                m_iHeight = oRect.m_iHeight;
            }
            else
            {
                // Union rectangles...
                int iNewRight = Math.Max(Right, oRect.Right);
                int iNewBottom = Math.Max(Bottom, oRect.Bottom);
                m_iLeft = Math.Min(m_iLeft, oRect.m_iLeft);
                m_iTop = Math.Min(m_iTop, oRect.m_iTop);
                m_iWidth = iNewRight - m_iLeft;
                m_iHeight = iNewBottom - m_iTop;
            }
        }

        // Set to rectangle to be empty
        /// <summary>
        /// Empties this instance.
        /// </summary>
        public void Empty()
        {
            m_iLeft = 0;
            m_iTop = 0;
            m_iWidth = -1;
            m_iHeight = -1;
        }

        // return true if the 2 rect have overlapping area (including the threshold )
        /// <summary>
        /// Determines whether the specified o rect is overlap.
        /// </summary>
        /// <param name="oRect">The o rect.</param>
        /// <returns></returns>
        public Boolean IsOverlap(TOCRRect oRect)
        {
            if ( ( m_iLeft - s_iXThreshold > oRect.Right ) ||
                ( Right + s_iXThreshold < oRect.m_iLeft ) ||
                ( m_iTop - s_iYThreshold > oRect.Bottom ) ||
                ( Bottom + s_iYThreshold < oRect.m_iTop ) )
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>
        /// The area.
        /// </value>
        public int Area
        {
            get
            {
                return m_iWidth * m_iHeight;
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
                return m_iWidth;
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
                return m_iHeight;
            }
        }

        // Check 2 rect overlapping & return the overlapping data
        //   oRect - (input) The rect to check
        //   oOverlapRect - (output) The overlapping area
        /// <summary>
        /// Overlaps the rect.
        /// </summary>
        /// <param name="oRect">The o rect.</param>
        /// <param name="oOverlapRect">The o overlap rect.</param>
        /// <returns></returns>
        public Boolean OverlapRect(TOCRRect oRect,
            TOCRRect oOverlapRect)
        {
            if ( !IsOverlap(oRect) )
                return false;

            int iNewLeft = Math.Max(m_iLeft, oRect.m_iLeft);
            int iNewTop = Math.Max(m_iTop, oRect.m_iTop);
            oOverlapRect.m_iWidth = Math.Min(Right, oRect.Right) - iNewLeft;
            oOverlapRect.m_iHeight = Math.Min(Bottom, oRect.Bottom) - iNewTop;
            oOverlapRect.m_iLeft = iNewLeft;
            oOverlapRect.m_iTop = iNewTop;

            // In case there is intersect only with the help of the thresholds the we create "small" rectangle in the intersect area
            if ( oOverlapRect.m_iWidth <= 0 )
                oOverlapRect.m_iWidth = 1;
            if ( oOverlapRect.m_iHeight <= 0 )
                oOverlapRect.m_iHeight = 1;

            return true;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TOCRRect"/> to <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator System.Drawing.Rectangle(TOCRRect rect)
        {
            return new System.Drawing.Rectangle(rect.m_iLeft,
                rect.m_iTop,
                rect.m_iWidth,
                rect.m_iHeight);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Drawing.Rectangle"/> to <see cref="TOCRRect"/>.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
				public static implicit operator TOCRRect (System.Drawing.Rectangle rect)
				{
					return new TOCRRect(rect.Left,
							rect.Top,
							rect.Width,
							rect.Height);
				}
			}
   
}
