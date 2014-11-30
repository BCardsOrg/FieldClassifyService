using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Modules
{
	class ModuleAnalyseNum : IModule
	{
		private readonly IRegionManager regionManager;

        public ModuleAnalyseNum(IRegionManager regionManager)
		{
			this.regionManager = regionManager;
		}

		public void Initialize()
		{
			regionManager.RegisterViewWithRegion("AnalyseNumRegion", typeof(Views.AnalyseNumView));
		}

	}
}
