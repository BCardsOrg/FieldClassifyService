using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.IO;
using System.Globalization;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.Localization
{
    public class TextResourceManager : ResourceManager
    {
        private string m_DirectoryName;
        private string m_DefaultExtention = ".str";
        private Dictionary<string, string> m_CacheStrings = new Dictionary<string, string>();       

        public string DefaultExtention
        {
            get { return m_DefaultExtention; }
            set { m_DefaultExtention = value; }
        }

        public TextResourceManager(string dirName, string baseFileName)
        {
            m_DirectoryName = dirName;
            this.BaseNameField = baseFileName;
			
            ResourceSets = new System.Collections.Hashtable();            
        }

        public TextResourceManager(string baseFileName)
        {
            string RelativeLanguageFilename = baseFileName + DefaultExtention ;
            InitTisStr(RelativeLanguageFilename);
        }      

        static public void InitTisStr(string RelativeLanguageFilename)
        {
            try
            {
                short nStatus = TisStr.Initialize(null);

                if (nStatus != TisStr.STR_ERROR_NO_ERROR)
                {
                    Log.Write(
                        Log.Severity.ERROR,
                        System.Reflection.MethodInfo.GetCurrentMethod(),
                        "Error : {0}",
                        nStatus.ToString());

                    throw new TisException("{0} - {1}",System.Reflection.MethodInfo.GetCurrentMethod(),"Error - TisStr.Initialize");
                }

                nStatus = TisStr.AddLanguageFile(RelativeLanguageFilename);

                if (nStatus != TisStr.STR_ERROR_NO_ERROR)
                {
                    Log.Write(
                        Log.Severity.ERROR,
                        System.Reflection.MethodInfo.GetCurrentMethod(),
                        "Error : {0}",
                        nStatus.ToString()); ;

                    throw new TisException("{0} - {1}", System.Reflection.MethodInfo.GetCurrentMethod(), "Error - TisStr.AddLanguageFile");
                }

                nStatus = TisStr.Load();

                if (nStatus != TisStr.STR_ERROR_NO_ERROR)
                {
                    Log.Write(
                        Log.Severity.ERROR,
                        System.Reflection.MethodInfo.GetCurrentMethod(),
                        "Error : {0}",
                        nStatus.ToString());

                    throw new TisException("{0} - {1}", System.Reflection.MethodInfo.GetCurrentMethod(), "Error - TisStr.Load");
                }
            }
            catch (Exception oExc)
            {
                Log.Write(
                    Log.Severity.ERROR,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "Error : {0}",
                    oExc);

                throw new TisException("{0} - {1}", System.Reflection.MethodInfo.GetCurrentMethod(), oExc.Message);
            }
            
        }

        public override string GetString(string name, CultureInfo culture)
        {           
            return GetString(name);
        }

        public override string GetString(string name)
        {
            string value = String.Empty;
            if (!m_CacheStrings.TryGetValue(name, out value))
            {
                value = TisStr.LoadText(name);
                m_CacheStrings[name] = value;
            }
            return value;
        }

        public static bool CanLoadResource(string dirName, string baseFileName)
        {
            if (Directory.Exists(dirName) == false)
                return false;
            string [] lang = Directory.GetFiles(dirName, baseFileName + "*", SearchOption.TopDirectoryOnly);
            return lang.Length > 0;
        }
        public TextResourceManager(string dirName, string baseFileName,string defaultExtension):this(dirName,baseFileName)
        {           
            m_DefaultExtention = defaultExtension;
        }


         
    }
}
