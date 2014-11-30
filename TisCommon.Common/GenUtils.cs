using System;
using System.Security.Principal;

namespace TiS.Core.TisCommon
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public class GenUtils
	{
        [ThreadStatic]
        private static string m_userName = String.Empty;

		public static void SafeDispose(IDisposable oDisposable)
		{
			if(oDisposable != null)
			{	
				// Dispose 
				oDisposable.Dispose();
			}
		}

		public static string GetUserName()
		{
            if (!StringUtil.IsStringInitialized(m_userName))
            {
                WindowsIdentity oIdentity = WindowsIdentity.GetCurrent();

                if (oIdentity != null)
                {
                    m_userName = oIdentity.Name;
                }
            }

            return m_userName;
		}
	}
}
