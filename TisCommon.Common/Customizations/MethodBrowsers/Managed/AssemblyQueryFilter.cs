using System;

namespace TiS.Core.TisCommon.Customizations.MethodBrowsers.Managed
{
    [Serializable]
	internal class AssemblyQueryFilter : IAssemblyQueryFilter
	{
		private readonly string m_sTypeName;
		private readonly int m_nMethodParamsCount;
		private readonly string[] m_MethodTypeOfParams;
		private readonly string m_sMethodReturnType;

		public AssemblyQueryFilter(
			string sTypeName,
			int nMethodParamsCount, 
			string[] MethodTypeOfParams, 
			string sMethodReturnType)
		{
			m_sTypeName          = sTypeName;
			m_nMethodParamsCount = nMethodParamsCount;
			m_MethodTypeOfParams = MethodTypeOfParams;
			m_sMethodReturnType  = sMethodReturnType;
		}

		#region IAssemblyQueryFilter Members

		public string TypeName
		{
			get
			{
				return m_sTypeName; 
			}
		}

		public int MethodParamsCount
		{
			get
			{
				return m_nMethodParamsCount;
			}
		}

		public string[] MethodTypeOfParams
		{
			get
			{
				return m_MethodTypeOfParams;
			}
		}

		public string MethodReturnType
		{
			get
			{
				return m_sMethodReturnType;
			}
		}

		#endregion
	}
}
