using System;
using System.Reflection;
using TiS.Core.TisCommon.Reflection;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon
{
	[Guid("dcbdcc72-6186-4f32-abef-440fd0b89c7e")]
    public interface IModuleVersion
	{
		int MajorVersion { get; set; }
		int MinorVersion { get; set; }
		int Build		 { get; set; }
		int Revision	 { get; set; }

		void Set(
			int nMajorVersion, 
			int nMinorVersion,
			int nBuild,
			int nRevision);
	}

	[ComVisible(false)]
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class ModuleVersion : IModuleVersion
	{
        [DataMember]
		private int m_nMajorVersion;
        [DataMember]
        private int m_nMinorVersion;
        [DataMember]
        private int m_nBuild;
        [DataMember]
        private int m_nRevision;

		public ModuleVersion()
		{
		}

		public ModuleVersion(
			int nMajorVersion, 
			int nMinorVersion,
			int nBuild,
			int nRevision)
		{
			Set(
				nMajorVersion,
				nMinorVersion,
				nBuild,
				nRevision);
		}

		public ModuleVersion(IModuleVersion oVer)
		{
			Set(oVer.MajorVersion, oVer.MinorVersion, oVer.Build, oVer.Revision);
		}
		
		public ModuleVersion(string sVersion)
		{
			string[] Parts = sVersion.Split(new char[] { '.' });

			if(Parts.Length != 4)
			{
				throw new TisException("Invalid version format [{0}]," +
					"Should be: MajorVer.MinorVer.Build.Revision",
					sVersion);
			}
		
			try
			{
				m_nMajorVersion = int.Parse(Parts[0]);
				m_nMinorVersion = int.Parse(Parts[1]);
				m_nBuild		= int.Parse(Parts[2]);
				m_nRevision		= int.Parse(Parts[3]);
			}
			catch(Exception oExc)
			{
				throw new TisException(
					oExc, 
					"Invalid version format [{0}]",
					oExc);
			}
		}

        public static IModuleVersion PlatformVersion
        {
            get
            {
                Assembly oThisAssembly = Assembly.GetExecutingAssembly();

                AssemblyFileVersionAttribute oAssemblyFileVer = (AssemblyFileVersionAttribute)ReflectionUtil.GetAttribute(
                    oThisAssembly,
                    typeof(AssemblyFileVersionAttribute));

                string sVersion = oAssemblyFileVer.Version;

                return new ModuleVersion(sVersion);
            }
        }

        public void Set(
			int nMajorVersion, 
			int nMinorVersion,
			int nBuild,
			int nRevision)
		{
			m_nMajorVersion = nMajorVersion;
			m_nMinorVersion = nMinorVersion;
			m_nBuild		= nBuild;
			m_nRevision		= nRevision;
		}

		public int MajorVersion
		{
			get { return m_nMajorVersion;  }
			set { m_nMajorVersion = value; }
		}

		public int MinorVersion
		{
			get { return m_nMinorVersion;  }
			set { m_nMinorVersion = value; }
		}

		public int Build
		{
			get { return m_nBuild;  }
			set { m_nBuild = value; }
		}

		public int Revision
		{
			get { return m_nRevision;  }
			set { m_nRevision = value; }
		}

		public int[] GetAsArray()
		{
			return new int[] { m_nMajorVersion, m_nMinorVersion, m_nBuild, m_nRevision };
		}

		public override string ToString()
		{
			return String.Format("{0}.{1}.{2}.{3}",
				m_nMajorVersion,
				m_nMinorVersion,
				m_nBuild,
				m_nRevision);
		}
		
		public override bool Equals(object obj)
		{
			if(Object.ReferenceEquals(obj, null))
			{
				return false;
			}

			ModuleVersion oOther = (ModuleVersion)obj;

			return ArrayCompare(
				GetAsArray(),
				oOther.GetAsArray()) == 0;
		}

		public static bool operator == (ModuleVersion oV1, ModuleVersion oV2)
		{
			if(Object.ReferenceEquals(oV1, null))
			{
				return false;
			}
			
			return oV1.Equals(oV2);
		}

		public static bool operator != (ModuleVersion oV1, ModuleVersion oV2)
		{
			return !(oV1 == oV2);
		}

		public static bool operator < (ModuleVersion oV1, ModuleVersion oV2)
		{
			return ArrayCompare(
				oV1.GetAsArray(),
				oV2.GetAsArray()) < 0;
		}

		public static bool operator > (ModuleVersion oV1, ModuleVersion oV2)
		{
			return ArrayCompare(
				oV1.GetAsArray(),
				oV2.GetAsArray()) > 0;
		}

		public override int GetHashCode()
		{
			return GetSum().GetHashCode();
		}

		//
		// Private
		//

		private int GetSum()
		{
			int nSum = 0;

			int[] Nums = GetAsArray();

			foreach(int n in Nums)
			{
				nSum += n;
			}

			return nSum;
		}

		private static int ArrayCompare(int[] A1, int[] A2)
		{
			if(A1.Length != A2.Length)
			{
				throw new TisException("Arrays must be same length");
			}

			for(int i=0; i<A1.Length; i++)
			{
				int n1 = A1[i];
				int n2 = A2[i];

				if(n1 < n2)
				{
					return -1;
				}
				if(n1 > n2)
				{
					return 1;
				}
			}

			return 0;
		}
	}
}
