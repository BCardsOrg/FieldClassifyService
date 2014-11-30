using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon
{
    [ComVisible(false)]
    public class CommandLine
    {
        private static readonly string[] DEFAULT_PREFIXES = new string[] { "", "/", "-" };
        private const string DEFAULT_NAME_VAL_SEP = ":";

        private CommandLine() { }

        public static bool SwitchFound(string sSwitch)
        {
            return SwitchFound(DEFAULT_PREFIXES, sSwitch);
        }

        public static bool SwitchFound(
            string[] ValidPrefixes,
            string sSwitch)
        {
            return GetParamValue(ValidPrefixes, "", sSwitch) != null;
        }

        public static string GetParamValue(
            string sParam)
        {
            return GetParamValue(
                DEFAULT_PREFIXES,
                DEFAULT_NAME_VAL_SEP,
                sParam);
        }

        public static string GetParamValue(
            string[] ValidPrefixes,
            string sNameValueSeparator,
            string sParam)
        {
            string sArg = GetParamStartingWith(ValidPrefixes, sParam + sNameValueSeparator);

            if (sArg != null)
            {
                int nSepPos = sArg.IndexOf(sNameValueSeparator);

                return sArg.Substring(nSepPos + 1);
            }

            return null;
        }

        private static string GetParamStartingWith(
            string[] ValidPrefixes,
            string sStartVal)
        {
            string[] Args = Environment.GetCommandLineArgs();

            foreach (string sArg in Args)
            {
                foreach (string sPrefix in ValidPrefixes)
                {
                    if (sArg.StartsWith(sPrefix + sStartVal))
                    {
                        return sArg;
                    }
                }

            }

            return null;
        }

    }
}
