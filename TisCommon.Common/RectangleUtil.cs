using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TiS.Core.TisCommon
{
    public static class RectangleUtil
    {
        /// <summary>
        /// Add rectangle
        /// </summary>
        /// <param name="targetRec">The target rec.</param>
        /// <param name="newRec">The new rec.</param>
        public static Rectangle Add(this Rectangle targetRec, Rectangle newRec)
        {
            if (targetRec.IsEmpty)
            {
                return newRec;
            }
            else
            {
                return
                    Rectangle.FromLTRB(
                        Math.Min(newRec.Left, targetRec.Left),
                        Math.Min(newRec.Top, targetRec.Top),
                        Math.Max(newRec.Right, targetRec.Right),
                        Math.Max(newRec.Bottom, targetRec.Bottom));
            }
        }

		/// <summary>
		/// Check if a point is inside a the rectangle.
		/// </summary>
		/// <param name="rec">The rec.</param>
		/// <param name="point">The point.</param>
		/// <returns></returns>
		public static bool IntersectsWith(this Rectangle rec, Point point )
		{
			return
				point.X >= rec.Left &&
				point.X <= rec.Right &&
				point.Y >= rec.Top &&
				point.Y <= rec.Bottom;
		}

		/// <summary>
		/// Gets the mid point of the rectangle.
		/// </summary>
		/// <param name="rec">The rec.</param>
		/// <returns></returns>
		public static Point GetMidPoint(this Rectangle rec)
		{
			if (rec.IsEmpty == true)
			{
				return Point.Empty;
			}
			else
			{
				return new Point(rec.Left + rec.Width / 2, rec.Top + rec.Height / 2);
			}
		}
    }
}
