using System;
using System.Runtime.Remoting;

namespace TiS.Core.TisCommon.Customizations.MethodInvokers.Managed
{
	internal class ManagedInstanceCache : BaseCache
	{
		internal override object OnCacheMissHandler(object oSender, CacheMissEventArgs oArgs)
		{
			string[] InstanceArgs = (string[]) ParsePersistKey ((string) oArgs.Key);

			string sAssemblyName  = InstanceArgs [0];
			string sClassImplName = InstanceArgs [1];

			ObjectHandle oWrappedObject;

			try
			{
				oWrappedObject = 
					AppDomain.CurrentDomain.CreateInstance (sAssemblyName, sClassImplName);
			}
			catch (Exception oExc) 
			{
				throw new TisException (
					oExc.InnerException, 
					"Failed to create an instance of {0} [Assembly : {1}]", 
					sClassImplName, sAssemblyName);
			}

			return oWrappedObject.Unwrap ();
		}

		internal override bool ValidatePersistKey (string[] ParsedPersitKey)
		{
			return ParsedPersitKey.Length == 2;
		}
	}
}
