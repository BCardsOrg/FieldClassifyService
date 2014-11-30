using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Cache;

namespace TiS.Core.TisCommon.Customizations.MethodInvokers.Managed
{
    public interface ICustomAssemblyResolver : IDisposable
    {
        string BaseAssemblyLocation { get; }

        string CustomizationDir { get;set; }

        void ValidateFile(ref string sFullFileName);

        ResolveEventHandler AssemblyResolveHandler { get; }
    }

    [ClassInterface(ClassInterfaceType.None)]
    public class CustomAssemblyResolver : ICustomAssemblyResolver
    {
        private string m_sBaseAssemblyLocation;
        private string m_sCustomizationDir;
        private ResolveEventHandler m_oAssemblyResolveHandler;
        private Dictionary<string, string> m_assemblyNameToFileMap;
        private object m_locker = new object();
        private readonly string m_cacheItemName = "CustomAssemblyResolver";

        public CustomAssemblyResolver(string sEFlowBinDir)
        {
            m_sBaseAssemblyLocation = sEFlowBinDir;

            m_oAssemblyResolveHandler = CustomAssemblyResolveHandler;

			IDataCache cache = CacheFactory.LocalCache;

            lock (m_locker)
            {
                object value;
                if (cache.TryGetCacheItem(m_cacheItemName, out value) == false)
                {
                    m_assemblyNameToFileMap = new Dictionary<string, string>();

                    cache.PutCacheItem(m_cacheItemName, m_assemblyNameToFileMap, true);
                }
                else
                {
                    m_assemblyNameToFileMap = (Dictionary<string, string>)value;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_oAssemblyResolveHandler = null;
        }

        #endregion

        public string BaseAssemblyLocation
        {
            get
            {
                return m_sBaseAssemblyLocation;
            }
        }

        public string CustomizationDir
        {
            get
            {
                return m_sCustomizationDir;
            }
            set
            {
                m_sCustomizationDir = value;
            }
        }

        public ResolveEventHandler AssemblyResolveHandler
        {
            get
            {
                return m_oAssemblyResolveHandler;
            }
        }

        public void ValidateFile(ref string sFullFileName)
        {
            string sValidFile = String.Empty;

            lock (m_locker)
            {
                if (!m_assemblyNameToFileMap.TryGetValue(sFullFileName, out sValidFile))
                {
                    string sFileName = GetFileNameWithoutExtension(sFullFileName);

                    if (!ValidateFile(sFileName, out sValidFile))
                    {
                        ValidateAssembly(sFileName, out sValidFile);
                    }

                    if (sValidFile == String.Empty)
                    {
                        throw new TisException("File [{0}] does not exist", sFullFileName);
                    }
                }
            }

            sFullFileName = sValidFile;
        }

        public bool ResolveAssembly(string sAssemblyName, out string sAssemblyFile)
        {
            bool resolved = true;

            sAssemblyFile = String.Empty;

            try
            {
                lock (m_locker)
                {
                    if (!m_assemblyNameToFileMap.TryGetValue(sAssemblyName, out sAssemblyFile))
                    {
                        if (!ValidateAssembly(sAssemblyName,
                                              m_sCustomizationDir,
                                              new string[] { m_sCustomizationDir },
                                              true,
                                              out sAssemblyFile))
                        {
                            if (!ValidateAssembly(sAssemblyName,
                                                  m_sBaseAssemblyLocation,
                                                  out sAssemblyFile))
                            {

                                if (!ValidateAssembly(sAssemblyName,
                                                      null,
                                                      out sAssemblyFile))
                                {
                                    resolved = false;
                                }
                            }
                        }

                        m_assemblyNameToFileMap.Add(sAssemblyName, sAssemblyFile);
                    }
                    else
                    {
                        resolved = StringUtil.IsStringInitialized(sAssemblyFile);
                    }
                }

                return resolved;
            }
            catch (Exception oExc)
            {
                Log.WriteException(oExc);
            }

            return false;
        }

        private Assembly CustomAssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            string sAssemblyFile;

            if (ResolveAssembly(args.Name, out sAssemblyFile))
            {
                try
                {
                    return Assembly.LoadFile(sAssemblyFile);
                }
                catch (Exception oExc)
                {
                    Log.WriteException(oExc);
                }
            }

            return null;
        }

        private bool ValidateAssembly(string sAssemblyName, out string sAssemblyFile)
        {
            if (!ValidateAssembly(sAssemblyName,
                                  m_sBaseAssemblyLocation,
                                  out sAssemblyFile))
            {
                //string sCustomAsseblyLocation =
                //    Path.Combine(m_sCustomizationDir, sAssemblyName);

                if (!ValidateAssembly(sAssemblyName,
                                      m_sCustomizationDir,
                                      out sAssemblyFile))
                {
                    sAssemblyFile = String.Empty;
                    return false;
                }
            }

            return true;
        }

        private bool ValidateAssembly(string sAssemblyName, string sBasePath, out string sAssemblyFile)
        {
            AssemblyName oAssemblyName = new AssemblyName();
            oAssemblyName.Name = sAssemblyName;

            string sShortAssemblyName = GetShortAssemblyName(oAssemblyName);

            if (StringUtil.IsStringInitialized(sBasePath))
            {
                sAssemblyFile = Path.Combine(sBasePath, sShortAssemblyName + ".dll");

                bool exists = File.Exists(sAssemblyFile);

                if (exists)
                    return true;

                if(exists == false)
                {
                    sAssemblyFile = Path.ChangeExtension(sAssemblyFile, "exe");
                }

                return File.Exists(sAssemblyFile);
            }
            else
            {
                sAssemblyFile = GACUtils.GetAssemblyPath(sShortAssemblyName + ".dll");

                return StringUtil.IsStringInitialized(sAssemblyFile);
            }
        }

        private bool ValidateAssembly(
            string sAssemblyName, 
            string sBasePath, 
            string[] PreferedSubDirs, 
            bool bRecursive, 
            out string sAssemblyFile)
        {
            sAssemblyFile = String.Empty;

            AssemblyName oAssemblyName = new AssemblyName();
            oAssemblyName.Name = sAssemblyName;

            string sShortAssemblyName = GetShortAssemblyName(oAssemblyName);

            string[] Files = TisVSEnvHelper.FindFiles(sBasePath, sShortAssemblyName +  ".dll", true);

            bool bFound = Files.Length > 0;

            if(bFound)
            {
                if (Files.Length > 1)
                {
                    if (PreferedSubDirs != null && PreferedSubDirs.Length > 1)
                    {
                        foreach (string sFile in Files)
                        {
                            foreach (string sPreferedSubDir in PreferedSubDirs)
                            {
                                if (sFile.IndexOf(sPreferedSubDir, StringComparison.InvariantCultureIgnoreCase) > -1)
                                {
                                    sAssemblyFile = sFile;
                                    return bFound;
                                }
                            }
                        }
                    }

                    bFound = false;
                }
                else
                {
                    sAssemblyFile = Files[0];
                }
            }

            return bFound;
        }

        private bool ValidateFile(string sFullFileName, out string sValidFile)
        {
            string sFileDirectory;

            string sFileName = GetFileNameWithoutExtension(sFullFileName);

            if (Path.IsPathRooted(sFullFileName))
            {
                sFileDirectory = Path.GetDirectoryName(sFullFileName);
            }
            else
            {
                sFileDirectory = Directory.GetCurrentDirectory();
            }

            return ValidateAssembly(sFileName, sFileDirectory, out sValidFile);
        }

        private string GetFileNameWithoutExtension(string sFullFileName)
        {
            string sFileName = sFullFileName;

            if (Path.HasExtension(sFullFileName))
            {
                string sFileExt = Path.GetExtension(sFullFileName);

                if (StringUtil.CompareIgnoreCase(sFileExt, ".dll"))
                {
                    sFileName = Path.GetFileNameWithoutExtension(sFullFileName);
                }
            }

            return sFileName;
        }

        private string GetShortAssemblyName(AssemblyName oAssemblyName)
        {
            string[] Parts = oAssemblyName.Name.Split(',');

            if (Parts.Length < 1)
            {
                throw new TisException("Invalid assembly name [{0}]", oAssemblyName);
            }

            return Parts[0];
        }
    }
}
