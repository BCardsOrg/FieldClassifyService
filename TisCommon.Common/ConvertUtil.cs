using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TiS.Core.TisCommon
{
    public static class ConvertUtil
    {
        static string m_universalSortableDateTimePatternWithMillisecond;
        static ConvertUtil()
        {
            m_universalSortableDateTimePatternWithMillisecond = CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern.Replace("'ss'", "'ss'.'fff'");
        }

        public static Type TypeFromTypeCode(TypeCode enTypeCode)
        {
            switch (enTypeCode)
            {
                case TypeCode.Empty:
                    throw new TisException("Invalid data type");

                case TypeCode.Object:
                    return typeof(object);

                case TypeCode.DBNull:
                    // Throw a SystemException for unsupported data types.
                    throw new SystemException("Invalid data type");

                case TypeCode.Char:
                    return typeof(char);

                case TypeCode.SByte:
                    return typeof(sbyte);

                case TypeCode.UInt16:
                    return typeof(System.UInt16);

                case TypeCode.UInt32:
                    return typeof(System.UInt32);

                case TypeCode.UInt64:
                    return typeof(System.UInt64);

                case TypeCode.Boolean:
                    return typeof(System.Boolean);

                case TypeCode.Byte:
                    return typeof(System.Byte);

                case TypeCode.Int16:
                    return typeof(System.Int16);

                case TypeCode.Int32:
                    return typeof(System.Int32);

                case TypeCode.Int64:
                    return typeof(System.Int64);

                case TypeCode.Single:
                    return typeof(System.Single);

                case TypeCode.Double:
                    return typeof(System.Double);

                case TypeCode.Decimal:
                    return typeof(System.Decimal);

                case TypeCode.DateTime:
                    return typeof(System.DateTime);

                case TypeCode.String:
                    return typeof(System.String);

                default:
                    throw new TisException("Value is of unknown data type");
            }
        }

		/// <summary>
		/// Convert string to DateTime in local zone (in case the input format is UniversalSortableDateTimePattern)
		/// </summary>
		/// <param name="dateTimeString">The date time string.</param>
		/// <returns></returns>
		public static DateTime ToLocalTime(string dateTimeString)
		{
			return DateTime.Parse(dateTimeString);
		}

		/// <summary>
		/// Convert DateTime to DateTime in local zone
		/// </summary>
		/// <param name="dateTimeString">The date time.</param>
		/// <returns></returns>
		public static DateTime ToLocalTime(DateTime dateTime)
		{
			return dateTime.ToLocalTime();
		}

		/// <summary>
		/// Convert string to DateTime in universal zone (GMT 0)
		/// </summary>
		/// <param name="dateTimeString">The date time string.</param>
		/// <returns></returns>
		public static DateTime ToUniversalTime(string dateTimeString)
		{
			return DateTime.Parse(dateTimeString).ToUniversalTime();
		}

		/// <summary>
		/// Convert DateTime to DateTime in universal zone (GMT 0)
		/// </summary>
		/// <param name="dateTimeString">The date time.</param>
		/// <returns></returns>
		public static DateTime ToUniversalTime(DateTime dateTime)
		{
			return dateTime.ToUniversalTime();
		}

		/// <summary>
		/// Convert DateTime to universal string.
		/// </summary>
		/// <param name="dateTime">The date time.</param>
		/// <returns></returns>
		public static string ToUniversalString(DateTime dateTime)
		{
            return dateTime.ToUniversalTime().ToString(m_universalSortableDateTimePatternWithMillisecond);
		}

	}
}
