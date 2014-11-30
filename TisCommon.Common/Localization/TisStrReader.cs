using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TiS.Core.TisCommon.Configuration;
using System.Globalization;
using System.Text;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Localization
{
    internal enum TIS_STR_STATUS
    { 
        STR_ERROR_NO_ERROR = 0, 
        STR_ERROR_NOT_INITIALIZED = 0x1001,
        STR_ERROR_MISSING_TOKEN = 0x1002,
        STR_ERROR_MISSING_FILENAME = 0x1003,
    };

    #region TisStrReader

    internal class TisStrReader
    {
        private const string LANGUAGE_ENV = "eFlowLang";
        private const string LANGUAGE_DIR = @"Language\";
        private const char DELIMITER = ',';
        private const char COMMENT = ';';
        private const string STRING_NOT_FOUND = "***" ;

        private string m_STRPath;
        private bool m_ignoreLoadErr;
        private string m_EflowLangPath;

        private SortedDictionary<string, SortedDictionary<string, string>> m_strFilesList; 

        private object m_locker = new object();

        public TisStrReader()
        {
            BasicConfiguration basicConfiguration = new BasicConfiguration();
            m_EflowLangPath = basicConfiguration.eFlowLangPath;

            m_strFilesList = new SortedDictionary<string, SortedDictionary<string, string>>();
        }

        public TisStrReader(
            string relativeLanguageFilename,
            string alternativeLanguagePath) : this()
        {
            AddLanguageFile(relativeLanguageFilename);

            STRPath = alternativeLanguagePath;
        }

        public string STRPath
        {
            get
            {
                return m_STRPath;
            }
            set
            {
                lock (m_locker)
                {
                    if (value != null)
                    {
                        m_STRPath = value;
                    }
                }
            }
        }

        public bool IgnoreLoadErr
        {
            get
            {
                return m_ignoreLoadErr;
            }
            set
            {
                lock (m_locker)
                {
                    m_ignoreLoadErr = value;
                }
            }
        }

        public void Clear()
        {
            lock (m_locker)
            {
                m_strFilesList.Clear();
            }
        }

        public bool IsLanguageFileLoaded(string relativeLanguageFilename)
        {
            bool result = false;
            lock (m_locker)
            {
                if (m_strFilesList.ContainsKey(relativeLanguageFilename))
                {
                    result = true;
                }
            }

            return result;
        }

        public TIS_STR_STATUS AddLanguageFile(string relativeLanguageFilename)
        {
            if (String.IsNullOrEmpty(relativeLanguageFilename))
            {
                return TIS_STR_STATUS.STR_ERROR_MISSING_FILENAME;
            }

            lock (m_locker)
            {
                if (!m_strFilesList.ContainsKey(relativeLanguageFilename))
                {
                    m_strFilesList.Add(relativeLanguageFilename, new SortedDictionary<string, string>());
                }
            }

            return TIS_STR_STATUS.STR_ERROR_NO_ERROR;
        }

        public TIS_STR_STATUS Load()
        {
            string strFileName;
            SortedDictionary<string, string> strFileContents;
            List<string> badFiles = new List<string>();

            lock (m_locker)
            {
                foreach (KeyValuePair<string, SortedDictionary<string, string>> kvp in m_strFilesList)
                {
                    strFileName = kvp.Key;
                    strFileContents = kvp.Value;

                    try
                    {
                        if (strFileContents.Count == 0)
                        {
                            LoadFile(strFileName, strFileContents);
                        }
                    }
                    catch (Exception e)
                    {
                        badFiles.Add(strFileName);

                        if (!m_ignoreLoadErr)
                        {
                            MessageBox.Show(
                                "Error loading [" + strFileName + "]. Details : " + e.Message,
                                "TisSTR",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
                        }
                    }
                }

                foreach (string badFile in badFiles)
                {
                    m_strFilesList.Remove(badFile);
                }
            }

            return TIS_STR_STATUS.STR_ERROR_NO_ERROR;
        }

        public TIS_STR_STATUS GetLocalizedStringByToken(
            string tokenName,
            ref string localizedString)
        {
            if (String.IsNullOrEmpty(tokenName))
            {
                return TIS_STR_STATUS.STR_ERROR_MISSING_TOKEN;
            }

            foreach (SortedDictionary<string, string> strFileContents in m_strFilesList.Values)
            {
                if (strFileContents.TryGetValue(tokenName, out localizedString))
                {
                    return TIS_STR_STATUS.STR_ERROR_NO_ERROR;
                }
            }

            localizedString = STRING_NOT_FOUND;

            return TIS_STR_STATUS.STR_ERROR_NO_ERROR;
        }


        public TIS_STR_STATUS GetLocalizedStringByTokenDef(
            string tokenName,
            ref string localizedString,
            string DefaultString)
        {
            TIS_STR_STATUS status =
                GetLocalizedStringByToken(tokenName, ref localizedString);

            if (status == TIS_STR_STATUS.STR_ERROR_NO_ERROR)
            {
                if (localizedString == STRING_NOT_FOUND && !String.IsNullOrEmpty(DefaultString))
                {
                    localizedString = DefaultString;
                }
            }

            return status;
        }


        private void LoadFile(
            string strFileName, 
            SortedDictionary<string, string> strFileContents)
        {
            strFileContents.Clear();

            string strFilePath = LocateFile(strFileName);

			Encoding enc = Encoding.GetEncoding(CultureInfo.CurrentUICulture.TextInfo.ANSICodePage);
			List<string> rawFileContents =
				new List<string>(File.ReadAllLines(strFilePath, enc));
			
            char[] delimiter = new char[] { DELIMITER };

            foreach (string line in rawFileContents)
            {
                if (!String.IsNullOrEmpty(line) && line[0] != COMMENT)
                {
                    string[] parts =
                        line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 1)
                    {
                        string token = parts[0];
                        string value = parts[1];

                        if (!strFileContents.ContainsKey(token))
                        {
                            strFileContents.Add(token, value);
                        }
                        else
                        {
                            strFileContents[token] = value;
                        }
                    }
                }
            }
        }

        private string LocateFile(string strFileName)
        {
            string strFilePath = String.Empty;

            try
            {
                // Normal client
                if (System.Diagnostics.Process.GetCurrentProcess().ProcessName != TisServicesConst.IIS_PROCESS_NAME)
                {
                    strFilePath = Path.Combine(m_EflowLangPath, strFileName);
                    if (File.Exists(strFilePath))
                    {
                        return strFilePath;
                    }

                    string environmentVariable = Environment.GetEnvironmentVariable(LANGUAGE_ENV);
                    if (StringUtil.IsStringInitialized(environmentVariable))
                    {
                        strFilePath = Path.Combine(Environment.GetEnvironmentVariable(LANGUAGE_ENV), strFilePath);
                    }

                    if (File.Exists(strFilePath))
                    {
                        return strFilePath;
                    }

                    string currentDirectory = Directory.GetCurrentDirectory();

                    strFilePath = Path.Combine(currentDirectory, strFileName);

                    if (File.Exists(strFilePath))
                    {
                        return strFilePath;
                    }

                    strFilePath =
                        Path.Combine(Path.Combine(currentDirectory, LANGUAGE_DIR), strFileName);

                    if (File.Exists(strFilePath))
                    {
                        return strFilePath;
                    }

                    strFilePath =
                        Path.Combine(Path.Combine(currentDirectory, @"..\" + LANGUAGE_DIR), strFileName);

                    if (File.Exists(strFilePath))
                    {
                        return strFilePath;
                    }

                    strFilePath =
                        Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), strFileName);

                    if (File.Exists(strFilePath))
                    {
                        return strFilePath;
                    }

                    if (String.IsNullOrEmpty(m_STRPath) == false)
                    {
                        strFilePath = Path.Combine(m_STRPath, strFileName);

                        if (File.Exists(strFilePath))
                        {
                            return strFilePath;
                        }
                    }
                }
                // Web server as a client
                else
                {
                    try
                    {
                        strFilePath = (string)AppDomain.CurrentDomain.GetData("APPBASE");

                        if (StringUtil.IsStringInitialized(strFilePath))
                        {
                            strFilePath = Path.Combine(Path.Combine(strFilePath, LANGUAGE_DIR), strFileName);

                            if (File.Exists(strFilePath))
                            {
                                return strFilePath;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                return strFilePath;
            }
            finally
            {
                if (String.IsNullOrEmpty(m_STRPath))
                {
                    m_STRPath = Path.GetDirectoryName(strFilePath);
                }
            }
        }
    }

    #endregion
}
