using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace TiS.Core.TisCommon
{
    public class IniFile
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern int WritePrivateProfileString(string psSection, string psKey, string psValue, string psFile);
        [DllImport("kernel32", SetLastError = true)]
        private static extern int GetPrivateProfileString(string psSection, string psKey, string psDefault, byte[] psrReturn, int piBufferLen, string psFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetPrivateProfileInt(string psAppName,
                                                      string psKeyName,
                                                      int piDefault,
                                                      string psFile);

        private string m_iniFilename;
        private int m_bufferLength = 65000;

        public IniFile(string iniFilename)
        {
            m_iniFilename = iniFilename;
        }

        /// <summary>
        /// INI Path and Filename
        /// </summary>
        public string IniFileName
        {
            get { return m_iniFilename; }
            set { m_iniFilename = value; }
        }

        /// <summary>
        /// Max return length when reading data
        /// </summary>
        public int BufferLen
        {
            get { return m_bufferLength; }
            set { m_bufferLength = value; }
        }

        /// <summary>
        /// Read value from INI File
        /// </summary>
        public string ReadValue(string psSection, string psKey, string psDefault)
        {
            byte[] sGetBuffer = new byte[this.m_bufferLength];
            //UnicodeEncoding oAscii = new UnicodeEncoding();
            //ASCIIEncoding oAscii = new ASCIIEncoding();
            Encoding oAscii = Encoding.GetEncoding(1252);
            int i = GetPrivateProfileString(psSection, psKey, psDefault, sGetBuffer, this.m_bufferLength, this.m_iniFilename);
            return oAscii.GetString(sGetBuffer, 0, i);
            //return string(sGetBuffer,i) ;
        }

        public int ReadInteger(string psSection,
                               string psKey,
                               int piDefault)
        {
            return GetPrivateProfileInt(psSection, psKey, piDefault, this.m_iniFilename);
        }

        /// <summary>
        /// Write value to INI File
        /// </summary>
        public void WriteValue(string psSection, string psKey, string psValue)
        {
            WritePrivateProfileString(psSection, psKey, psValue, this.m_iniFilename);
        }

        /// <summary>
        /// Remove value from INI File
        /// </summary>
        public void RemoveValue(string psSection, string psKey)
        {
            WritePrivateProfileString(psSection, psKey, null, this.m_iniFilename);
        }

        /// <summary>
        /// Read values in a section from INI File
        /// </summary>
        public void ReadValues(string psSection, out Array poValues)
        {
            byte[] sGetBuffer = new byte[this.m_bufferLength];
            int i = GetPrivateProfileString(psSection, null, null, sGetBuffer, this.m_bufferLength, this.m_iniFilename);
            if (i != 0)
            {
                ASCIIEncoding oAscii = new ASCIIEncoding();
                poValues = oAscii.GetString(sGetBuffer, 0, i - 1).Split((char)0);
            }
			else
				poValues = null;
        }

        /// <summary>
        /// Read values in a section from INI File
        /// </summary>
        public void ReadValues(string psSection, out Dictionary<string, string> poValues)
		{
			poValues = new Dictionary<string, string>();

			Array values;
			ReadValues(psSection, out values);
			if (values == null)
				return;

			foreach (object obj in values)
			{
				string key = obj.ToString();
				string value = ReadValue(psSection, key, string.Empty);
				poValues.Add(key, value);
			}
		}

			/// <summary>
        /// Read sections from INI File
        /// </summary>
        public void ReadSections(ref string[] poSections)
        {
            byte[] sGetBuffer = new byte[this.m_bufferLength];
            int i = GetPrivateProfileString(null, null, null, sGetBuffer, this.m_bufferLength, this.m_iniFilename);
			if (i != 0)
			{
				ASCIIEncoding oAscii = new ASCIIEncoding();
				poSections = oAscii.GetString(sGetBuffer, 0, i - 1).Split((char)0);
			}
        }

        /// <summary>
        /// Remove section from INI File
        /// </summary>
        public void RemoveSection(string psSection)
        {
            WritePrivateProfileString(psSection, null, null, this.m_iniFilename);
        }
    }
}
