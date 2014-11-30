using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{

    public class IniParser
    {
        private Dictionary<string, Dictionary<string, string>> keyPairs = new Dictionary<string, Dictionary<string, string>>();
        private string iniFilePath;



        /// <summary>
        /// Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name="iniPath">Full path to INI file.</param>
        public IniParser(string iniPath)
        {
            TextReader iniFile = null;
            string strLine = null;
            string currentRoot = null;
            string[] keyPair = null;

            iniFilePath = iniPath;

            if (File.Exists(iniPath))
            {
                try
                {
                    iniFile = new StreamReader(iniPath,true);

                    strLine = iniFile.ReadLine();

                    while (strLine != null)
                    {


                        if (strLine != "")
                        {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                            {
                                currentRoot = strLine.Substring(1, strLine.Length - 2);
                            }
                            else
                            {

                                keyPair = strLine.Split(new char[] { '=' }, 2);

                                string value = null;
                                if (currentRoot == null)
                                    currentRoot = "ROOT";
                                Dictionary<string, string> currentVal;
                                if (keyPairs.TryGetValue(currentRoot, out currentVal) == false)
                                    keyPairs.Add(currentRoot, currentVal = new Dictionary<string, string>());


                                if (keyPair.Length > 1)
                                    value = keyPair[1];

                                currentVal.Add(keyPair[0], value);
                            }
                        }

                        strLine = iniFile.ReadLine();
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (iniFile != null)
                        iniFile.Close();
                }
            }
            else
                throw new FileNotFoundException("Unable to locate " + iniPath);

        }

        /// <summary>
        /// Returns the value for the given section, key pair.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="settingName">Key name.</param>
        public string GetSetting(string sectionName, string settingName)
        {

            return keyPairs[sectionName][settingName];
        }

        /// <summary>
        /// Enumerates all lines for given section.
        /// </summary>
        /// <param name="sectionName">Section to enum.</param>
        public IEnumerable<string> EnumSection(string sectionName)
        {
            if (keyPairs.ContainsKey(sectionName) == true)
            {
                return (from x in keyPairs[sectionName].Keys select x);
            }
            else
            {
                return new List<string>();
            }

        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        /// <param name="settingValue">Value of key.</param>
        public void AddSetting(string sectionName, string settingName, string settingValue)
        {
            Dictionary<string, string> val;
            if (keyPairs.TryGetValue(sectionName, out val) == false)
                keyPairs[sectionName] = val = new Dictionary<string, string>();

            val.Add(settingName, settingValue);
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved with a null value.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void AddSetting(string sectionName, string settingName)
        {
            AddSetting(sectionName, settingName, null);
        }

        /// <summary>
        /// Remove a setting.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void DeleteSetting(string sectionName, string settingName)
        {
            Dictionary<string, string> val;
            if (keyPairs.TryGetValue(sectionName, out val) == false)
                keyPairs[sectionName] = val = new Dictionary<string, string>();

            val.Remove(settingName);
        }

        /// <summary>
        /// Save settings to new file.
        /// </summary>
        /// <param name="newFilePath">New file path.</param>
        public void SaveSettings(string newFilePath)
        {

            string tmpValue = "";
            string strToSave = "";




            foreach (var sectionPair in keyPairs.Keys)
            {
                strToSave += ("[" + sectionPair + "]\r\n");

                foreach (var vals in keyPairs[sectionPair].Keys)
                {

                    tmpValue = keyPairs[sectionPair][vals];

                    if (tmpValue != null)
                        tmpValue = "=" + tmpValue;

                    strToSave += (vals + tmpValue + "\r\n");
                }

                strToSave += "\r\n";
            }

            try
            {
                TextWriter tw = new StreamWriter(newFilePath);
                tw.Write(strToSave);
                tw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save settings back to ini file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(iniFilePath);
        }
    }
}
