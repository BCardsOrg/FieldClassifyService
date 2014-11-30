using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Modules
{
	class ModuleAnalyse : IModule
	{
		private readonly IRegionManager regionManager;

        public ModuleAnalyse(IRegionManager regionManager)
		{
			this.regionManager = regionManager;
		}

		public void Initialize()
		{
			regionManager.RegisterViewWithRegion("AnalyseRegion", typeof(Views.AnalyseView));
		}

	}
}
