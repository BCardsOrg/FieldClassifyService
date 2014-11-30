using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

namespace TiS.Core.TisCommon
{
    // SQL helpers
    public class SQL
    {
        public const string NULL_TEXT = "NULL";

        public static string ToSQLText(Guid oVal)
        {
            return @"'" + oVal.ToString() + @"'";
        }

        public static string ToSQLText(bool oVal)
        {
            // Translate bool to 0/1
            if (oVal)
            {
                return "1";
            }

            return "0";
        }

        public static string ToSQLText(DateTime oVal)
        {
            // Example for format... 2004-05-23T14:25:10.087
            string result = String.Format(
                "'{0}-{1}-{2}T{3}:{4}:{5}.{6}'",
                GetNumberSize(oVal.Year, 4),
                GetNumberSize(oVal.Month, 2),
                GetNumberSize(oVal.Day, 2),
                GetNumberSize(oVal.Hour, 2),
                GetNumberSize(oVal.Minute, 2),
                GetNumberSize(oVal.Second, 2),
                GetNumberSize(oVal.Millisecond, 3));

            return result;
        }

        private static string GetNumberSize(int number, int size)
        {
            string numberStr = number.ToString();
            while (numberStr.Length < size)
            {
                numberStr = "0" + numberStr;
            }

            return numberStr;
        }

        public static string ToSQLText(string sVal)
        {
            // Decorate strings with "'"
            return @"'" + sVal.Replace("'", "''") + @"'";
        }

        public static string ToSQLNumeric(object oVal)
        {
            return String.Format(
                EnUSCulture.NumberFormat,
                "{0}",
                oVal);
        }

        public static string ObjectToSQLText(object oVal)
        {
            // By default return as string
            return oVal.ToString();
        }

        public static string ToSQLText(object oVal)
        {
            if (oVal == null)
            {
                return NULL_TEXT;
            }

            if (oVal is Guid)
            {
                return ToSQLText((Guid)oVal);
            }

            TypeCode enTypeCode = Type.GetTypeCode(oVal.GetType());

            switch (enTypeCode)
            {
                case TypeCode.Boolean:
                    return ToSQLText((bool)oVal);
                case TypeCode.DateTime:
                    return ToSQLText((DateTime)oVal);
                case TypeCode.String:
                    return ToSQLText((string)oVal);
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return ToSQLNumeric(oVal);
            }

            return ObjectToSQLText(oVal);
        }

        public static string ToCommaDelimitedString(object[] Values)
        {
            return ToCommaDelimitedString((string[])Values);
        }

        public static string ToCommaDelimitedString(string[] Values)
        {
            return StringUtil.ToCommaDelimitedString(Values);
        }

        public static string ToCommaDelimitedString(IEnumerable Values)
        {
            return StringUtil.ToCommaDelimitedString(Values);
        }

        public static IEnumerable<string> ToSQLValues(IEnumerable oValues)
        {
            foreach (object value in oValues)
            {
                yield return ToSQLText(value);
            }
            //string[] SQLValues = new string[oValues.Count];

            //int i = 0;

            //foreach (object oVal in oValues)
            //{
            //    SQLValues[i] = ToSQLText(oVal);

            //    i++;
            //}

            //return SQLValues;
        }

        public static string ToSQLCommaDelimitedString(IEnumerable oValues)
        {
            return ToCommaDelimitedString(ToSQLValues(oValues));
        }

        public static IEnumerable<string> ToNameValuePairs(string[] Names, object[] Values)
        {
            // Validate the length of the arrays
            if (Names.Length != Values.Length)
            {
                throw new TisException("Names.Length != Values.Length");
            }

            for (int i = 0; i < Names.Length; i++)
            {
                yield return ToNameValuePair(Names[i], Values[i]);
            }
            //int nLen = Names.Length;

            //string[] Entries = new string[nLen];

            //for (int i = 0; i < nLen; i++)
            //{
            //    // Create Name=Value string
            //    Entries[i] = ToNameValuePair(Names[i], Values[i]);
            //}

            //return Entries;
        }

        public static string ToNameValuePair(string sName, object oVal)
        {
            return sName + "=" + ToSQLText(oVal);
        }

        public static IEnumerable<string> ToNameValuePairs(IEnumerable oNames, IEnumerable oValues)
        {
            //List<string> Entries = new List<string>();

            IEnumerator oNamesEnum = oNames.GetEnumerator();
            IEnumerator oValuesEnum = oValues.GetEnumerator();

            while (oNamesEnum.MoveNext() && oValuesEnum.MoveNext())
            {
                string sName = oNamesEnum.Current.ToString();
                object oVal = oValuesEnum.Current;

                yield return ToNameValuePair(sName, oVal);

                //Entries.Add(ToNameValuePair(sName, oVal));
            }

            //return Entries.ToArray();
        }

        public static IEnumerable<string> ToNameValuePairs(IDictionary oMap)
        {
            return ToNameValuePairs(oMap.Keys, oMap.Values);
        }

        public static string ToCommaDelimitedNameValuePairs(IDictionary oMap)
        {
            return ToCommaDelimitedString(ToNameValuePairs(oMap.Keys, oMap.Values));
        }

        public static string ToCommaDelimitedNameValuePairs(IDictionary<string, object> oMap)
        {
            return ToCommaDelimitedString(ToNameValuePairs(oMap.Keys, oMap.Values));
        }


        //public static string[] ToNameValuePairs(IDictionary<string, object> oMap)
        //{
        //    return ToNameValuePairs(oMap.Keys, oMap.Values);
        //}

        //public static string ToCommaDelimitedNameValuePairs(IDictionary<string, object> oMap)
        //{
        //    return ToCommaDelimitedString(ToNameValuePairs(oMap));
        //}
        //
        //	Private
        //


        private static readonly CultureInfo EnUSCulture =
            new CultureInfo("en-US");
    }
}
