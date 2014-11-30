using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TiS.Core.TisCommon.Cache;

namespace TiS.Core.TisCommon
{
    public interface IGacAssemblyResolver : IDisposable
    {
        void ValidateFile(ref string sFullFileName);

        ResolveEventHandler AssemblyResolveHandler { get; }
    }

    [ClassInterface(ClassInterfaceType.None)]
    public class GacAssemblyResolver : IGacAssemblyResolver
    {
        private ResolveEventHandler m_oAssemblyResolveHandler;
        private Dictionary<string, string> m_assemblyNameToFileMap;
        private object m_locker = new object();
        private readonly string m_cacheItemName = "GacAssemblyResolver";

        public GacAssemblyResolver()
        {
            m_oAssemblyResolveHandler = GacAssemblyResolveHandler;

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

        public ResolveEventHandler AssemblyResolveHandler
        {
            get
            {
                return m_oAssemblyResolveHandler;
            }
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
                        ValidateAssembly(sAssemblyName, out sAssemblyFile);

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

        private Assembly GacAssemblyResolveHandler(object sender, ResolveEventArgs args)
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

        public void ValidateFile(ref string sFullFileName)
        {
            string sValidFile = String.Empty;

            lock (m_locker)
            {
                if (!m_assemblyNameToFileMap.TryGetValue(sFullFileName, out sValidFile))
                {
                    string sFileName = GetFileNameWithoutExtension(sFullFileName);

                    ValidateAssembly(sFileName, out sValidFile);

                    if (sValidFile == String.Empty)
                    {
                        throw new TisException("File [{0}] does not exist", sFullFileName);
                    }
                }
            }

            sFullFileName = sValidFile;
        }

        private bool ValidateAssembly(string sAssemblyName, out string sAssemblyFile)
        {
            AssemblyName oAssemblyName = new AssemblyName();
            oAssemblyName.Name = sAssemblyName;

            string sShortAssemblyName = GetShortAssemblyName(oAssemblyName);
            sAssemblyFile = GACUtils.GetAssemblyPath(sShortAssemblyName + ".dll");

            return StringUtil.IsStringInitialized(sAssemblyFile);
        }
    }
}
