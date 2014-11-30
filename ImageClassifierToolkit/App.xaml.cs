using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism;
using System.Windows.Threading;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Model;
using System.Diagnostics;

namespace ImageClassifierToolkit
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
            #if  DEBUG 
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal;
            #endif

			var bootstrapper = new TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Bootstrapper();
			bootstrapper.Run();
		}

		/// <summary>
		/// Error handler
		/// </summary>
		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
		/*	TiS.Core.TisCommon.Log.WriteException(e.Exception);

			Exception innerException = e.Exception;
			while (innerException.InnerException != null)
			{
				innerException = innerException.InnerException;
			}

			AppDataCenter.Singleton.Message = innerException.Message;

			e.Handled = true;

			Debug.WriteLine(innerException);*/
		}
	}
}
