using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace TiS.Core.TisCommon
{
	public class StringUtil
	{
		private const string NULL_STRING = "null";

		public static bool IsStringInitialized(string sVal)
		{
			if(sVal == null)
			{
				return false;
			}

			if(sVal.Length == 0)
			{
				return false;
			}

			return true;
		}

		public static string[] ToStringArray(object[] ObjArray)
		{
			return (string[])ArrayBuilder.ChangeArrayType(ObjArray, typeof(string));
		}

		public static string ToCommaDelimitedString(string[] Values)
		{
            return ToCommaDelimitedString((IEnumerable)Values);
		}

        public static string ToCommaDelimitedString(IEnumerable oValues)
		{
			StringBuilder oText = new StringBuilder();
			
			bool bFirst = true;

			foreach(object oVal in oValues)
			{
				if(!bFirst)
				{
					oText.Append(",");
				}
				else
				{
					bFirst = false;
				}

				if(oVal == null)
				{
					oText.Append(NULL_STRING);
				}
				else
				{
					oText.Append(oVal.ToString());
				}
			}

			return oText.ToString();
		}
		
		public static string[] SplitCommaDelimitedString(string sValue)
		{
			return sValue.Split(',');
		}
	
		public static bool CompareIgnoreCase(string s1, string s2)
		{
			return String.Compare(
				s1, 
				s2, 
				true	// Ignore case
				) == 0;
		}

		public static string NullToEmpty(string s)
		{
            return s ?? String.Empty;
		}

		public static bool ArrayContains(string[] StringArray, string sVal)
		{
			return Array.IndexOf(StringArray, sVal) >= 0;
		}

		public static bool ArrayContainsIgnoreCase(ICollection oStrings, string sVal)
		{
			foreach(string sStr in oStrings)
			{
				if(CompareIgnoreCase(sStr, sVal))
				{
					return true;
				}
			}

			return false;
		}

        public static string ArrayGetIgnoreCase(ICollection oStrings, string sVal)
        {
            foreach (string sStr in oStrings)
            {
                if (CompareIgnoreCase(sStr, sVal))
                {
                    return sStr;
                }
            }

            return null;
        }

        public static bool ArrayContainsAny(
			string[] StringArray, 
			string[] Searched)
		{
			foreach(string sEntry in Searched)
			{
				if(ArrayContains(StringArray, sEntry))
				{
					return true;
				}
			}

			return false;
		}
		
		public static string[] GetDifference(string[] Set1, string[] Set2)
		{
			return (string[])ArrayBuilder.ChangeArrayType(
				SetUtil<string>.GetDifference(Set1, Set2),
				typeof(string));
		}

		public static void GetDifference(
			string[] PrevSet, 
			string[] NewSet,
			out string[] ItemsAdded,
			out string[] ItemsRemoved)
		{
			ItemsRemoved = GetDifference(
				NewSet, 
				PrevSet);

			ItemsAdded = GetDifference(
				PrevSet, 
				NewSet);
		}

		public static string[] GetUnion(string[] set1, string[] set2)
		{
            IDictionary<string, string> result = new Dictionary<string, string>();

            // Add objects from set 1
            foreach (string str in set1)
            {
                if (!result.ContainsKey(str))
                {
                    result.Add(str, str);
                }
            }

            // Add objects from set 2
            foreach (string str in set2)
            {
                if (!result.ContainsKey(str))
                {
                    result.Add(str, str);
                }
            }

            string[] resultArray = new string[result.Keys.Count];
            result.Keys.CopyTo(resultArray, 0);
            return resultArray;
        }

		public static string[] GetIntersection(string[] Set1, string[] Set2)
		{
			return (string[])ArrayBuilder.ChangeArrayType(
				SetUtil<string>.GetIntersection(Set1, Set2),
				typeof(string));
		}

		public static byte[] ToBytesASCII(string sValue)
		{
			return System.Text.ASCIIEncoding.UTF8.GetBytes(sValue);
		}

		public static string FromBytesASCII(byte[] Data)
		{
            return System.Text.ASCIIEncoding.UTF8.GetString(Data);
		}

        public static string FromBytesHex(byte[] Data)
        {
            string hexString = String.Empty;

            for (int i = 0; i < Data.Length; i++)
            {
                hexString += Data[i].ToString("X2");
            }

            return hexString;
        }
    }
}
