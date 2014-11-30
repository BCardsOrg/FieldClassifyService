using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Modules
{
	class ModuleSections : IModule
	{
		private readonly IRegionManager regionManager;

		public ModuleSections(IRegionManager regionManager)
		{
			this.regionManager = regionManager;
		}

		public void Initialize()
		{
            regionManager.RegisterViewWithRegion("SectionsRegion", typeof(Views.SectionsView));
		}

	}
}

