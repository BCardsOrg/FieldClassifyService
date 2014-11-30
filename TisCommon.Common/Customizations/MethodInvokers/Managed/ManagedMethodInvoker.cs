using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace TiS.Core.TisCommon.Customizations.MethodInvokers.Managed
{
	[Guid ("91D80E99-C3ED-413f-AB51-B433BB70D922")]
	public interface IManagedMethodInvoker
	{
		object InvokeMethod (
			string sAssemblyName, 
			string sClassImplName, 
			string sMethodName,
			ref object [] InParams);
	}

	public class ManagedMethodInvokerViaCom : IManagedMethodInvoker, IDisposable
	{  
		private ManagedMethodInvoker m_oInvoker; 

		public ManagedMethodInvokerViaCom ()
		{
			m_oInvoker = new ManagedMethodInvoker (null);
		}

		#region ITiSMethodInvoker Members

		public object InvokeMethod (
			string sAssemblyName, 
			string sClassImplName, 
			string sMethodName, 
			ref object[] InParams)
		{
			return m_oInvoker.InvokeMethod (
                sAssemblyName,
				sClassImplName,
				sMethodName,
				ref InParams);
		}

		#endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_oInvoker.Dispose();
        }

        #endregion
    }

	internal class ManagedMethodInvoker : BaseMethodInvoker, IManagedMethodInvoker
	{
		private string m_sAssemblyName;
        private ResolveEventHandler m_oAssemblyResolverHandler;
		private static ManagedInstanceCache m_InstanceCache = new ManagedInstanceCache ();
		private static ManagedMethodCache m_MethodCache     = new ManagedMethodCache ();

        public ManagedMethodInvoker(ResolveEventHandler oAssemblyResolverHandler)
            : base(m_MethodCache, m_InstanceCache)
        {
            m_oAssemblyResolverHandler = oAssemblyResolverHandler;
        }

        public override void Dispose()
        {
            m_oAssemblyResolverHandler = null;

            base.Dispose();
        }

		#region ITiSMethodInvoker Members

		public object InvokeMethod (
			string sAssemblyName, 
			string sClassImplName, 
			string sMethodName, 
			ref object[] InParams)
		{
			m_sAssemblyName = sAssemblyName;

            AppDomain.CurrentDomain.AssemblyResolve += m_oAssemblyResolverHandler;

			try
			{
				try
				{
					MethodInfo oMethod = (MethodInfo) GetMethod (
						sAssemblyName, 
						sClassImplName, 
						sMethodName);   

              		sClassImplName = oMethod.ReflectedType.FullName;

					object oInstance = GetInstance (
						sAssemblyName, 
						sClassImplName);

					object oResult;

					try
					{
                        Log.WriteInfo("Calling method {0} [Class : {1} Assembly : {2}]", sMethodName, sClassImplName, sAssemblyName);

                        oResult = oMethod.Invoke(oInstance, InParams);
                    }
					catch (Exception oExc) 
					{
						Log.WriteException (oExc);

						throw new TisException (
							oExc.InnerException, 
							"Failed to invoke method {0} [Class : {1} Assembly : {2}]", 
							sMethodName, sClassImplName, sAssemblyName);
					}

					return oResult;
				}
				catch (Exception oExc) 
				{
					Log.WriteException (oExc);

					throw;
				}
			}
			finally
			{
                AppDomain.CurrentDomain.AssemblyResolve -= m_oAssemblyResolverHandler; 
			}
		}

		#endregion

        private delegate object InvokeMethodDelegate(MethodInfo oMethod, object oInstance, object[] InParams);

		private object InvokeMethodAsync (MethodInfo oMethod, object oInstance, object[] InParams)
		{
			return oMethod.Invoke (oInstance, InParams);
		}
    }
}
