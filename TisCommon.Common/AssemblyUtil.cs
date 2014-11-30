using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon
{
    #region AssemblyVersionIgnorer

    public class AssemblyVersionIgnorer : IDisposable
    {
        private Thread m_Thread;

        public AssemblyVersionIgnorer()
        {
            m_Thread = Thread.CurrentThread;

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveHandler);
        }

        #region IDisposable Members

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyResolveHandler);
            m_Thread = null;
        }

        #endregion

        private Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            if (m_Thread != Thread.CurrentThread)
            {
                return null;
            }

            try
            {
                AssemblyName assemblyName = new AssemblyName(args.Name);

                if (assemblyName.Version != null)
                {
                    assemblyName.Version = null;
                    return Assembly.Load(assemblyName);
                }
                else
                {
                    // Avoid stack overflow
                    return null;
                }
            }
            catch (Exception oExc)
            {
                Log.WriteException(oExc);
            }

            return null;
        }
    }

    #endregion

    #region TypeResolver

    public class TypeResolver
    {
        private string m_sFullTypeName;
        private string m_sAssemblyName;
        private string m_sShortTypeName;
        private string m_sRuntimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
        private bool m_bIsEnabled;
        private const string TYPE_NAME_DELIMITER = ",";

        private Dictionary<string, Type> m_oResolvedTypesCache = new Dictionary<string, Type>();

       private ObsoleteTypes m_obsoleteTypesResolver = new ObsoleteTypes();

        public bool GetType(string sFullTypeName, out Type oObj)
        {
            if (m_oResolvedTypesCache.TryGetValue(sFullTypeName, out oObj))
            {
                return true;
            }

            m_sFullTypeName = sFullTypeName;

            int nAsmNameSep = sFullTypeName.IndexOf(TYPE_NAME_DELIMITER);

            m_bIsEnabled = nAsmNameSep > 0;

            if (m_bIsEnabled)
            {
                m_sAssemblyName = sFullTypeName.Substring(nAsmNameSep + 1).TrimStart(new char[] { ' ' });

                m_sShortTypeName = sFullTypeName.Substring(0, nAsmNameSep);

                if (TryObsoleteTypes(out oObj) || TryShortTypeName(out oObj) || TryAnotherAssembly(m_sRuntimeDir, out oObj))
                {
                    m_oResolvedTypesCache.Add(m_sFullTypeName, oObj);

                    return true;
                }
            }

            return false;
        }

        public bool IsEnabled
        {
            get
            {
                return m_bIsEnabled;
            }
        }

        private bool TryObsoleteTypes(out Type oObj)
        {
            bool found = false;
            string contemporaryTypeName = null;
            oObj = null;

            if (m_obsoleteTypesResolver.ObtainContemporaryTypeName(m_sFullTypeName, out contemporaryTypeName))
            {
                found = GetType(null, contemporaryTypeName, out oObj);
            }

            return found;
        }

        private bool TryShortTypeName(out Type oObj)
        {
            if (GetType(null, m_sShortTypeName, out oObj) == true)
                return true;
            Assembly asm = null;
            asm = Assembly.Load(m_sAssemblyName);
            if (asm == null)
                return false;
            string[] nameParts = m_sShortTypeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string typeShortName = nameParts[nameParts.Length - 1];
            var res = from c in asm.GetTypes()
                      where c.Name == typeShortName
                      select c;
            foreach (var item in res)
            {
                oObj = item;
                return true;
            }
            return false;
        }

        private bool TryAnotherAssembly(string sAssemblyPath, out Type oObj)
        {
            oObj = null;

            try
            {
                Assembly oAssembly;

                string sAssemblyName =
                    Path.Combine(sAssemblyPath.Trim(), m_sAssemblyName + ".dll");

                if (File.Exists(sAssemblyName))
                {
                    if (Path.IsPathRooted(sAssemblyName))
                    {
                        oAssembly = Assembly.LoadFile(sAssemblyName);
                    }
                    else
                    {
                        oAssembly = Assembly.Load(sAssemblyName);
                    }

                    if (oAssembly != null)
                    {
                        AssemblyName oAssemblyName = oAssembly.GetName();

                        if (!GetType(null, m_sShortTypeName + TYPE_NAME_DELIMITER + oAssemblyName.FullName, out oObj))
                        {
                            if (!GetType(null, m_sShortTypeName + TYPE_NAME_DELIMITER + oAssemblyName.Name, out oObj))
                            {
                                return GetType(oAssembly, m_sShortTypeName, out oObj);
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private bool GetType(Assembly oAssembly, string sTypeName, out Type oObj)
        {
            oObj = null;

            try
            {
                if (oAssembly != null)
                {
                    oObj = oAssembly.GetType(sTypeName, false);
                }
                else
                {
                    oObj = Type.GetType(sTypeName, false);
                }
            }
            catch
            {
                return false;
            }

            return oObj != null;
        }
    }

    #endregion

    #region GACUtils

    public class GACUtils
    {
        [ThreadStatic]
        private static IAssemblyCache m_assemblyCache;

        public static bool IsAssemblyInGAC(string assemblyName)
        {
            return StringUtil.IsStringInitialized(GetAssemblyPath(assemblyName));
        }

        public static string GetAssemblyPath(string assemblyName)
        {
            string assemblyPath = String.Empty;

            assemblyName = Path.GetFileNameWithoutExtension(assemblyName);

            ASSEMBLY_INFO assembyInfo = new ASSEMBLY_INFO();
            assembyInfo.cchBuf = 512;
            assembyInfo.currentAssemblyPath = new String('\0', assembyInfo.cchBuf);

            IntPtr hr;

            if (m_assemblyCache == null)
            {
                hr = CreateAssemblyCache(out m_assemblyCache, 0);

                if (hr != IntPtr.Zero)
                {
                    Log.WriteWarning("Failed to create assembly cache.");

                    return assemblyPath;
                }
            }

            hr = m_assemblyCache.QueryAssemblyInfo(1, assemblyName, ref assembyInfo);

            if (hr == IntPtr.Zero)
            {
                assemblyPath = assembyInfo.currentAssemblyPath;
            }

            return assemblyPath;
        }

        [DllImport("fusion.dll")]
        internal static extern IntPtr CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
        internal interface IAssemblyCache
        {
            int Dummy1();
            [PreserveSig()]
            IntPtr QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref ASSEMBLY_INFO assemblyInfo);
            int Dummy2();
            int Dummy3();
            int Dummy4();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ASSEMBLY_INFO
        {
            public int cbAssemblyInfo;
            public int assemblyFlags;
            public long assemblySizeInKB;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String currentAssemblyPath;
            public int cchBuf;
        }
    }

    #endregion
}
