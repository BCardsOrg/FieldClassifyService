using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Modules
{
	class ModuleConfiguration : IModule
	{
		private readonly IRegionManager regionManager;

		public ModuleConfiguration(IRegionManager regionManager)
		{
			this.regionManager = regionManager;
		}

		public void Initialize()
		{
            regionManager.RegisterViewWithRegion("ConfigurationRegion", typeof(Views.BaseConfigurationView));
        }

	}
}

