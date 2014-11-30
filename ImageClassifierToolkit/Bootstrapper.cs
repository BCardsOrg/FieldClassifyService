using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using ImageClassifierToolkit;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Modules;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.ViewModel;

using Microsoft.Practices.Prism.UnityExtensions;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit
{
	class Bootstrapper : UnityBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			return new Shell();
		}

		protected override void InitializeShell()
		{
			//base.InitializeShell();

			App.Current.MainWindow = (Window)this.Shell;
			App.Current.MainWindow.Show();
		}
		protected override void ConfigureModuleCatalog()
		{
			base.ConfigureModuleCatalog();

			ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

            moduleCatalog.AddModule(typeof(ModuleSections));
            moduleCatalog.AddModule(typeof(ModuleConfiguration));
            moduleCatalog.AddModule(typeof(ModuleCategorize));
            moduleCatalog.AddModule(typeof(ModuleTrainTest));
            moduleCatalog.AddModule(typeof(ModuleRun));
            moduleCatalog.AddModule(typeof(ModuleReport));
            moduleCatalog.AddModule(typeof(ModuleAnalyse));
            moduleCatalog.AddModule(typeof(ModuleAnalyseNum));
            moduleCatalog.AddModule(typeof(ModuleHistory));
            moduleCatalog.AddModule(typeof(ModuleStatusBar));
        }
	}
}
