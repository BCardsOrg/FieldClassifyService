using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Customizations
{
    #region EventAssemblyInfo

    [ComVisible(false)]
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class EventAssemblyInfo : IComparable<EventAssemblyInfo>, IDeserializationCallback
    {
        private List<string> m_allAssemblies = new List<string>();

        public EventAssemblyInfo(
            string assemblyPath,
            string assemblyName,
            TisEventsManager eventsManager,
            string customizationDir)
        {
            AssemblyName = assemblyName;
            AssemblyPath = assemblyPath;

            ReferencedAssemblies = new List<string>();

            if (!StringUtil.IsStringInitialized(assemblyPath))
            {
                AssemblyPath = customizationDir;
            }

            try
            {
                ObtainReferencedAssemblies(eventsManager);
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to obtain referenced assemblies for assembly [{0}]. Details : [{1}]", Path.Combine(AssemblyPath, AssemblyName), exc.Message);
            }
        }

        public string AssemblyPath { get; set; }

        [DataMember]
        public string AssemblyName { get; set; }

        [DataMember]
        public List<string> ReferencedAssemblies { get; set; }

        public void ObtainReferencedAssemblies(TisEventsManager eventsManager)
        {
            eventsManager.CustomizationDir = AssemblyPath;

            ReferencedAssemblies.AddRange(eventsManager.GetReferencedAssemblies(
                Path.Combine(AssemblyPath, AssemblyName) + "." + CommonPlatformConsts.CUSTOM_BINARY_EXTENSION));
        }

        public List<string> AllAssemblies
        {
            get
            {
                m_allAssemblies.Clear();

                m_allAssemblies.Add(AssemblyName + "." + CommonPlatformConsts.CUSTOM_BINARY_EXTENSION);

                if (ReferencedAssemblies != null)
                {
                    m_allAssemblies.AddRange(ReferencedAssemblies);
                }

                return m_allAssemblies;
            }
        }

        #region IComparable<EventAssemblyInfo> Members

        public int CompareTo(EventAssemblyInfo other)
        {
            return String.Compare(this.AssemblyName, other.AssemblyName, true);
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            if (m_allAssemblies == null)
            {
                m_allAssemblies = new List<string>();
            }
        }

        #endregion
    }

    #endregion
}