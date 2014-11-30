using System;
using System.Reflection;

namespace TiS.Core.TisCommon.Customizations.MethodInvokers.Managed
{
	internal class ManagedMethodCache : BaseCache
	{
		internal override object OnCacheMissHandler(object oSender, CacheMissEventArgs oArgs)
		{
			string[] MethodArgs = (string[])ParsePersistKey ((string)oArgs.Key);

			string sAssemblyName  = MethodArgs [0];
			string sClassImplName = MethodArgs [1];
			string sMethodName    = MethodArgs [2];

			Assembly oAssembly;

            try
            {
                oAssembly = AppDomain.CurrentDomain.Load(sAssemblyName);
            }
            catch (Exception oExc)
            {
                throw new TisException(
                    oExc.InnerException,
                    "Failed to load assembly {0}",
                    sAssemblyName);
            }

			Type oType;

			try
			{
				oType = oAssembly.GetType (sClassImplName);

				if (oType == null)
				{
					Type [] AssemblyTypes = oAssembly.GetTypes ();

					foreach (Type oAssemblyType in AssemblyTypes)
					{
						if (StringUtil.CompareIgnoreCase(oAssemblyType.Name, sClassImplName))
						{
							oType = oAssemblyType;
							break;
						}
					}
				}

				if (oType == null)
				{
					throw new Exception ("Type is not found");
				}

			}
			catch (Exception oExc) 
			{
				throw new TisException (
					oExc.InnerException, 
					"Failed to obtain type of {0}", 
					sClassImplName);
			}

            MethodInfo oMethod = null;

            try
            {
                oMethod = oType.GetMethod(sMethodName);
            }
            catch (AmbiguousMatchException )
            {
                oMethod =
                    oType.GetMethod(sMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);
            }

			if (oMethod == null)
			{
				throw new TisException (
					"Failed to obtain method {0}", 
					sMethodName);
			}

			return oMethod;
		}

		internal override bool ValidatePersistKey (string[] ParsedPersitKey)
		{
			return ParsedPersitKey.Length == 3;
		}
	}
}
