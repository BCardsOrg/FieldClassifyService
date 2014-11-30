using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.Localization
{
	/// <summary>
	/// Wrapper class for TisStrWrap.dll
	/// Used to retreive localized strings from str files
	/// </summary>
	[ComVisible(false)]
	public class TisStr
	{
        public const short STR_ERROR_NO_ERROR = (short)TIS_STR_STATUS.STR_ERROR_NO_ERROR;
        public const short STR_ERROR_NOT_INITIALIZED = (short)TIS_STR_STATUS.STR_ERROR_NOT_INITIALIZED;
        public const short STR_ERROR_MISSING_TOKEN = (short)TIS_STR_STATUS.STR_ERROR_MISSING_TOKEN;
        public const short STR_ERROR_MISSING_FILENAME = (short)TIS_STR_STATUS.STR_ERROR_MISSING_FILENAME;

        private static TisStrReader m_strReader;

        public static bool ShowErrorOnLoad
        {
            get
            {
                EnsureStrReader();

                return !m_strReader.IgnoreLoadErr;
            }
            set
            {
                EnsureStrReader();

                m_strReader.IgnoreLoadErr = !value;
            }
        }

        public static bool Init(
            string sRelativeLanguageFilename,
            string sAlternativeLanguagePath)
        {
            bool bOK = 
                Initialize(sAlternativeLanguagePath) == (short)TIS_STR_STATUS.STR_ERROR_NO_ERROR &&
                AddLanguageFile(sRelativeLanguageFilename) == (short)TIS_STR_STATUS.STR_ERROR_NO_ERROR &&
                Load() == (short)TIS_STR_STATUS.STR_ERROR_NO_ERROR;

            if (!bOK)
            {
                Log.WriteWarning("Failed to initialize localization.");
            }

            return bOK;
        }

        public static short Initialize(string AlternativeLanguagePath)
        {
            EnsureStrReader();

            m_strReader.STRPath = AlternativeLanguagePath;

            return (short)TIS_STR_STATUS.STR_ERROR_NO_ERROR;
        }

        public static void Clear()
        {
            m_strReader.Clear();
        }

        public static bool IsLanguageFileLoaded(string RelativeLanguageFilename)
        {
            EnsureStrReader();

            bool result = m_strReader.IsLanguageFileLoaded(RelativeLanguageFilename);

            return result;
        }

        public static short AddLanguageFile(string RelativeLanguageFilename)
        {
            EnsureStrReader();

            TIS_STR_STATUS status = m_strReader.AddLanguageFile(RelativeLanguageFilename);

            return (short)status;
        }

        public static short Load()
        {
            EnsureStrReader();

            TIS_STR_STATUS status = m_strReader.Load();

            return (short)status;
        }

		public static string LoadText( string sTokenName)
		{
			if(sTokenName == null)
				sTokenName = "";

			return LoadText(sTokenName, "*"+sTokenName);
		}

		public static string LoadText( string sTokenName, string sDefaultString)
		{
			string errorMsg = "Failed to get localized string for '"+sTokenName+"'.";
			try 
			{
                EnsureStrReader();

                string localizedString = null;

                TIS_STR_STATUS status = 
                    m_strReader.GetLocalizedStringByTokenDef(sTokenName, ref localizedString, sDefaultString);

                if (status != TIS_STR_STATUS.STR_ERROR_NO_ERROR)
				{
					switch(status)
					{
                        case TIS_STR_STATUS.STR_ERROR_NOT_INITIALIZED:
						    Log.WriteError("Localization was not initialized. "+errorMsg);
							break;
                        case TIS_STR_STATUS.STR_ERROR_MISSING_TOKEN:
							Log.WriteError("Missing token '"+sTokenName+"'. "+errorMsg);
							break;
                        case TIS_STR_STATUS.STR_ERROR_MISSING_FILENAME:
							Log.WriteError("Missing localization file name. "+errorMsg);
							break;
					}

					return sDefaultString;
				}
				else
					return localizedString;
				
			}
			catch(Exception e)
			{
				Log.WriteError(errorMsg+" : "+e.Message);
				return sDefaultString;
			}
		}


        private static void EnsureStrReader()
        {
            if (m_strReader == null)
            {
                m_strReader = new TisStrReader();
            }
        }
    }
}
