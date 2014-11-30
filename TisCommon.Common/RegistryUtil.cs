using System;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon
{
    public class RegistryUtil
    {
        private const string REG_CONFIG_FILES_ENTRY = "SOFTWARE\\TopImageSystems\\eFLOW 5";
        public static string Version
        {
            get
            {
                return RegistryWrapper.RegKeyValue(REG_CONFIG_FILES_ENTRY, "Version") ?? string.Empty;
            }
        }
        public static string eFlowPath
        {
            get
            {
                return RegistryWrapper.RegKeyValue(REG_CONFIG_FILES_ENTRY, "eFlowPath") ?? string.Empty;
            }
        }
    }
    public class RegistryWrapper
    {
        public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
        public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);

        // http://msdn.microsoft.com/en-us/library/ms724878(VS.85).aspx
        public static int KEY_READ = (int)unchecked(0x00020019);

        public static int KEY_WOW64_64KEY = (int)unchecked(0x0000100);
        public static int KEY_WOW64_32KEY = (int)unchecked(0x0000200);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RegOpenKeyEx(
            [In] UIntPtr hKey,
            [In] string subKey,
                 int ulOptions,
            [In] int samDesired,
            [Out] out UIntPtr hkResult);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        public static extern int RegQueryValueEx(
            [In] UIntPtr hKey,
            [In] string lpValueName,
                 IntPtr lpReserved,
            [Out] out uint lpType,
            [Out] StringBuilder lpData,
            [In, Out] ref uint lpcbData);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(
            [In] UIntPtr hKey);

        public static bool RegKeyExists(string key)
        {
            UIntPtr keyHandle = UIntPtr.Zero;
            int result = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, KEY_READ | KEY_WOW64_32KEY, out keyHandle);

            if (result == 0)
                RegCloseKey(keyHandle);

            return result == 0;
        }

        public static string RegKeyValue(string key, string value)
        {
            UIntPtr keyHandle = UIntPtr.Zero;

            int result = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, KEY_READ | KEY_WOW64_32KEY, out keyHandle);

            if (result == 0)
            {
                uint size = 1024;
                uint type;
                string keyValue = null;
                StringBuilder keyBuffer = new StringBuilder(2048);

                if (RegQueryValueEx(keyHandle, value, IntPtr.Zero, out type, keyBuffer, ref size) == 0)
                {
                    keyValue = keyBuffer.ToString();

                    RegCloseKey(keyHandle);
                }

                return keyValue;
            }

            return null;
        }
    }
}
